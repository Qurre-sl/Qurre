using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Qurre.API.Addons
{
    public class JsonConfig
    {
        public string Name { get; }
        public JsonConfig(string name)
        {
            Name = name;
        }

        public JToken JsonArray
        {
            get
            {
                try
                {
                    var obj = Cache[Name];
                    if (obj is not null) return obj;
                }
                catch { }
                Cache[Name] = JObject.Parse("{ }");
                return Cache[Name];
            }
        }

        public JToken GetValue(string name, JToken source = null) => (source ?? JsonArray)[name];
        public JToken SafeGetTokenValue(string name, JToken value, string desc = null, JToken source = null)
        {
            try
            {
                var par = source ?? JsonArray;
                var val = par[name];
                if (val is not null) return val;
                if (desc is not null && desc.Trim() != string.Empty) par[name + "_desc"] = desc.Trim();
                par[name] = value;
                return value;
            }
            catch (Exception e)
            {
                string assembly = Assembly.GetCallingAssembly().GetName().Name;
                string text = $"[ERROR] [{assembly} => JsonConfig] Occurred error in [SafeGetTokenValue]:\n{e}\n{e.StackTrace}";
                ServerConsole.AddLog(text, ConsoleColor.Red);
                try { Log.LogTxt(text); } catch { }
                return value;
            }
        }
        public T SafeGetValue<T>(string name, T value, string desc = null, JToken source = null)
        {
            try
            {
                var par = source ?? JsonArray;
                var val = par[name];
                if (val is not null) return val.ToObject<T>();
                if (desc is not null && desc.Trim() != string.Empty) par[name + "_desc"] = desc.Trim();
                par[name] = ConvertObject(value);
                return value;
            }
            catch (Exception e)
            {
                string assembly = Assembly.GetCallingAssembly().GetName().Name;
                string text = $"[ERROR] [{assembly} => JsonConfig] Occurred error in [SafeGetValue]:\n{e}\n{e.StackTrace}";
                ServerConsole.AddLog(text, ConsoleColor.Red);
                try { Log.LogTxt(text); } catch { }
                return value;
            }
        }

        static public JsonConfig Register(string name) => new(name);
        static public void UpdateFile() => File.WriteAllText(ConfigPath, Cache.ToString());
        static public void RefreshConfig() => Init();

        static internal JObject Cache;
        static internal string ConfigPath = string.Empty;
        static internal void Init()
        {
            if (!Directory.Exists(Pathes.Configs))
                Directory.CreateDirectory(Pathes.Configs);

            ConfigPath = Path.Combine(Pathes.Configs, $"{Server.Port}.json");
            if (!File.Exists(ConfigPath))
            {
                byte[] content = new UTF8Encoding(true).GetBytes("{\n    \n}");
                var stream = File.Create(ConfigPath);
                stream.Write(content, 0, content.Length);
                stream.Close();
            }
            try
            {
                Cache = JObject.Parse(File.ReadAllText(ConfigPath));
            }
            catch
            {
                File.WriteAllText(ConfigPath, "{\n    \n}");
                Cache = JObject.Parse("{\n    \n}");
            }
        }

        JToken ConvertObject<T>(T obj)
        {
            if (obj is string str) return str;
            if (obj is bool bl) return bl;
            if (obj is IEnumerable<object> list)
            {
                JArray jt = new();
                MergeArray(jt, list);
                return jt;
            }
            if (obj is UnityEngine.Vector3 vec)
                return new JObject()
                {
                    ["x"] = vec.x,
                    ["y"] = vec.y,
                    ["z"] = vec.z
                };
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
                catch { }
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
                    else jt.Add(targetItem);

                    i++;
                }
            }
            static JToken CreateFromContent(object content)
            {
                if (content is JToken token) return token;
                if (content is UnityEngine.Vector3 vec)
                    return new JObject()
                    {
                        ["x"] = vec.x,
                        ["y"] = vec.y,
                        ["z"] = vec.z
                    };
                return new JValue(content);
            }
        }
    }
}