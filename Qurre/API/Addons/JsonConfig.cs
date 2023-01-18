using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Qurre.API.Addons
{
    public class JsonConfig
    {
        private static JObject _cache;
        private static string _path;

        public JsonConfig(string name) => Name = name;

        public string Name { get; }

        public JToken JsonArray
        {
            get
            {
                JToken obj = _cache?[Name];

                if (obj is not null)
                {
                    return obj;
                }

                _cache![Name] = JObject.Parse("{ }");

                return _cache[Name];
            }
        }

        public static JsonConfig Register(string name) => new (name);

        public static void Save() => File.WriteAllText(_path, _cache.ToString());

        public static void Refresh() => Init();

        public JToken GetValue(string name, JToken source = null) => (source ?? JsonArray)[name];

        public JToken TrySafeGetTokenValue(string name, JToken value, string desc = null, JToken source = null)
        {
            try
            {
                return SafeGetTokenValue(name, value, desc, source);
            }
            catch (Exception e)
            {
                string assembly = Assembly.GetCallingAssembly().GetName().Name;
                var text = $"[JsonConfig] Occurred error in [SafeGetTokenValue]:\n{e}\n{e.StackTrace}";

                Extensions.TryAction(Log.LogTxt, text, text);

                e.PrintError(text, string.Empty);

                return value;
            }
        }

        public T TrySafeGetValue<T>(string name, T value, string desc = null, JToken source = null)
        {
            try
            {
                return SafeGetValue(name, value, desc, source);
            }
            catch (Exception error)
            {
                string assemblyName = Assembly.GetCallingAssembly().GetName().Name;
                var text = $"[ERROR] [JsonConfig] Occurred error in [SafeGetValue]:\n{error}\n{error.StackTrace}";

                Extensions.TryAction(Log.LogTxt, text, text);

                error.PrintError(text, string.Empty);

                return value;
            }
        }

        public JToken SafeGetTokenValue(string name, JToken value, string desc = null, JToken source = null)
        {
            JToken parent = source ?? JsonArray;
            JToken val = parent[name];

            if (val is not null)
            {
                return val;
            }

            if (desc is not null && desc.Trim() != string.Empty)
            {
                parent[name + "_desc"] = desc.Trim();
            }

            parent[name] = value;

            return value;
        }

        public T SafeGetValue<T>(string name, T value, string desc = null, JToken source = null)
        {
            JToken parent = source ?? JsonArray;
            JToken token = parent[name];

            if (token is not null)
            {
                return token.ToObject<T>();
            }

            if (desc is not null && desc.Trim() != string.Empty)
            {
                parent[name + "_desc"] = desc.Trim();
            }

            parent[name] = ConvertObject(value);

            return value;
        }

        internal static void Init()
        {
            _path = Path.Combine(Paths.Configs, $"{Server.Port}.json");

            if (!File.Exists(_path))
            {
                byte[] content = Encoding.UTF8.GetBytes("{\n    \n}");

                FileStream stream = File.Create(_path);

                stream.Write(content, 0, content.Length);

                stream.Close();
            }

            try
            {
                _cache = JObject.Parse(File.ReadAllText(_path));
            }
            catch
            {
                File.WriteAllText(_path, "{\n    \n}");

                _cache = JObject.Parse("{\n    \n}");
            }
        }

        private static JToken ConvertObject<T>(T obj)
            => obj switch
            {
                string stringValue => stringValue,
                bool boolValue => boolValue,
                long longValue => longValue,
                IEnumerable<object> enumerable => MergeArray(enumerable),
                float floatValue => floatValue,
                double doubleValue => doubleValue,
                decimal decimalValue => decimalValue,
                char charValue => charValue,
                byte byteValue => byteValue,
                short shortValue => shortValue,
                ushort ushortValue => ushortValue,
                sbyte sbyteValue => sbyteValue,
                ulong ulongValue => ulongValue,
                Version version => new JObject
                {
                    ["major"] = version.Major,
                    ["Build"] = version.Build,
                    ["Minor"] = version.Minor
                },
                Quaternion quaternion => new JObject
                {
                    ["w"] = quaternion.w,
                    ["x"] = quaternion.x,
                    ["y"] = quaternion.y,
                    ["z"] = quaternion.z
                },
                Vector3 vector3 => new JObject
                {
                    ["x"] = vector3.x,
                    ["y"] = vector3.y,
                    ["z"] = vector3.z
                },
                Vector2 vector2 => new JObject
                {
                    ["x"] = vector2.x,
                    ["y"] = vector2.y
                },
                _ => JObject.FromObject(obj)
            };

        private static JToken CreateFromContent(object content)
            => content switch
            {
                JToken token => token,
                Version version => new JObject
                {
                    ["major"] = version.Major,
                    ["Build"] = version.Build,
                    ["Minor"] = version.Minor
                },
                Quaternion quaternion => new JObject
                {
                    ["w"] = quaternion.w,
                    ["x"] = quaternion.x,
                    ["y"] = quaternion.y,
                    ["z"] = quaternion.z
                },
                Vector3 vector3 => new JObject
                {
                    ["x"] = vector3.x,
                    ["y"] = vector3.y,
                    ["z"] = vector3.z
                },
                Vector2 vector2 => new JObject
                {
                    ["x"] = vector2.x,
                    ["y"] = vector2.y
                },
                _ => new JValue(content)
            };

        private static JArray MergeArray(IEnumerable<object> arr)
        {
            JArray array = new ();

            var i = 0;

            foreach (object targetItem in arr)
            {
                if (i < array.Count)
                {
                    JToken sourceItem = array[i];

                    if (sourceItem is JContainer existingContainer)
                    {
                        existingContainer.Merge(targetItem);
                    }
                    else if (targetItem is not null)
                    {
                        JToken contentValue = CreateFromContent(targetItem);

                        if (contentValue.Type is not JTokenType.Null)
                        {
                            array[i] = contentValue;
                        }
                    }
                }
                else
                {
                    array.Add(targetItem);
                }

                i++;
            }

            return array;
        }
    }
}