using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Editor.Utilities
{
    public static class Serializer
    {
        public static void ToFile<T>(T instance, string path)
        {
            try
            {
                var serializer = new DataContractSerializer(typeof(T));

                var settings = new XmlWriterSettings
                {
                    Indent = true,                 // 整形あり
                    IndentChars = "  ",            // スペース2つでインデント
                    NewLineOnAttributes = false,   // 属性は改行しない
                    Encoding = new UTF8Encoding(false) // UTF-8 (BOMなし)
                };

                using (var writer = XmlWriter.Create(path, settings))
                {
                    serializer.WriteObject(writer, instance);
                }
                //using var fs = new FileStream(path, FileMode.Create);
                //serializer.WriteObject(fs, instance);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to serialize {instance} to {path}: {ex.Message}");
                throw;
            }
        }

        internal static T FromFile<T>(string path)
        {
            try
            {
                using var fs = new FileStream(path, FileMode.Open);
                var serializer = new DataContractSerializer(typeof(T));
                T instance = (T)serializer.ReadObject(fs)!;
                return instance;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to deserialize from {path}: {ex.Message}");
                throw;
                return default(T)!;
            }
        }
    }
}
