using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPlusTest
{
    public interface ITRestRequest
    {
        int Attempts
        {
            get;
        }

        string DateFormat
        {
            get;
            set;
        }

        TMethod Method
        {
            get;
            set;
        }

        List<TParameter> Parameters
        {
            get;
        }

        int ReadWriteTimeout
        {
            get;
            set;
        }

        IRestRequest Request
        {
            get;
        }

        string Resource
        {
            get;
            set;
        }

        int Timeout
        {
            get;
            set;
        }

        bool UseDefaultCredentials
        {
            get;
            set;
        }

        ITRestRequest AddJsonBody(object obj);

        ITRestRequest AddObject(object obj, params string[] includedProperties);

        ITRestRequest AddObject(object obj);

        ITRestRequest AddParameter(string name, object value);

        ITRestRequest AddParameter(string name, object value, TParameterType type);

        ITRestRequest AddParameter(string name, object value, string contentType, TParameterType type);
    }

    public enum TMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
        HEAD,
        OPTIONS,
        PATCH,
        MERGE
    }
    public class TParameter
    {
        private Parameter _para;

        public string ContentType
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public TParameterType Type
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public TParameter(Parameter para)
        {
            this.Name = para.Name;
            this.Value = para.Value;
            this.Type = (TParameterType)Enum.Parse(typeof(TParameterType), para.Type.ToString());
            this.ContentType = para.ContentType;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}", this.Name, this.Value);
        }
    }
    public enum TParameterType
    {
        Cookie,
        GetOrPost,
        UrlSegment,
        HttpHeader,
        RequestBody,
        QueryString
    }

    public class TRestRequest : ITRestRequest
    {
        private IRestRequest _request;

        public int Attempts
        {
            get
            {
                return this._request.Attempts;
            }
        }

        public string DateFormat
        {
            get
            {
                return this._request.DateFormat;
            }
            set
            {
                this._request.DateFormat = value;
            }
        }

        public TMethod Method
        {
            get
            {
                return (TMethod)Enum.Parse(typeof(TMethod), this._request.Method.ToString());
            }
            set
            {
                this._request.Method = (Method)Enum.Parse(typeof(Method), value.ToString());
            }
        }

        public List<TParameter> Parameters
        {
            get
            {
                List<TParameter> tParameters = new List<TParameter>();
                foreach (Parameter parameter in this._request.Parameters)
                {
                    tParameters.Add(new TParameter(parameter));
                }
                return tParameters;
            }
        }

        public int ReadWriteTimeout
        {
            get
            {
                return this._request.ReadWriteTimeout;
            }
            set
            {
                this._request.ReadWriteTimeout = value;
            }
        }

        public IRestRequest Request
        {
            get
            {
                return this._request;
            }
        }

        public string Resource
        {
            get
            {
                return this._request.Resource;
            }
            set
            {
                this._request.Resource = value;
            }
        }

        public int Timeout
        {
            get
            {
                return this._request.Timeout;
            }
            set
            {
                this._request.Timeout = value;
            }
        }

        public bool UseDefaultCredentials
        {
            get
            {
                return this._request.UseDefaultCredentials;
            }
            set
            {
                this._request.UseDefaultCredentials = value;
            }
        }

        public TRestRequest()
        {
            this._request = new RestRequest();
        }

        protected TRestRequest(RestRequest request)
        {
            this._request = request;
        }

        public ITRestRequest AddJsonBody(object obj)
        {
            this._request = this._request.AddJsonBody(obj);
            return this;
        }

        public ITRestRequest AddObject(object obj)
        {
            this._request = this._request.AddObject(obj);
            return this;
        }

        public ITRestRequest AddObject(object obj, params string[] includedProperties)
        {
            this._request = this._request.AddObject(obj, includedProperties);
            return this;
        }

        public ITRestRequest AddParameter(string name, object value)
        {
            this._request = this._request.AddParameter(name, value);
            return this;
        }

        public ITRestRequest AddParameter(string name, object value, TParameterType type)
        {
            ParameterType parameterType = (ParameterType)Enum.Parse(typeof(ParameterType), type.ToString());
            this._request = this._request.AddParameter(name, value, parameterType);
            return this;
        }

        public ITRestRequest AddParameter(string name, object value, string contentType, TParameterType type)
        {
            ParameterType parameterType = (ParameterType)Enum.Parse(typeof(ParameterType), type.ToString());
            this._request = this._request.AddParameter(name, value, contentType, parameterType);
            return this;
        }
    }
}
