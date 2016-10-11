using System;
using System.Net;
using RestSharp;

namespace MinistryPlatform.Translation.Extensions
{
    public static class RestSharpExtensions
    {
        /// <summary>
        /// Determine if this response is an error.  It looks at the ResponseStatus and the HTTP StatusCode to make the determination.
        /// </summary>
        /// <param name="response">The response to check</param>
        /// <param name="errorNotFound">Indicates if a 404 should be considered an error</param>
        /// <returns></returns>
        public static bool IsError(this IRestResponse response, bool errorNotFound = false)
        {
            // If the request is not completed, this is an error
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                return true;
            }

            // If we got a 404, and we're considering that an error, then it's an error
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return errorNotFound;
            }

            // If we have a bad response code, then it's an error
            return (int)response.StatusCode < 200 || (int)response.StatusCode > 399;
        }

        /// <summary>
        /// Checks for error response, and throws a RestResponseException if the response is in error.
        /// </summary>
        /// <param name="response">The response to check</param>
        /// <param name="errorMessage">The error message to include in the exception, if error</param>
        /// <param name="errorNotFound">Indicates if a 404 should be considered an error</param>
        public static void CheckForErrors(this IRestResponse response, string errorMessage, bool errorNotFound = false)
        {
            if (!IsError(response, errorNotFound))
            {
                return;
            }

            throw new RestResponseException(errorMessage, response);
        }
    }

    public class RestResponseException : Exception
    {
        public IRestResponse Response { get; private set; }

        public RestResponseException(string message, IRestResponse response) : base(string.Format("{0} - Status: {1}, Status Code: {2}, Error: {3}, Content: {4}", message, response.ResponseStatus, response.StatusCode, response.ErrorMessage, response.Content), response.ErrorException)
        {
            Response = response;
        }
    }
}
