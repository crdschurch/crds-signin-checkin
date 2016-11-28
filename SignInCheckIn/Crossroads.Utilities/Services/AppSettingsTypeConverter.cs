using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crossroads.Utilities.Services.Interfaces;

namespace Crossroads.Utilities.Services
{
    public class AppSettingsTypeConverter : TypeConverter
    {
        private readonly IConfigurationWrapper _configurationWrapper;

        public AppSettingsTypeConverter()
        {
            _configurationWrapper = new ConfigurationWrapper();
        }

        /// <summary>
        /// Construct a new AppSettingsTypeConverter with the given IConfigurationWrapper.
        /// This is intended primarily for unit testing, as a TypeConverter requires a 
        /// zero-arg constructor to be used by Unity.
        /// </summary>
        /// <param name="configurationWrapper"></param>
        public AppSettingsTypeConverter(IConfigurationWrapper configurationWrapper)
        {
            _configurationWrapper = configurationWrapper;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (_configurationWrapper.GetConfigValue((string)value));
        }
    }
}
