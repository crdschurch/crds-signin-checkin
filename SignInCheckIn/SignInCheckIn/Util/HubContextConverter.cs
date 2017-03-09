using System;
using System.ComponentModel;
using System.Configuration;
using System.Data.Common;
using Microsoft.AspNet.SignalR;
using SignInCheckIn.Hubs;

namespace SignInCheckIn.Util
{
    public class HubContextConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            return GlobalHost.ConnectionManager.GetHubContext<EventHub>();
        }
    }
}