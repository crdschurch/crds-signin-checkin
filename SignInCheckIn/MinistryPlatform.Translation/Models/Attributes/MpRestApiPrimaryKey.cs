using System;

namespace MinistryPlatform.Translation.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MpRestApiPrimaryKey : Attribute
    {
        public string Name { get; set; }

        public MpRestApiPrimaryKey(string name)
        {
            Name = name;
        }
    }
}
