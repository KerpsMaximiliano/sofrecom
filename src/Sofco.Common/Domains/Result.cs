using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Common.Domains
{
    public class Result
    {
        public readonly List<ResultError> Errors;

        public object ResultData { get; set; }

        public Result(object data)
            : this()
        {
            ResultData = data;
        }

        public Result()
        {
            Errors = new List<ResultError>();
        }

        public virtual bool HasErrors => Errors.Any();

        public void AddError(string message)
        {
            Errors.Add(new ResultError(message));
        }

        public void AddError(string message, string errorCode)
        {
            Errors.Add(new ResultError(errorCode, message, new List<string>()));
        }

        public void AddError(string message, string errorCode, string memberName)
        {
            Errors.Add(new ResultError(errorCode, message, new List<string> { memberName }));
        }

        public void AddError(ResultError validationResult)
        {
            Errors.Add(validationResult);
        }

        public void AddErrorRange(IEnumerable<ResultError> validationResults)
        {
            Errors.AddRange(validationResults);
        }

        public void AddErrorRange(IEnumerable<string> errorMessages)
        {
            foreach (var error in errorMessages)
            {
                Errors.Add(new ResultError(error));
            }
        }

        public Result AddErrorFluent(string message)
        {
            Errors.Add(new ResultError(message));

            return this;
        }
    }

    public sealed class Result<T> : Result
    {
        private T data;

        public Result()
        {
        }

        public Result(T data)
        {
            Data = data;
        }

        public T Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
                ResultData = data;
            }
        }

        public Result<TTarget> ToResult<TTarget>(Func<T, TTarget> converter = null)
        {
            if (HasErrors)
            {
                var errorsResult = new Result<TTarget>();

                errorsResult.AddErrorRange(Errors);

                return errorsResult;
            }

            if (converter == null) return new Result<TTarget>();

            var translated = converter(Data);

            return new Result<TTarget>(translated);
        }

        public Result<T> AddErrors(Result result)
        {
            Errors.AddRange(result.Errors);

            return this;
        }

        public new Result<T> AddErrorFluent(string message)
        {
            Errors.Add(new ResultError(message));

            return this;
        }
    }
}