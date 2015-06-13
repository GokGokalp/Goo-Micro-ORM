using Goo.Helpers;

namespace Goo.ConfigurationManagement
{
    internal class ConfigurationManagementManager
    {
        private ConfigurationBase _configurationBase;
        public ConfigurationManagementManager(ConfigurationBase configurationBase = null)
        {
            this._configurationBase = configurationBase;
        }

        public T Get<T>(string key)
        {
            ConfigurationFactory crf = new ConfigurationFactory();
            ConfigurationBase rb;

            if (_configurationBase == null)
                rb = crf.GetConfigReaderObject();
            else
                rb = _configurationBase;
            return OrmHelper.getInstance.ConvertTo<T>(rb.Get(key));
        }
    }
}