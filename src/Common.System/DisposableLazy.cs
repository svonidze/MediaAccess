namespace Common.System;

using global::System;

public class DisposableLazy<T> : Lazy<T>, IDisposable
    where T : IDisposable
{
    private readonly Action<T>? beforeDisposing;

    public DisposableLazy(Func<T> valueFactory, Action<T>? beforeDisposing = null) : base(valueFactory)
    {
        this.beforeDisposing = beforeDisposing;
    }

    public void Dispose()
    {
        if (!this.IsValueCreated)
        {
            return;
        }

        this.beforeDisposing?.Invoke(this.Value);
        this.Value.Dispose();
    }
}