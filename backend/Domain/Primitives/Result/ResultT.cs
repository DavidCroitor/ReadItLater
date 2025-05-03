namespace Domain.Primitives.Result
{
    public class Result<T> : Result
    {
        public T? Value { get; protected set; }
        public static Result<T> Success(T value) => new Result<T> { IsSuccess = true, Value = value };
        public new static Result<T> Fail(string error) => new Result<T> { IsSuccess = false, Error = error };
    }
}