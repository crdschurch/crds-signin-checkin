using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace SignInCheckIn.Util
{
    public class HttpAuthResult : IHttpActionResult
    {
        private readonly string _token;
        private readonly string _refreshToken;
        private readonly IHttpActionResult _result;

        public const string AuthorizationTokenHeaderName = "Authorization";
        public const string RefreshTokenHeaderName = "RefreshToken";

        public HttpAuthResult(IHttpActionResult result, string token, string refreshToken)
        {
            _result = result;
            _token = token;
            _refreshToken = refreshToken;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var response = _result.ExecuteAsync(cancellationToken).Result;
                response.Headers.Add("Access-Control-Expose-Headers", new [] { AuthorizationTokenHeaderName , RefreshTokenHeaderName });
                response.Headers.Add(AuthorizationTokenHeaderName, _token);
                response.Headers.Add(RefreshTokenHeaderName, _refreshToken);
                return response;
            },
            cancellationToken);
        }
    }
}