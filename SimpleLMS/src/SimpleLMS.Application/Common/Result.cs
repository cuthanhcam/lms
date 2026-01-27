using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Common
{
    /// <summary>
    /// Class representing a result of an operation.
    /// Use the result pattern to avoid exceptions for control flow.
    /// </summary>
    /// <typeparam name="T">Type of the result value.</typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Data { get; }
        public string? ErrorMessage { get; }

        private Result(bool isSuccess, T? data, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
        }

        // Create a successful result
        public static Result<T> Success(T data) => new(true, data, null);

        // Create a failed result
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
    }

    /// <summary>
    /// Result class for operations that do not return a value.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        private Result(bool isSuccess, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        // Create a successful result
        public static Result Success() => new(true, null);

        // Create a failed result
        public static Result Failure(string errorMessage) => new(false, errorMessage);
    }
}
