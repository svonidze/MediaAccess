using YamlDotNet.Serialization;

namespace Common.Serialization.Yaml
{
    public static class SerializationExtensions
    {
        private static readonly Deserializer Deserializer = new Deserializer();
        private static readonly Serializer Serializer  = new Serializer();

        public static string ToYaml(this object item)
        {
            return Serializer.Serialize(item);
        }

        public static T FromYamlTo<T>(this string yaml)
        {
            return Deserializer.Deserialize<T>(yaml);
        }
    }
}