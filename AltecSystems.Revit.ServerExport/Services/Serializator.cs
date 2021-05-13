using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace AltecSystems.Revit.ServerExport.Services
{
    public static class Serializator
    {
        private static BinaryFormatter _bin = new BinaryFormatter();

        public static void Serialize(string pathOrFileName, object objToSerialise)
        {
            using (Stream stream = File.Open(pathOrFileName, FileMode.Create))
            {
                try
                {
                    _bin.Serialize(stream, objToSerialise);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to serialize. Reason: " + e.Message);
                    throw;
                }
            }
        }

        public static T Deserialize<T>(string pathOrFileName)
        {
            T items;

            using (Stream stream = File.Open(pathOrFileName, FileMode.Open))
            {
                try
                {
                    items = (T)_bin.Deserialize(stream);
                }
                catch (SerializationException e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    throw;
                }
            }

            return items;
        }
    }
}