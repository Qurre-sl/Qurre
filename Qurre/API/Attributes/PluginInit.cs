using System;
namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginInit : Attribute
    {
        public string Name { get; }
        public string Developer { get; }
        public Version Version { get; }

        public PluginInit(string name, string developer = "", Version version = default)
        {
            Name = name;
            Developer = developer;
            Version = version;
        }
    }
}