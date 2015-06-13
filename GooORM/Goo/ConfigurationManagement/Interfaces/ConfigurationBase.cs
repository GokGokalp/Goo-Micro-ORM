namespace Goo.ConfigurationManagement
{
    internal abstract class ConfigurationBase
    {
        public object Get(string key)
        {
            object val = GetValue(key);

            return val;
        }

        protected abstract object GetValue(string key);
    }
}