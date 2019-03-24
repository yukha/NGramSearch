using System.Runtime.Serialization;

namespace LargeIndexPerformaceConsoleTest
{
    [DataContract]
    public class Actor
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }
}
