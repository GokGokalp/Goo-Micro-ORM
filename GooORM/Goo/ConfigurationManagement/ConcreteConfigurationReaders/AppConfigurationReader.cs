using System.Collections.Specialized;
using System.Configuration;

namespace Goo.ConfigurationManagement
{
    internal class AppConfigurationReader : ConfigurationBase
    {
        protected override object GetValue(string key)
        {
            var gooSection = ConfigurationManager.GetSection("goo") as NameValueCollection;

            return gooSection[key];
        }
    }
}