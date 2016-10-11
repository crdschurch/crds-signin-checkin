using System;

namespace MinistryPlatform.Translation.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MpRestApiTable : System.Attribute
    {
        public string Name { get; set; }
    }
}
