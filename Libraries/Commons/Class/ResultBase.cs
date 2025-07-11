using System.Diagnostics;

namespace Common.Library.Results
{
    public class ResultBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerResultBase{T}"/> class.
        /// </summary>
        /// <param name="statusCode">Status of the action.</param>
        protected ResultBase(T statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerResultBase{T}"/> class.
        /// </summary>
        /// <param name="statusCode">Status of the action.</param>
        /// <param name="exception">Exception which occured during the execution of the action.</param>
        protected ResultBase(
            T statusCode,
            Exception? exception)
            : this(statusCode)
        {
            Exception = exception;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerResultBase{T}"/> class.
        /// </summary>
        /// <param name="statusCode">Status of the action.</param>
        /// <param name="exception">Exception which occured during the execution of the action.</param>
        /// <param name="stackTrace">Stack trace of all called method when the exception occurs.</param>
        protected ResultBase(
            T statusCode,
            Exception? exception,
            StackTrace? stackTrace)
            : this(statusCode, exception)
        {
            StackTrace = stackTrace;
        }

        /// <summary>
        /// Gets the <see cref="System.Diagnostics.StackTrace"/> when the <see cref="Exception"/> occurs.
        /// </summary>
        public StackTrace? StackTrace { get;  set; }

        /// <summary>
        /// Gets the exception with occured during the execution of the action.
        /// </summary>
        public Exception? Exception { get;  set; }

        /// <summary>
        /// Gets the status code of the execution.
        /// </summary>
        public T StatusCode { get;  set; }
    }
}