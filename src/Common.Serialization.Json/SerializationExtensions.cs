namespace Common.Serialization.Json
{
    using System;

    using global::System;
    using global::System.IO;

    using Newtonsoft.Json;

    public static class SerializationExtensions
    {
        private static readonly JsonSerializerSettings DefaultSerializerSettings = new()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Populate,
            };

        private static readonly Lazy<JsonSerializer> JsonSerializer =
            LazyExtensions.InitLazy(() => new JsonSerializer());

        public static string ToJsonIndented(this object item, JsonSerializerSettings? settings = null) =>
            JsonConvert.SerializeObject(item, Formatting.Indented, settings ?? DefaultSerializerSettings);

        public static string ToJson(this object item, JsonSerializerSettings? settings = null) =>
            JsonConvert.SerializeObject(item, Formatting.None, settings ?? DefaultSerializerSettings);

        public static T? FromJsonTo<T>(this string json, JsonSerializerSettings? settings = null) =>
            JsonConvert.DeserializeObject<T>(json, settings ?? DefaultSerializerSettings);

        public static T? FromJsonTo<T>(this StreamReader json) => (T?)JsonSerializer.Value.Deserialize(json, typeof(T));
    }
}