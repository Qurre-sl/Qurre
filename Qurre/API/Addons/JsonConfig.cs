using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Qurre.API.Addons;

[PublicAPI]
public class JsonConfig(string name)
{
    internal static JObject Cache = JObject.Parse("{}");
    internal static string ConfigPath = string.Empty;

    public string Name { get; } = name;

    public JToken JsonArray
    {
        get
        {
            try
            {
                JToken? obj = Cache[Name];

                if (obj is null)
                    throw new NullReferenceException(nameof(obj));

                return obj;
            }
            catch
            {
                JObject parsed = JObject.Parse("{ }");
                Cache[Name] = parsed;

                return parsed;
            }
        }
    }

    public JToken? GetValue(string name, JToken? source = null)
    {
        return (source ?? JsonArray)[name];
    }

    public JToken SafeGetTokenValue(string name, JToken value, string desc = "", JToken? source = null)
    {
        try
        {
            JToken par = source ?? JsonArray;
            JToken? val = par[name];

            if (val is not null)
                return val;

            if (desc.Trim() != string.Empty)
                par[name + "_desc"] = desc.Trim();

            par[name] = value;
            return value;
        }
        catch (Exception e)
        {
            string assembly = Assembly.GetCallingAssembly().GetName().Name;
            string text =
                $"[ERROR] [{assembly} => JsonConfig] Occurred error in [SafeGetTokenValue]:\n{e}\n{e.StackTrace}";
            ServerConsole.AddLog(text, ConsoleColor.Red);
            try
            {
                Log.LogTxt(text);
            }
            catch
            {
                // ignored
            }

            return value;
        }
    }

    public T SafeGetValue<T>(string name, T value, string desc = "", JToken? source = null)
    {
        try
        {
            JToken par = source ?? JsonArray;
            JToken? val = par[name];

            if (val is not null)
            {
                T? retVal = val.ToObject<T>();
                if (retVal is not null)
                    return retVal;
            }

            if (desc.Trim() != string.Empty)
                par[name + "_desc"] = desc.Trim();

            par[name] = ConvertObject(value);
            return value;
        }
        catch (Exception e)
        {
            string assembly = Assembly.GetCallingAssembly().GetName().Name;
            string text = $"[ERROR] [{assembly} => JsonConfig] Occurred error in [SafeGetValue]:\n{e}\n{e.StackTrace}";
            ServerConsole.AddLog(text, ConsoleColor.Red);
            try
            {
                Log.LogTxt(text);
            }
            catch
            {
                // ignored
            }

            return value;
        }
    }

    public static JsonConfig Register(string name)
    {
        return new JsonConfig(name);
    }

    public static void UpdateFile()
    {
        File.WriteAllText(ConfigPath, Cache.ToString());
    }

    public static void RefreshConfig()
    {
        Init();
    }

    internal static void Init()
    {
        if (!Directory.Exists(Paths.Configs))
            Directory.CreateDirectory(Paths.Configs);

        ConfigPath = Path.Combine(Paths.Configs, $"{Server.Port}.json");
        if (!File.Exists(ConfigPath))
        {
            byte[] content = new UTF8Encoding(true).GetBytes("{\n    \n}");
            FileStream stream = File.Create(ConfigPath);
            stream.Write(content, 0, content.Length);
            stream.Close();
        }

        string fileContent = File.ReadAllText(ConfigPath);

        try
        {
            Cache = JObject.Parse(fileContent);
        }
        catch (Exception ex)
        {
            ServerConsole.AddLog(ex.Message, ConsoleColor.Red);
            File.WriteAllText(ConfigPath + ".bak", fileContent);

            File.WriteAllText(ConfigPath, "{\n    \n}");
            Cache = JObject.Parse("{\n    \n}");
        }
    }

    private JToken ConvertObject<T>(T obj)
    {
        switch (obj)
        {
            case string str:
                return str;
            case bool bl:
                return bl;
            case IEnumerable<object> list:
                {
                    JArray jt = [];
                    MergeArray(jt, list);
                    return jt;
                }
            case Vector3 vec:
                return new JObject
                {
                    ["x"] = vec.x,
                    ["y"] = vec.y,
                    ["z"] = vec.z
                };
        }

        if (obj is null)
            return default!;

        try
        {
            long numb = long.Parse(obj.ToString());
            return numb;
        }
        catch
        {
            try
            {
                float numb = float.Parse(obj.ToString());
                if (!float.IsNaN(numb)) return numb;
            }
            catch
            {
                // ignored
            }
        }

        return JObject.FromObject(obj);

        static void MergeArray(JArray jt, IEnumerable<object> arr)
        {
            int i = 0;
            foreach (object targetItem in arr)
            {
                if (i < jt.Count)
                {
                    JToken sourceItem = jt[i];

                    if (sourceItem is JContainer existingContainer)
                    {
                        existingContainer.Merge(targetItem);
                    }
                    else if (targetItem is not null)
                    {
                        JToken contentValue = CreateFromContent(targetItem);
                        if (contentValue.Type is not JTokenType.Null)
                            jt[i] = contentValue;
                    }
                }
                else
                {
                    try
                    {
                        jt.Add(targetItem);
                    }
                    catch
                    {
                        jt.Add(JObject.FromObject(targetItem));
                    }
                }

                i++;
            }
        }

        static JToken CreateFromContent(object content)
        {
            return content switch
            {
                JToken token => token,
                Vector3 vec => new JObject { ["x"] = vec.x, ["y"] = vec.y, ["z"] = vec.z },
                _ => new JValue(content)
            };
        } // end void
    } // end void
}