using System;
using Newtonsoft.Json;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// Handles JSON serialization and deserialization
    /// Configured for Unity types and custom objects
    /// </summary>
    public class TypeConverter
    {
        private readonly JsonSerializerSettings settings;

        public TypeConverter()
        {
            settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        /// <summary>
        /// Convert JSON string to specified type
        /// </summary>
        public T ConvertFromJson<T>(string jsonString)
        {
            try
            {
                T result = JsonConvert.DeserializeObject<T>(jsonString, settings);
                Debug.Log($"[Converter] -- Converted to {typeof(T).Name}");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Converter] -- Failed to convert to {typeof(T).Name}: {e.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Convert JSON string to runtime type
        /// </summary>
        public object ConvertFromJson(string jsonString, Type type)
        {
            try
            {
                object result = JsonConvert.DeserializeObject(jsonString, type, settings);
                Debug.Log($"[Converter] -- Converted to {type.Name}");
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Converter] -- Failed to convert to {type.Name}: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Convert object to JSON string
        /// </summary>
        public string ConvertToJson(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Converter] -- Failed to serialize object: {e.Message}");
                return null;
            }
        }
    }
}