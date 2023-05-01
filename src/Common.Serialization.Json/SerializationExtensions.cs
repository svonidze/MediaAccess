namespace Common.Serialization.Json
{
    using Newtonsoft.Json;

    public static class SerializationExtensions
    {
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Populate,
            };

        public static string ToJsonIndented(this object item, JsonSerializerSettings? settings = null) =>
            JsonConvert.SerializeObject(item, Formatting.Indented, settings ?? DefaultSerializerSettings);

        public static string ToJson(this object item, JsonSerializerSettings? settings = null) =>
            JsonConvert.SerializeObject(item, Formatting.None, settings ?? DefaultSerializerSettings);

        public static T? FromJsonTo<T>(this string json, JsonSerializerSettings? settings = null) =>
            JsonConvert.DeserializeObject<T>(json, settings ?? DefaultSerializerSettings);
    }
}