using System;

namespace MinistryPlatform.Translation.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MpRestApiTable : Attribute
    {
        public string Name { get; set; }
    }
}
