namespace Common.Monads;

public class Either<TLeft, TRight>
{
    public bool IsLeft { get; }

    public TLeft? Left { get; }

    public TRight? Right { get; }

    public Either(TLeft left)
    {
        this.Left = left;
        this.IsLeft = true;
    }

    public Either(TRight right)
    {
        this.Right = right;
        this.IsLeft = false;
    }

    public T Match<T>(Func<TLeft, T> leftFunc, Func<TRight, T> rightFunc) =>
        this.IsLeft
            ? leftFunc(this.Left)
            : rightFunc(this.Right);

    public static implicit operator Either<TLeft, TRight>(TLeft left) => new(left);

    public static implicit operator Either<TLeft, TRight>(TRight right) => new(right);
}