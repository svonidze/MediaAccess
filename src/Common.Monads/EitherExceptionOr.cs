namespace Common.Monads;

public class EitherExceptionOr<TRight> : Either<Exception, TRight>
{
    public EitherExceptionOr(Exception left) : base(left)
    {
    }

    public EitherExceptionOr(TRight right) : base(right)
    {
    }
}