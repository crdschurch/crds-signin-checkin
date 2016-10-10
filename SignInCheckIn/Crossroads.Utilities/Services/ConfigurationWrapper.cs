using System;
using System.Configuration;
using Crossroads.Utilities.Interfaces;

namespace Crossroads.Utilities.Services
{
    /// <summary>
    /// This class will retrieve configuration values from config files or environment variables.
    /// If you are expecting a certain type of data, you can parse the value here before returning the value. 
    /// </summary>
    public class ConfigurationWrapper : IConfigurationWrapper
    {
        public int GetConfigIntValue(string key)
        {
            int pageId;
            if (!int.TryParse(ConfigurationManager.AppSettings[key], out pageId))
            {
                throw new InvalidOperationException(string.Format("Invalid App Setting Key: {0}", key));
            }
            return pageId;
        }

        public string GetConfigValue(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                throw new ApplicationException(string.Format("Invalid App Setting Key: {0}", key));
            }
            return ConfigurationManager.AppSettings[key];
        }

        public string GetEnvironmentVarAsString(string variable)
        {
            return GetEnvironmentVarAsString(variable, true);
        }

        public string GetEnvironmentVarAsString(string variable, bool throwIfNotFound)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (value == null && throwIfNotFound)
            {
                throw new ApplicationException(string.Format("Invalid Environment Variable: {0}", variable));
            }
            return value;
        }
    }
}