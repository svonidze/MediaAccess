namespace Common.Reflection
{
    using System.Reflection;

    public static class CustomAttributeProviderExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(
            this ICustomAttributeProvider provider,
            bool inherit = false) =>
            provider.GetCustomAttributes(typeof(T), inherit).Cast<T>();
    }
}