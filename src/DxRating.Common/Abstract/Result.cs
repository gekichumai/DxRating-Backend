namespace DxRating.Common.Abstract;

public readonly struct Result<TOk, TFail>
{
    private readonly TOk? _success;
    private readonly TFail? _error;

    private Result(TOk success)
    {
        _success = success;
    }

    private Result(TFail error)
    {
        _error = error;
    }

    public static Result<TOk, TFail> Ok(TOk success)
    {
        return new Result<TOk, TFail>(success);
    }

    public static Result<TOk, TFail> Fail(TFail error)
    {
        return new Result<TOk, TFail>(error);
    }

    public bool IsOk => _success != null;
    public bool IsFail => _error != null;

    public TOk GetOk()
    {
        if (!IsOk)
        {
            throw new InvalidOperationException("Result is not Ok");
        }

        return _success!;
    }

    public TFail GetFail()
    {
        if (!IsFail)
        {
            throw new InvalidOperationException("Result is not Fail");
        }

        return _error!;
    }

    public static implicit operator Result<TOk, TFail>(TOk success) => Ok(success);
    public static implicit operator Result<TOk, TFail>(TFail error) => Fail(error);

    public void Map(Action<TOk> okAction, Action<TFail> failAction)
    {
        if (IsOk)
        {
            okAction(_success!);
        }
        else
        {
            failAction(_error!);
        }
    }

    public TResult Map<TResult>(Func<TOk, TResult> okFunc, Func<TFail, TResult> failFunc)
    {
        return IsOk ? okFunc(_success!) : failFunc(_error!);
    }
}
