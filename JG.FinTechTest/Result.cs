using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JG.FinTechTest
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; private set; }
        public ErrorType ErrorType { get; private set; }

        protected Result(bool isSuccess, string error, ErrorType type)
        {
            if (isSuccess && error != string.Empty && type != ErrorType.None)
                throw new InvalidOperationException();
            if (!isSuccess && error == string.Empty && type == ErrorType.None)
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
            ErrorType = type;
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty, ErrorType.None);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty, ErrorType.None);
        }

        public static Result Fail(string error, ErrorType type)
        {
            return new Result(false, error, type);
        }

        public static Result<T> Fail<T>(T value, string error, ErrorType type)
        {
            return new Result<T>(value, false, error, type);
        }
    }

    public class Result<T> : Result
    {
        public readonly T _value;
        public T Value => _value;

        protected internal Result(T value, bool isSuccess, string error, ErrorType type) : base(isSuccess, error, type)
        {
            _value = value;
        }
    }

    public enum ErrorType
    {
        None,
        DonationAmountAboveMaximum,
        DonationAmountBelowMinimum,
        DonationNotFound,
        InvalidParameter,
        FailedToInsertToDatabase,
        InvalidPostCode,
    }
}
