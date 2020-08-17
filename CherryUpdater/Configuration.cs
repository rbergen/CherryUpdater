using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Managed.Adb;

namespace CherryUpdater
{
    [Serializable]
    public class Configuration : ISerializable
    {
        #region Member variables

        private bool mEnableLogging;
        private string mLogLevelValue;
        private bool mIgnoreProductModel;
        private bool mShowHelp;

        #endregion

        #region Constants

        private const string cFileName = "config.bin";

        #endregion

        #region Constructor

        public Configuration()
        {
            setMemberStartValues();
            fillEmptyMembers();
        }

        protected Configuration(SerializationInfo info, StreamingContext context)
        {
            setMemberStartValues();

            try
            {
                mIgnoreProductModel = info.GetBoolean("mIgnoreProductModel");
            }
            catch { }

            try
            {
                mShowHelp = info.GetBoolean("mShowHelp");
            }
            catch { }

            try
            {
                mEnableLogging = info.GetBoolean("mEnableLogging");
            }
            catch { }

            try
            {
                mLogLevelValue = info.GetString("mLogLevelValue");
            }
            catch { }

            fillEmptyMembers();
        }

        #endregion

        #region Public static methods

        public static Configuration Load()
        {
            try
            {
                if (File.Exists(cFileName))
                {
                    using (FileStream serializationStream = new FileStream(cFileName, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        Configuration configuration = (Configuration)formatter.Deserialize(serializationStream);

                        return configuration;
                    }
                }
            }
            catch { }

            return new Configuration();
        }

        #endregion

        #region Public methods

        public void Save()
        {
            try
            {
                using (FileStream serializationStream = new FileStream(cFileName, FileMode.Create))
                {
                    new BinaryFormatter().Serialize(serializationStream, this);
                }
            }
            catch { }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("mIgnoreProductModel", mIgnoreProductModel);
            info.AddValue("mShowHelp", mShowHelp);
            info.AddValue("mEnableLogging", mEnableLogging);
            info.AddValue("mLogLevelValue", mLogLevelValue);
        }

        #endregion

        #region Private methods

        private void setMemberStartValues()
        {
            mIgnoreProductModel = false;
            mShowHelp = true;
            mEnableLogging = true;
            mLogLevelValue = null;
        }

        private void fillEmptyMembers()
        {
            if (mLogLevelValue == null)
            {
                mLogLevelValue = LogLevel.Verbose.Value;
            }
        }

        #endregion

        #region Properties

        public bool IgnoreProductModel
        {
            get
            {
                return mIgnoreProductModel;
            }
            set
            {
                mIgnoreProductModel = value;
            }
        }

        public bool ShowHelp
        {
            get
            {
                return mShowHelp;
            }
            set
            {
                mShowHelp = value;
            }
        }

        public bool EnableLogging
        {
            get
            {
                return mEnableLogging;
            }
            set
            {
                mEnableLogging = value;
            }
        }

        public LogLevel.LogLevelInfo LogLevelInfo
        {
            get
            {
                return LogLevel.Levels[mLogLevelValue];
            }
            set
            {
                mLogLevelValue = value.Value;
            }
        }

        #endregion
    }
}
