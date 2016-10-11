using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using Crossroads.Utilities.Services.Interfaces;

namespace Crossroads.Utilities.Services
{
    /// <summary>
    /// A TypeConverter to expose system Environment variables to unity configuration.
    /// </summary>
    public class EnvironmentVariableTypeConverter : TypeConverter
    {
        private IConfigurationWrapper configurationWrapper;

        public EnvironmentVariableTypeConverter()
        {
            configurationWrapper = new ConfigurationWrapper();
        }

        /// <summary>
        /// Construct a new EnvironmentVariableTypeConverter with the given IConfigurationWrapper.
        /// This is intended primarily for unit testing, as a TypeConverter requires a 
        /// zero-arg constructor to be used by Unity.
        /// </summary>
        /// <param name="configurationWrapper"></param>
        public EnvironmentVariableTypeConverter(IConfigurationWrapper configurationWrapper)
        {
            this.configurationWrapper = configurationWrapper;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return (configurationWrapper.GetEnvironmentVarAsString((string)value));
        }
    }
}
