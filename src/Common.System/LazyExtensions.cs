namespace Common.System
{
    using global::System;

    public static class LazyExtensions
    {
        public static Lazy<T> InitLazy<T>(this Func<T> valueFactory) => new(valueFactory);

        public static DisposableLazy<T> NewDisposableLazy<T>(
            this Func<T> valueFactory,
            Action<T>? beforeDisposing = null)
            where T : IDisposable =>
            new(valueFactory, beforeDisposing);
    }
}