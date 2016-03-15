using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SSQSignon
{
    public abstract class ProxyController : ApiController
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

        public ProxyController(string moduleName, string clientId, string clientSecret, bool grantTypeDetection = false)
        {
            ModuleName = moduleName;
            ClientId = clientId;
            ClientSecret = clientSecret;
            GrantTypeDetection = grantTypeDetection;
            restClient = new RestClient(string.Format("https://ssqsignon.com/{0}/auth", moduleName));
            if (!string.IsNullOrEmpty(clientSecret))
            {
                restClient.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(clientId, clientSecret);
            }
        }

        public virtual dynamic Post(AuthRequestModel model)
        {
            var request = new RestRequest(Method.POST);
            if (string.IsNullOrEmpty(model.client_id))
            {
                model.client_id = ClientId;
            }
            if (string.IsNullOrEmpty(model.grant_type) && GrantTypeDetection)
            {
                model.grant_type = DetectGrantType(model);
            }
            request.AddJsonBody(model);

            var response = restClient.Execute<AuthorizationResponse>(request);
            if (response.ErrorException == null && response.StatusCode.Equals(HttpStatusCode.OK))
            {
                return Json(response.Data);
            }
            else
            {
                var responseProxy = new HttpResponseMessage(response.StatusCode);
                var mimeType = new System.Net.Mime.ContentType(response.ContentType);
                responseProxy.Content = new StringContent(response.Content, System.Text.Encoding.GetEncoding(mimeType.CharSet), mimeType.MediaType);
                return responseProxy;
            }
        }

        protected virtual string DetectGrantType(AuthRequestModel model)
        {
            if (!string.IsNullOrEmpty(model.username) || !string.IsNullOrEmpty(model.password))
            {
                return "password";
            }
            if (!string.IsNullOrEmpty(model.code))
            {
                return "authorization_code";
            }
            if (!string.IsNullOrEmpty(model.refresh_token))
            {
                return "refresh_token";
            }
            return null;
        }

        protected string ModuleName { get; set; }

        protected string ClientId { get; set; }

        protected string ClientSecret { get; set; }

        protected bool GrantTypeDetection { get; set; }

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
