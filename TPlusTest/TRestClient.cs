using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TPlusTest
{
    public interface ITRestClient
    {
        ITRestRequest AddSignHeader(ITRestRequest request, string name, string value);

        string Execute(ITRestRequest request);
    }
    public class TRestClient : ITRestClient
    {

        protected RestClient _client;

        protected string AuthToken
        {
            get;
            set;
        }

        protected string DateFormat
        {
            get;
            set;
        }

        static TRestClient()
        {
        }

        public TRestClient(string hosturl)
        {
            this._client = new RestClient()
            {
                BaseUrl = new Uri(hosturl),
                Timeout = 30500
            };
        }

        public ITRestRequest AddSignHeader(ITRestRequest request, string name, string value)
        {
            return request.AddParameter(name, value, TParameterType.HttpHeader);
        }

        public virtual T Execute<T>(ITRestRequest request)
        where T : new()
        {
            request.Request.OnBeforeDeserialization = (IRestResponse resp) => {
                if (resp.StatusCode >= HttpStatusCode.BadRequest)
                {
                    string str = string.Format("{{ \"RestException\" : {0} }}", resp.Content);
                    resp.Content = null;
                    resp.RawBytes = Encoding.UTF8.GetBytes(str.ToString());
                }
            };
            request.DateFormat = this.DateFormat;
            return this._client.Execute<T>(request.Request).Data;
        }

        public string Execute(ITRestRequest request)
        {
            IRestResponse restResponse = this._client.Execute(request.Request);
            string empty = string.Empty;
            empty = (restResponse.StatusCode < HttpStatusCode.BadRequest ? restResponse.Content : string.Format("{{ \"RestException\" : {0} }}", restResponse.Content));
            return empty;
        }

        public virtual void ExecuteAsync<T>(ITRestRequest request, Action<T> callback)
        where T : new()
        {
            request.Request.OnBeforeDeserialization = (IRestResponse resp) => {
                if (resp.StatusCode >= HttpStatusCode.BadRequest)
                {
                    string str = string.Format("{{ \"RestException\" : {0} }}", resp.Content);
                    resp.Content = null;
                    resp.RawBytes = Encoding.UTF8.GetBytes(str.ToString());
                }
            };
            request.DateFormat = "ddd, dd MMM yyyy HH:mm:ss '+0000'";
            this._client.ExecuteAsync<T>(request.Request, (IRestResponse<T> response) => callback(response.Data));
        }

        public virtual void ExecuteAsync(IRestRequest request, Action<IRestResponse> callback)
        {
            this._client.ExecuteAsync(request, callback);
        }
    }
}
