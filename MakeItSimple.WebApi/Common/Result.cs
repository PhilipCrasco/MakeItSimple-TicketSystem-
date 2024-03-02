namespace MakeItSimple.WebApi.Common
{
    public class Result
    {
        protected internal Result(bool isSuccess, bool isWarning, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException();
            }

            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException();
            }

            if (!isWarning && error == Error.None)
            {
                throw new InvalidOperationException();
            }

            IsSuccess = isSuccess;
            Error = error;
            IsWarning = isSuccess;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess || !IsWarning;
        public bool IsWarning { get; }

        public Error Error { get; }

        public static Result Success() => new(true , true, Error.None);
        public static Result Warning() => new(true, true, Error.None);

        public static Result<TValue> Success<TValue>(TValue value) => new(value,true , true, Error.None);
        public static Result<TValue> Warning<TValue>(TValue value) => new(value,true, true, Error.None);

        public static Result Failure(Error error) => new(false ,false, error);

        public static Result<TValue> Failure<TValue>(Error error) => new(default, false , false, error);

        public static Result Create(bool condition) => condition ? Success(): Failure(Error.ConditionNotMet);

        public static Result CreateWarning(bool condition) => condition ? Warning() : Failure(Error.ConditionNotMet);

        public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

        public static Result<TValue> CreateWarning<TValue>(TValue? value) => value is not null ? Warning(value) : Failure<TValue>(Error.NullValue);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, bool IsWarning, Error error)
            : base(isSuccess ,IsWarning,error) =>
            _value = value;

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }




    public record Error(string Code, string Message )
    {
        public static readonly Error None = new(string.Empty, string.Empty);

        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null." );

        public static readonly Error ConditionNotMet = new("Error.ConditionNotMet", "The specified condition was not met.");
    }


}
