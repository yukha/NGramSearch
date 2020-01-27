using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using NGramSearch;

namespace DemoWeb.Services
{
    public abstract class JsonFileSource<TIndexType> where TIndexType : NGramIndex<int>, new()
    {
        private readonly object IndexLock = new object();
        private NGramIndex<int> _initializedIndex;
        private Dictionary<int, string> _initializedItems;


        protected (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) GetIndex(string indexName)
        {
            lock (IndexLock)
            {
                if (_initializedIndex == null)
                {
                    (_initializedIndex, _initializedItems) = ReloadIndex(indexName);

                }
                return (_initializedIndex, _initializedItems);
            }
        }

        private (NGramSearch.NGramIndex<int> index, Dictionary<int, string> items) ReloadIndex(string indexName)
        {
            List<JsonDataItem> list;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<JsonDataItem>));

            using (FileStream inputStream = new FileStream($"Data\\{indexName}.json", FileMode.Open, FileAccess.Read))
            {
                list = serializer.ReadObject(inputStream) as List<JsonDataItem>;
            }

            var index = new TIndexType(); // use defaults: trigram and SimpleNormalizer
            var items = new Dictionary<int, string>();

            foreach (var item in list)
            {
                index.Add(item.Id, item.Name);
                items[item.Id] = item.Name;
            }
            return (index, items);
        }


        [DataContract]
        private class JsonDataItem
        {
            [DataMember(Name = "id")]
            public int Id { get; set; }

            [DataMember(Name = "name")]
            public string Name { get; set; }
        }
    }
}
