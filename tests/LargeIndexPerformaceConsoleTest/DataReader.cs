using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace LargeIndexPerformaceConsoleTest
{
    public class DataReader
    {
        public static IList<Actor> ReadFromFile(string filePath)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Actor>));

            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return serializer.ReadObject(inputStream) as List<Actor>;
            }
        }
    }
}
