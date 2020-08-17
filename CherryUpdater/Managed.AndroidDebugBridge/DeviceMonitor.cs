using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Managed.Adb
{
	/// <summary>
	/// A Device monitor. This connects to the Android Debug Bridge and get device and
	/// debuggable process information from it.
	/// </summary>
	public class DeviceMonitor
	{
		/// <summary>
		/// 
		/// </summary>
		private const string TAG = "DeviceMonitor";

		/// <summary>
		/// 
		/// </summary>
		private readonly byte[] LengthBuffer = null;


		/// <summary>
		/// Initializes a new instance of the <see cref="DeviceMonitor"/> class.
		/// </summary>
		/// <param name="bridge">The bridge.</param>
		public DeviceMonitor(AndroidDebugBridge bridge)
		{
			Server = bridge;
			Devices = new List<Device>();
			DebuggerPorts = new List<int>();
			ClientsToReopen = new Dictionary<IClient, int>();
			DebuggerPorts.Add(DdmPreferences.DebugPortBase);
			LengthBuffer = new byte[4];
		}

		/// <summary>
		/// Gets the devices.
		/// </summary>
		public List<Device> Devices { get; private set; }
		/// <summary>
		/// Gets the debugger ports.
		/// </summary>
		public List<int> DebuggerPorts { get; private set; }
		/// <summary>
		/// Gets the clients to reopen.
		/// </summary>
		public Dictionary<IClient, int> ClientsToReopen { get; private set; }
		/// <summary>
		/// Gets the server.
		/// </summary>
		public AndroidDebugBridge Server { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is monitoring.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is monitoring; otherwise, <c>false</c>.
		/// </value>
		public bool IsMonitoring { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance is running.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is running; otherwise, <c>false</c>.
		/// </value>
		public bool IsRunning { get; private set; }
		/// <summary>
		/// Gets the connection attempt count.
		/// </summary>
		public int ConnectionAttemptCount { get; private set; }
		/// <summary>
		/// Gets the restart attempt count.
		/// </summary>
		public int RestartAttemptCount { get; private set; }
		/// <summary>
		/// Gets a value indicating whether this instance has initial device list.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has initial device list; otherwise, <c>false</c>.
		/// </value>
		public bool HasInitialDeviceList { get; private set; }
		/// <summary>
		/// Gets or sets the main adb connection.
		/// </summary>
		/// <value>
		/// The main adb connection.
		/// </value>
		private Socket MainAdbConnection { get; set; }

		/// <summary>
		/// Adds the client to drop and reopen.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="port">The port.</param>
		public void AddClientToDropAndReopen(IClient client, int port)
		{
			lock (ClientsToReopen)
			{
				Log.D(TAG, "Adding {0} to list of client to reopen ({1})", client, port);
				if (!ClientsToReopen.ContainsKey(client))
				{
					ClientsToReopen.Add(client, port);
				}
			}
		}

		/// <summary>
		/// Starts the monitoring
		/// </summary>
		public void Start()
		{
			Thread t = new Thread(new ThreadStart(DeviceMonitorLoop))
			{
				Name = "Device List Monitor",
				IsBackground = true
			};
			t.Start();
		}

		/// <summary>
		/// Stops the monitoring
		/// </summary>
		public void Stop()
		{
			IsRunning = false;

			// wakeup the main loop thread by closing the main connection to adb.
			try
			{
				if (MainAdbConnection != null)
				{
					MainAdbConnection.Close();
				}
			}
			catch (IOException)
			{
			}

			// wake up the secondary loop by closing the selector.
			/*if ( Selector != null ) {
					Selector.WakeUp ( );
			}*/
		}

		/// <summary>
		/// Monitors the devices. This connects to the Debug Bridge
		/// </summary>
		private void DeviceMonitorLoop()
		{
			IsRunning = true;
			do
			{
				try
				{
					if (MainAdbConnection == null)
					{
						Log.D(TAG, "Opening adb connection");
						MainAdbConnection = OpenAdbConnection();

						if (MainAdbConnection == null)
						{
							ConnectionAttemptCount++;
							Log.E(TAG, "Connection attempts: {0}", ConnectionAttemptCount);

							if (ConnectionAttemptCount > 10)
							{
								if (Server.Restart() == false)
								{
									RestartAttemptCount++;
									Log.E(TAG, "adb restart attempts: {0}", RestartAttemptCount);
								}
								else
								{
									RestartAttemptCount = 0;
								}
							}
							WaitBeforeContinue();
						}
						else
						{
							Log.D(TAG, "Connected to adb for device monitoring");
							ConnectionAttemptCount = 0;
						}
					}
					//break;
					if (MainAdbConnection != null && !IsMonitoring)
					{
						IsMonitoring = SendDeviceListMonitoringRequest();
					}

					if (IsMonitoring)
					{
						// read the length of the incoming message
						int length = ReadLength(MainAdbConnection, LengthBuffer);

						if (length >= 0)
						{
							// read the incoming message
							ProcessIncomingDeviceData(length);

							// flag the fact that we have build the list at least once.
							HasInitialDeviceList = true;
						}
					}
				}
				catch (IOException ioe)
				{
					Log.E(TAG, "Adb connection Error: ", ioe);
					IsMonitoring = false;
					if (MainAdbConnection != null)
					{
						try
						{
							MainAdbConnection.Close();
						}
						catch (IOException)
						{
							// we can safely ignore that one.
						}
						MainAdbConnection = null;
					}
				}
				catch (Exception /*ex*/ )
				{
					//Console.WriteLine ( ex );
				}
			} while (IsRunning);
		}

		/// <summary>
		/// Waits before continuing.
		/// </summary>
		private void WaitBeforeContinue()
		{
			Thread.Sleep(1000);
		}

		/// <summary>
		/// Sends the device list monitoring request.
		/// </summary>
		/// <returns></returns>
		private bool SendDeviceListMonitoringRequest()
		{
			byte[] request = AdbHelper.Instance.FormAdbRequest("host:track-devices");

			if (AdbHelper.Instance.Write(MainAdbConnection, request) == false)
			{
				Log.E(TAG, "Sending Tracking request failed!");
				MainAdbConnection.Close();
				throw new IOException("Sending Tracking request failed!");
			}

			AdbResponse resp = AdbHelper.Instance.ReadAdbResponse(MainAdbConnection, false /* readDiagstring */);

			if (!resp.IOSuccess)
			{
				Log.E(TAG, "Failed to read the adb response!");
				MainAdbConnection.Close();
				throw new IOException("Failed to read the adb response!");
			}

			if (!resp.Okay)
			{
				// request was refused by adb!
				Log.E(TAG, "adb refused request: {0}", resp.Message);
			}

			return resp.Okay;
		}

		/// <summary>
		/// Processes the incoming device data.
		/// </summary>
		/// <param name="length">The length.</param>
		private void ProcessIncomingDeviceData(int length)
		{
			List<Device> list = new List<Device>();

			if (length > 0)
			{
				byte[] buffer = new byte[length];
				string result = Read(MainAdbConnection, buffer);

				string[] devices = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

				foreach (string d in devices)
				{
					try
					{
						Log.V(TAG, "Received device data: {0}", d);
						Device device = Device.CreateFromAdbData(d);
						if (device != null)
						{
							list.Add(device);
						}
					}
					catch (ArgumentException ae)
					{
						Log.E(TAG, ae);
					}
				}

			}

			// now merge the new devices with the old ones.
			UpdateDevices(list);
		}

		private void UpdateDevices(List<Device> list)
		{
			// because we are going to call mServer.deviceDisconnected which will acquire this lock
			// we lock it first, so that the AndroidDebugBridge lock is always locked first.
			lock (AndroidDebugBridge.GetLock())
			{
				lock (Devices)
				{
					// For each device in the current list, we look for a matching the new list.
					// * if we find it, we update the current object with whatever new information
					//   there is
					//   (mostly state change, if the device becomes ready, we query for build info).
					//   We also remove the device from the new list to mark it as "processed"
					// * if we do not find it, we remove it from the current list.
					// Once this is done, the new list contains device we aren't monitoring yet, so we
					// add them to the list, and start monitoring them.

					for (int d = 0; d < Devices.Count;)
					{
						Device device = Devices[d];

						// look for a similar device in the new list.
						int count = list.Count;
						bool foundMatch = false;
						for (int dd = 0; dd < count; dd++)
						{
							Device newDevice = list[dd];
							// see if it matches in id and serial number.
							if (string.Compare(newDevice.SerialNumber, device.SerialNumber, true) == 0)
							{
								foundMatch = true;

								// update the state if needed.
								if (device.State != newDevice.State)
								{
									device.State = newDevice.State;

									// if the device just got ready/online, we need to start
									// monitoring it.
									if (device.IsOnline)
									{
										if (AndroidDebugBridge.ClientSupport)
										{
											if (StartMonitoringDevice(device) == false)
											{
												Log.E(TAG, "Failed to start monitoring {0}", device.SerialNumber);
											}
										}

										if (device.Properties.Count == 0)
										{
											device.RetrieveDeviceInfo();
										}
									}

									device.OnStateChanged(EventArgs.Empty);
								}

								// remove the new device from the list since it's been used
								list.RemoveAt(dd);
								break;
							}
						}

						if (foundMatch == false)
						{
							// the device is gone, we need to remove it, and keep current index
							// to process the next one.
							RemoveDevice(device);
							device.State = DeviceState.Offline;
							device.OnStateChanged(EventArgs.Empty);
							Server.OnDeviceDisconnected(new DeviceEventArgs(device));
						}
						else
						{
							// process the next one
							d++;
						}
					}

					// at this point we should still have some new devices in newList, so we
					// process them.
					foreach (Device newDevice in list)
					{
						// add them to the list
						Devices.Add(newDevice);
						if (Server != null)
						{
							newDevice.State = DeviceState.Online;
							newDevice.RetrieveDeviceInfo(false);
							newDevice.OnStateChanged(EventArgs.Empty);
							Server.OnDeviceConnected(new DeviceEventArgs(newDevice));
						}

						// start monitoring them.
						if (AndroidDebugBridge.ClientSupport)
						{
							if (newDevice.IsOnline)
							{
								StartMonitoringDevice(newDevice);
							}
						}
					}
				}
			}
			list.Clear();
		}

		/// <summary>
		/// Removes the device.
		/// </summary>
		/// <param name="device">The device.</param>
		private void RemoveDevice(Device device)
		{
			//device.Clients.Clear ( );
			Devices.Remove(device);

			Socket channel = device.ClientMonitoringSocket;
			if (channel != null)
			{
				try
				{
					channel.Close();
				}
				catch (IOException)
				{
					// doesn't really matter if the close fails.
				}
			}
		}

		private bool StartMonitoringDevice(Device device)
		{
			Socket socket = OpenAdbConnection();

			if (socket != null)
			{
				try
				{
					bool result = SendDeviceMonitoringRequest(socket, device);
					if (result)
					{

						/*if ( Selector == null ) {
								StartDeviceMonitorThread ( );
						}*/

						device.ClientMonitoringSocket = socket;

						lock (Devices)
						{
							// always wakeup before doing the register. The synchronized block
							// ensure that the selector won't select() before the end of this block.
							// @see deviceClientMonitorLoop
							//Selector.wakeup ( );

							socket.Blocking = true;
							//socket.register(mSelector, SelectionKey.OP_READ, device);
						}

						return true;
					}
				}
				catch (IOException e)
				{
					try
					{
						// attempt to close the socket if needed.
						socket.Close();
					}
					catch (IOException /*e1*/ )
					{
						// we can ignore that one. It may already have been closed.
					}
					Log.D(TAG, "Connection Failure when starting to monitor device '{0}' : {1}", device, e.Message);
				}
			}

			return false;
		}

		/// <summary>
		/// Sends the device monitoring request.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <param name="device">The device.</param>
		/// <returns></returns>
		private bool SendDeviceMonitoringRequest(Socket socket, Device device)
		{
			AdbHelper.Instance.SetDevice(socket, device);
			byte[] request = AdbHelper.Instance.FormAdbRequest("track-jdwp");
			if (!AdbHelper.Instance.Write(socket, request))
			{
				Log.E(TAG, "Sending jdwp tracking request failed!");
				socket.Close();
				throw new IOException();
			}
			AdbResponse resp = AdbHelper.Instance.ReadAdbResponse(socket, false /* readDiagstring */);
			if (resp.IOSuccess == false)
			{
				Log.E(TAG, "Failed to read the adb response!");
				socket.Close();
				throw new IOException();
			}

			if (resp.Okay == false)
			{
				// request was refused by adb!
				Log.E(TAG, "adb refused request: " + resp.Message);
			}

			return resp.Okay;
		}

		/// <summary>
		/// Adds the port to available list.
		/// </summary>
		/// <param name="port">The port.</param>
		public void AddPortToAvailableList(int port)
		{
			if (port > 0)
			{
				lock (DebuggerPorts)
				{
					// because there could be case where clients are closed twice, we have to make
					// sure the port number is not already in the list.
					if (DebuggerPorts.IndexOf(port) == -1)
					{
						// add the port to the list while keeping it sorted. It's not like there's
						// going to be tons of objects so we do it linearly.
						int count = DebuggerPorts.Count;
						for (int i = 0; i < count; i++)
						{
							if (port < DebuggerPorts[i])
							{
								DebuggerPorts.Insert(i, port);
								break;
							}
						}
						// TODO: check if we can compact the end of the list.
					}
				}
			}
		}

		/// <summary>
		/// Reads the length of the next message from a socket.
		/// </summary>
		/// <param name="socket">The Socket to read from.</param>
		/// <param name="buffer"></param>
		/// <returns>the length, or 0 (zero) if no data is available from the socket.</returns>
		private int ReadLength(Socket socket, byte[] buffer)
		{
			string msg = Read(socket, buffer);
			if (msg != null)
			{
				try
				{
					int len = int.Parse(msg, System.Globalization.NumberStyles.HexNumber);
					return len;
				}
				catch (FormatException /*nfe*/ )
				{
					// we'll throw an exception below.
				}
			}
			//throw new IOException ( "unable to read data length" );
			// we receive something we can't read. It's better to reset the connection at this point.
			return -1;
		}

		/// <summary>
		/// Reads the specified socket.
		/// </summary>
		/// <param name="socket">The socket.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		private string Read(Socket socket, byte[] data)
		{
			int count = -1;
			int totalRead = 0;

			while (count != 0 && totalRead < data.Length)
			{
				try
				{
					int left = data.Length - totalRead;
					int buflen = left < socket.ReceiveBufferSize ? left : socket.ReceiveBufferSize;

					byte[] buffer = new byte[buflen];
					socket.ReceiveBufferSize = buffer.Length;
					count = socket.Receive(buffer, buflen, SocketFlags.None);
					if (count < 0)
					{
						throw new IOException("EOF");
					}
					else if (count == 0)
					{
					}
					else
					{
						Array.Copy(buffer, 0, data, totalRead, count);
						totalRead += count;
					}
				}
				catch (SocketException sex)
				{
					if (sex.Message.Contains("connection was aborted"))
					{
						// ignore this?
						return string.Empty;
					}
					else
					{
						throw new IOException(string.Format("No Data to read: {0}", sex.Message));
					}
				}
			}

			return StringHelper.GetString(data, AdbHelper.DEFAULT_ENCODING);
		}

		/// <summary>
		/// Attempts to connect to the debug bridge server.
		/// </summary>
		/// <returns>a connect socket if success, null otherwise</returns>
		private Socket OpenAdbConnection()
		{
			Log.D(TAG, "Connecting to adb for Device List Monitoring...");
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.Connect(AndroidDebugBridge.SocketAddress);
				socket.NoDelay = true;
			}
			catch (Exception e)
			{
				Log.W(TAG, e);
				socket = null;
			}

			return socket;
		}
	}
}
