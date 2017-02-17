﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
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

        /// <summary>
        /// This is a hack to set a Json body on a request, making sure the object is serialized properly according to Json attributes.  RestSharp's JSON serializer does not pay attention to attributes when serializing using request.AddJsonBody(), so objects do not get sent appropriately. Note that this will clear all previously set parameters on the request, so use with caution.
        /// </summary>
        /// <param name="request">The IRestRequest to set body on</param>
        /// <param name="record">The object to serialize onto the request body - it will be wrapped in an array if it is not already a collection of some sort.</param>
        /// <returns>The IRestRequest, in case you want to chain method calls</returns>
        public static IRestRequest SetJsonArrayBody(this IRestRequest request, object record)
        {
            // Wrap the record in an array, if it is not already some sort of collection.
            var body = record.GetType().GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ICollection<>)) ? record : new List<object> {record};

            return request.SetJsonBody(body);
        }

        /// <summary>
        /// This is a hack to set a Json body on a request, making sure the object is serialized properly according to Json attributes.  RestSharp's JSON serializer does not pay attention to attributes when serializing using request.AddJsonBody(), so objects do not get sent appropriately. Note that this will clear all previously set parameters on the request, so use with caution.
        /// </summary>
        /// <param name="request">The IRestRequest to set body on</param>
        /// <param name="record">The object to serialize onto the request body.</param>
        /// <returns>The IRestRequest, in case you want to chain method calls</returns>
        public static IRestRequest SetJsonBody(this IRestRequest request, object record)
        {
            // This nonsense is needed because request.setJsonBody() does not honor Json name
            // attributes on the object, so proper names are not sent to MP. If the input
            // record is already a collection of some sort, just serialize it onto the body,
            // otherwise create a new collection containing the single record.
            var jsonBody = JsonConvert.SerializeObject(record);
            request.Parameters.Clear();
            request.AddHeader("Accept", "application/json");
            request.AddParameter("application/json", jsonBody, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            return request;
        }


        public static IRestRequest AddQueryParameterIfSpecified(this IRestRequest request, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return request;
            }
            request.AddQueryParameter(name, value);
            return request;
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