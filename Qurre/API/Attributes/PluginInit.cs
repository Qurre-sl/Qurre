using System;

namespace Qurre.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginInit : Attribute
    {
        public PluginInit(string name, string developer = "", string version = "")
        {
            Name = name;
            Developer = developer;
            Version = new (version);
        }

        public string Name { get; }
        public string Developer { get; }
        public Version Version { get; }
    }
}