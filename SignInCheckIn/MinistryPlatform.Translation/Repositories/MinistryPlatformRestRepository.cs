using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinistryPlatform.Translation.Extensions;
using MinistryPlatform.Translation.Models.Attributes;
using MinistryPlatform.Translation.Repositories.Interfaces;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;

namespace MinistryPlatform.Translation.Repositories
{
    public class MinistryPlatformRestRepository : IMinistryPlatformRestRepository
    {
        private readonly IRestClient _ministryPlatformRestClient;
        private readonly ThreadLocal<string> _authToken = new ThreadLocal<string>();

        public MinistryPlatformRestRepository(IRestClient ministryPlatformRestClient)
        {
            _ministryPlatformRestClient = ministryPlatformRestClient;
        }

        public IMinistryPlatformRestRepository UsingAuthenticationToken(string authToken)
        {
            _authToken.Value = authToken;
            return this;
        }

        public T Get<T>(int recordId, List<string> selectColumns)
        {
            return Get<T>(recordId, string.Join(",", selectColumns.ToArray()));
        }

        public T Get<T>(int recordId, string selectColumns = null)
        {
            var url = AddGetColumnSelection(string.Format("/tables/{0}/{1}", GetTableName<T>(), recordId), selectColumns);
            var request = new RestRequest(url, Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Accept", "application/json");
            AddAuthorization(request);

            var response = _ministryPlatformRestClient.Execute(request);
            _authToken.Value = null;
            response.CheckForErrors(string.Format("Error getting {0} by ID {1}", GetTableName<T>(), recordId), true);

            var content = JsonConvert.DeserializeObject<List<T>>(response.Content);
            if (content == null || !content.Any())
            {
                return default(T);
            }

            return content.FirstOrDefault();
        }

        public T Get<T>(string tableName, int recordId, string columnName)
        {
            var url = AddGetColumnSelection(string.Format("/tables/{0}/{1}", tableName, recordId), columnName);
            var request = new RestRequest(url, Method.GET);
            AddAuthorization(request);

            var response = _ministryPlatformRestClient.Execute(request);
            _authToken.Value = null;
            response.CheckForErrors(string.Format("Error getting {0} by ID {1}", tableName, recordId), true);

            var content = JsonConvert.DeserializeObject<List<T>>(response.Content);
            if (content == null || !content.Any())
            {
                return default(T);
            }

            return content.FirstOrDefault();
        }

        public T Create<T>(T objectToCreate, List<string> selectColumns)
        {
            return Create(objectToCreate, string.Join(",", selectColumns.ToArray()));
        }

        public T Create<T>(T objectToCreate, string selectColumns = null)
        {
            return ExecutePutOrPost(objectToCreate, Method.POST, selectColumns);
        }

        public T Update<T>(T objectToUpdate, List<string> selectColumns)
        {
            return Update(objectToUpdate, string.Join(",", selectColumns.ToArray()));
        }

        public T Update<T>(T objectToUpdate, string selectColumns = null)
        {
            return ExecutePutOrPost(objectToUpdate, Method.PUT, selectColumns);
        }

        private T ExecutePutOrPost<T>(T record, Method method, string selectColumns)
        {
            var tableName = GetTableName<T>();
            var url = AddGetColumnSelection($"/tables/{tableName}", selectColumns);
            var request = new RestRequest(url, method).SetJsonBody(record);

            AddAuthorization(request);

            var response = _ministryPlatformRestClient.Execute(request);
            _authToken.Value = null;
            response.CheckForErrors($"Error {(method == Method.PUT ? "updating existing" : "creating new")} {tableName}");

            var result = JsonConvert.DeserializeObject<List<T>>(response.Content);
            if (result == null || !result.Any())
            {
                return default(T);
            }
            return result.First();
        }

        public List<List<T>> GetFromStoredProc<T>(string procedureName)
        {
            return GetFromStoredProc<T>(procedureName, new Dictionary<string, object>());
        }

        public List<List<T>> GetFromStoredProc<T>(string procedureName, Dictionary<string, object> parameters)
        {
            var url = string.Format("/procs/{0}/{1}", procedureName, FormatStoredProcParameters(parameters));
            var request = new RestRequest(url, Method.GET);
            AddAuthorization(request);

            var response = _ministryPlatformRestClient.ExecuteAsGet(request, "GET");
            _authToken.Value = null;
            response.CheckForErrors(string.Format("Error executing procedure {0}", procedureName), true);

            var content = JsonConvert.DeserializeObject<List<List<T>>>(response.Content);
            if (content == null || !content.Any())
            {
                return default(List<List<T>>);
            }
            return content;
        }

        public int PostStoredProc(string procedureName, Dictionary<string, object> parameters)
        {
            var url = string.Format("/procs/{0}", procedureName);
            var request = new RestRequest(url, Method.POST);
            AddAuthorization(request);

            request.AddParameter("application/json", FormatStoredProcBody(parameters), ParameterType.RequestBody);
            var response = _ministryPlatformRestClient.Execute(request);
            _authToken.Value = null;
            response.CheckForErrors(string.Format("Error executing procedure {0}", procedureName), true);

            return (int)response.StatusCode;
        }

        private static string FormatStoredProcBody(Dictionary<string, object> parameters)
        {
            var parmlist = new List<string>();
            foreach (var item in parameters)
            {
                var parm = "\"" + item.Key + "\":\"" + item.Value + "\"";
                parmlist.Add(parm);
            }

            return "{" + string.Join(",", parmlist) + "}";
        }

        private static string FormatStoredProcParameters(Dictionary<string, object> parameters)
        {
            var result = parameters.Aggregate("?", (current, parameter) => current + ((parameter.Key.StartsWith("@") ? parameter.Key : "@" + parameter.Key) + "=" + parameter.Value + "&"));
            return result.TrimEnd('&');
        }

        public List<T> SearchTable<T>(string tableName, string searchString = null, string selectColumns = null)
        {
            var search = string.IsNullOrWhiteSpace(searchString) ? string.Empty : string.Format("?$filter={0}", searchString);

            var url = AddColumnSelection(string.Format("/tables/{0}{1}", tableName, search), selectColumns);
            var request = new RestRequest(url, Method.GET);
            AddAuthorization(request);

            var response = _ministryPlatformRestClient.Execute(request);
            _authToken.Value = null;
            response.CheckForErrors(string.Format("Error searching {0}", GetTableName<T>()));

            var content = JsonConvert.DeserializeObject<List<T>>(response.Content);

            return content;
        }

        public List<T> SearchTable<T>(string tableName, string searchString, List<string> selectColumns)
        {
            return SearchTable<T>(tableName, searchString, selectColumns == null ? null : string.Join(",", selectColumns));
        }

        public List<T> Search<T>(string searchString = null, string selectColumns = null)
        {
            return SearchTable<T>(GetTableName<T>(), searchString, selectColumns);
        }

        public List<T> Search<T>(string searchString, List<string> columns)
        {
            return SearchTable<T>(GetTableName<T>(), searchString, columns);
        }

        public void UpdateRecord(string tableName, int recordId, Dictionary<string, object> fields)
        {
            var url = string.Format("/tables/{0}", tableName);
            var request = new RestRequest(url, Method.PUT);
            AddAuthorization(request);
            request.AddParameter("application/json", "[" + FormatStoredProcBody(fields) + "]", ParameterType.RequestBody);

            var response = _ministryPlatformRestClient.Execute(request);
            response.CheckForErrors(string.Format("Error updating {0}", tableName), true);
        }

        private void AddAuthorization(IRestRequest request)
        {
            if (_authToken.IsValueCreated)
            {
                request.AddHeader("Authorization", string.Format("Bearer {0}", _authToken.Value));
            }
        }

        private static string GetTableName<T>()
        {
            var table = typeof(T).GetAttribute<MpRestApiTable>();
            if (table == null)
            {
                throw new NoTableDefinitionException(typeof(T));
            }

            return table.Name;
        }

        private static string AddColumnSelection(string url, string selectColumns)
        {
            return string.IsNullOrWhiteSpace(selectColumns) ? url : string.Format("{0}&$select={1}", url, selectColumns);
        }

        private static string AddGetColumnSelection(string url, string selectColumns)
        {
            return string.IsNullOrWhiteSpace(selectColumns) ? url : string.Format("{0}?$select={1}", url, selectColumns);
        }
    }

    public class NoTableDefinitionException : Exception
    {
        public NoTableDefinitionException(Type t) : base(string.Format("No RestApiTable attribute specified on type {0}", t)) { }
    }
}