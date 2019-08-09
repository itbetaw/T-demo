using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TPlusTest
{
    class Program
    {
        static void Main()
        {
            // v2 获取token
            string appkey = "自己申请的appkey";
            string appsecret = "自己申请的appsecret ";
            string orgid = "自己企业的orgId";

           
            //string privateKeyPath = @"d:\cjet_pri.pem";



            var header = new Dictionary<string, object>
                {
                    {"appkey", appkey},
                    {"orgid",orgid},//
                    {"appsecret", appsecret}
                };

            string datas = JsonConvert.SerializeObject(header);
            TokenManage tokenManage = new TokenManage();
            string signvalue = tokenManage.CreateSignedToken(datas, @"d:\cjet_pri.pem");
            string authStr = @"{""appKey"":""" + appkey + @""",""authInfo"":""" + signvalue + @""",""orgId"":" + orgid + @"}";
            string encode = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(authStr));
            string serverUrl = "http://localhost:4000/TPlus/api/v2/";
            string host = serverUrl.Substring(0, serverUrl.IndexOf('/', serverUrl.IndexOf("//") + 2) + 1);
            TRestClient restclient = new TRestClient(host);
            ITRestRequest restquest = new TRestRequest();
            restquest.Resource = serverUrl.Replace(host, "") + "collaborationapp/GetAnonymousTPlusToken?IsFree=1";
            restquest.AddParameter("Authorization", encode, TParameterType.HttpHeader);
            restquest.Method = TMethod.POST;
            string responsedata = restclient.Execute(restquest);
            Newtonsoft.Json.Linq.JObject token = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(responsedata);
            var token_v2 = token["access_token"].ToString();

            Console.WriteLine("鉴权接口响应：" + responsedata);
            Console.WriteLine("token:" + token_v2);


            var customParas = new Dictionary<string, object>
                {
                    {"access_token", token_v2},

                };
            string bizAuthorization = tokenManage.CreateSignedToken(datas, @"d:\cjet_pri.pem", customParas);
            ITRestRequest restquest1 = new TRestRequest();
            restquest1.Method = TMethod.POST;
            restquest1.Resource = serverUrl.Replace(host, "") + "Account/Query";
            string authStr1 = @"{""appKey"":""" + appkey + @""",""authInfo"":""" + bizAuthorization + @""",""orgId"":" + orgid + @"}";
            string encode1 = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(authStr1));
            restquest1.AddParameter("Authorization", encode1, TParameterType.HttpHeader);
            string args = @"{dto: {}}";
            restquest1.AddParameter("_args", args);
            restclient = new TRestClient(host);
            string responsedata1 = restclient.Execute(restquest1);
            Console.WriteLine("凭证查询接口响应：" + responsedata1);

            Console.ReadLine();

        }




        public static string ConvertBytesToHex(byte[] arrByte, bool reverse)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (reverse)
            {
                Array.Reverse(arrByte);
            }
            byte[] numArray = arrByte;
            for (int i = 0; i < (int)numArray.Length; i++)
            {
                stringBuilder.AppendFormat("{0:x2}", numArray[i]);
            }
            return stringBuilder.ToString();
        }
    }
}
