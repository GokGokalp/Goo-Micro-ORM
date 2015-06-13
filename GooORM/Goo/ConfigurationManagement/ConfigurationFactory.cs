using System;

namespace Goo.ConfigurationManagement
{
    internal class ConfigurationFactory
    {
        #region Constructor
        private static object objLock = new object();
        private static ConfigurationFactory m_configurationFactory;
        public static ConfigurationFactory getInstance
        {
            get
            {
                if (m_configurationFactory == null)
                {
                    lock (objLock)
                    {
                        if (m_configurationFactory == null)
                            m_configurationFactory = new ConfigurationFactory();
                    }
                }

                return m_configurationFactory;
            }
            set { m_configurationFactory = value; }
        }

        #endregion

        public ConfigurationBase GetConfigReaderObject(ConfigurationBase configurationBase = null)
        {
            string GokConfigurationManagmentDefaultType = string.Empty;
            if (configurationBase == null)
            {
                AppConfigurationReader appConfigReader = new AppConfigurationReader();

                GokConfigurationManagmentDefaultType = (string)appConfigReader.Get("gooConfigurationDefaultType");
            }
            else
                return configurationBase;

            if (GokConfigurationManagmentDefaultType == "appConfig")
                return new AppConfigurationReader();
            else
                return new AppConfigurationReader();
        }
    }
}