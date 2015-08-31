using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SSQSignon
{
    public abstract class AuthProxyController : ApiController
    {
        public class AuthRequestModel
        {
            public string grant_type { get; set; }

            public string client_id { get; set; }

            public string code { get; set; }

            public string redirect_uri { get; set; }

            public string username { get; set; }

            public string password { get; set; }

            public string refresh_token { get; set; }

            public string scope { get; set; }

            public string client_secret { get; set; }
        }

        public AuthProxyController(string moduleName, string clientId, string clientSecret)
        {
            ModuleName = moduleName;
            ClientId = clientId;
            ClientSecret = clientSecret;
            restClient = new RestClient(string.Format("https://tinyusers.azurewebsites.net/{0}/auth", moduleName));
            if (!string.IsNullOrEmpty(clientSecret))
            {
                restClient.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(clientId, clientSecret);
            }
        }

        public virtual dynamic Post(AuthRequestModel model)
        {
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(model);

            var response = restClient.Execute<AuthorizationResponse>(request);
            if (response.ErrorException == null && response.StatusCode.Equals(HttpStatusCode.OK))
            {
                return Json(response.Data);
            }
            else
            {
                return StatusCode(HttpStatusCode.BadGateway);
            }
        }

        protected string ModuleName { get; set; }

        protected string ClientId { get; set; }

        protected string ClientSecret { get; set; }

        private class AuthorizationResponse
        {
            public string user_id { get; set; }

            public string scope { get; set; }

            public string access_token { get; set; }

            public string refresh_token { get; set; }
        }

        private RestClient restClient;
    }
}
