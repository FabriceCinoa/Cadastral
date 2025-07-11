/* 
 * Project: Iristia.Core
 * FileName: StringNullOrEmptyException.cs
 * Author: JEREMY JANISZEWSKI
 * Creation Date : 2023-3-20
 * Last Modified Date : 2023-4-11
 * Last Modified By: JEREMY JANISZEWSKI
 * @ 2023 - Lexico - All rights reserved
*/

using System.Runtime.Serialization;

namespace Common.Library
{
    /// <summary>
    /// Represents the exception which occurs when a <see cref="string"/> is
    ///  <see langword="null" /> or <see cref="string.Empty"/>.
    /// </summary>
    public sealed class StringNullOrEmptyException
        : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class.
        /// </summary>
        public StringNullOrEmptyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class with
        ///  the specified error message.
        /// </summary>
        /// <param name="paramName">The name of the parameter that is the cause of the exception.</param>
        public StringNullOrEmptyException(string? paramName)
            : base($"The {paramName} cannot be null or empty.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or the destination.</param>
        public StringNullOrEmptyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class with
        ///  the specified error message and a reference to the inner exception that is the cause
        ///  of this exception.
        /// </summary>
        /// <param name="message">The message that explain the reason of the exception.</param>
        /// <param name="innerException">The inner exception that is the cause of the exception.</param>
        public StringNullOrEmptyException(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class with
        ///  the specified error message and the name of the parameter that is the cause
        ///  of this exception.
        /// </summary>
        /// <param name="message">The message that explain the reason of the exception.</param>
        /// <param name="paramName">The name of the parameter that is the cause of the exception.</param>
        public StringNullOrEmptyException(string? message, string? paramName)
            : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class with
        ///  the specified error message, the name of the parameter and the exception that is the cause
        ///  of this exception.
        /// </summary>
        /// <param name="message">The message that explain the reason of the exception.</param>
        /// <param name="paramName">The name of the parameter that is the cause of the exception.</param>
        /// <param name="innerException">The inner exception that is the cause of the exception.</param>
        public StringNullOrEmptyException(string? message, string? paramName, Exception? innerException)
            : base(message, paramName, innerException)
        {
        }
    }
}
