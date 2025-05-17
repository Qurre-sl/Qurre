using System;
using System.Linq;
using JetBrains.Annotations;

namespace Qurre.API.Attributes;

[PublicAPI]
[AttributeUsage(AttributeTargets.Class)]
public class PluginInit : Attribute
{
    public PluginInit(string name, string developer = "", string version = "")
    {
        Name = name;
        Developer = developer;
        try
        {
            Version = new Version(version);
        }
        catch
        {
            int[] versions = version.Split('.').Select(TryParse).ToArray();

            Version = versions.Length switch
            {
                >= 4 => new Version(versions[0], versions[1], versions[2], versions[3]),
                3 => new Version(versions[0], versions[1], versions[2]),
                2 => new Version(versions[0], versions[1]),
                1 => new Version(versions[0], 0),
                _ => new Version(0, 0)
            };
        }

        return;

        static int TryParse(string str)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
    }

    public string Name { get; }
    public string Developer { get; }
    public Version Version { get; }
}