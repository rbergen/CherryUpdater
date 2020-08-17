using System.Windows.Forms;

namespace CherryUpdater
{
	public class DecoratedMessage
	{
		public DecoratedMessage(string message, string caption, bool? showOnce, bool? showIfFailed, MessageBoxButtons? buttons, MessageBoxIcon? type)
		{
			Message = message;
			Caption = caption;
			ShowOnce = showOnce ?? false;
			Shown = false;
			ShowIfFailed = showIfFailed ?? false;
			Buttons = buttons ?? MessageBoxButtons.OK;
			Type = type ?? MessageBoxIcon.None;
		}

		public DecoratedMessage(string message, string caption) : this(message, caption, null, null, null, null) { }

		public bool ShowOnce
		{
			get;
			private set;
		}

		public bool Shown
		{
			get;
			set;
		}

		public bool ShowIfFailed
		{
			get;
			private set;
		}

		public MessageBoxButtons Buttons
		{
			get;
			private set;
		}

		public MessageBoxIcon Type
		{
			get;
			private set;
		}

		public string Caption
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}
	}
}
