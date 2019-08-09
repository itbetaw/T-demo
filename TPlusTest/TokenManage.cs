using CSharp_easy_RSA_PEM;
using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace TPlusTest
{
    public class TokenManage
    {
        public string CreateSignedToken(string data, string pemFile)
        {
            TimeSpan utcNow = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            double totalMilliseconds = utcNow.TotalMilliseconds + 30000;
            Dictionary<string, object> strs = new Dictionary<string, object>()
            {
                { "sub", "tester" },
                { "exp", totalMilliseconds },
                { "datas", this.GetMd5(data).ToLower() }
            };
            Dictionary<string, object> strs1 = strs;
            RSACryptoServiceProvider rSACryptoServiceProvider = Crypto.DecodeRsaPrivateKey(File.ReadAllText(pemFile), "");
            return JWT.Encode(strs1, rSACryptoServiceProvider, JwsAlgorithm.PS256, null);
        }

        //byte[] GetBytesFromPEM(string pemString, string section)
        //{
        //    var header = String.Format("-----BEGIN {0}-----", section);
        //    var footer = String.Format("-----END {0}-----", section);

        //    var start = pemString.IndexOf(header, StringComparison.Ordinal);
        //    if (start < 0)
        //        return null;

        //    start += header.Length;
        //    var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

        //    if (end < 0)
        //        return null;

        //    return Convert.FromBase64String(pemString.Substring(start, end));
        //}

        //string GetBytesFromPEM1(string pemString, string section)
        //{
        //    var header = String.Format("-----BEGIN {0}-----", section);
        //    var footer = String.Format("-----END {0}-----", section);

        //    var start = pemString.IndexOf(header, StringComparison.Ordinal);
        //    if (start < 0)
        //        return null;

        //    start += header.Length;
        //    var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

        //    if (end < 0)
        //        return null;

        //    return pemString.Substring(start, end);
        //}


        //public static string Encode(object payload, object key, JwsAlgorithm algorithm, IDictionary<string, object> extraHeaders = null)
        //{
        //    return Encode(JsonConvert.SerializeObject(payload), key, algorithm, extraHeaders);
        //}

        //public static string Encode(string payload, object key, JwsAlgorithm algorithm, IDictionary<string, object> extraHeaders = null)
        //{
        //    Ensure.IsNotEmpty(payload, "Payload expected to be not empty, whitespace or null.", new object[0]);
        //    return EncodeBytes(Encoding.UTF8.GetBytes(payload), key, algorithm, extraHeaders);
        //}

        //public static string EncodeBytes(byte[] payload, object key, JwsAlgorithm algorithm, IDictionary<string, object> extraHeaders = null)
        //{
        //    if (payload == null)
        //    {
        //        throw new ArgumentNullException("payload");
        //    }
        //    if (extraHeaders == null)
        //    {
        //        extraHeaders = new Dictionary<string, object>()
        //        {
        //            { "typ", "JWT" }
        //        };
        //    }
        //    Dictionary<string, object> strs = new Dictionary<string, object>()
        //    {
        //        { "alg", "PS256" }
        //    };
        //    Dictionaries.Append<string, object>(strs, extraHeaders);
        //    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(strs));
        //    byte[] bytes2 = Encoding.UTF8.GetBytes(Serialize(new byte[][]
        //    {
        //        bytes,
        //        payload
        //    }));



        //    //var pc = new X509Certificate2(@"d:\cjet_pri.pem", "", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

        //    string a = RsaPemFormatHelper.Pkcs1PrivateKeyFormatRemove(File.ReadAllText(@"d:\cjet_pri.pem")).Replace("\n", "");

        //    var rsa = new RSAHelper(RSAType.RSA2, Encoding.UTF8, a, "");

        //    byte[] array = rsa.Sign(bytes2);

        //    //string bb = Encoding.UTF8.GetString(bytes2);

        //    //byte[] numArray = Encoding.UTF8.GetBytes(Serialize(new byte[][] { bytes, payload }));
        //    //byte[] numArray1 = JWT.HashAlgorithms[algorithm].Sign(numArray, key);

        //    //byte[] numArray1 = new RsaPkcs1Util(Encoding.UTF8, "", File.ReadAllText(@"d:\cjet_pri.pem")).SignDataGetBytes(Serialize(new byte[][] { bytes, payload }), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        //    //return Compact.Serialize(new byte[][] { bytes, payload, numArray1 });

        //    //byte[] array = new RsaPkcs1Util(Encoding.UTF8, "", File.ReadAllText(@"d:\cjet_pri.pem")).SignDataGetBytes(bb, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);




        //    return Serialize(new byte[][]
        //    {
        //        bytes,
        //        payload,
        //        array
        //    });
        //}



        //public static string Serialize(params byte[][] parts)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    byte[][] numArray = parts;
        //    for (int i = 0; i < (int)numArray.Length; i++)
        //    {
        //        byte[] numArray1 = numArray[i];
        //        stringBuilder.Append(Base64Url.Encode(numArray1)).Append(".");
        //    }
        //    stringBuilder.Remove(stringBuilder.Length - 1, 1);
        //    return stringBuilder.ToString();
        //}


        public static string CreateTokenByHandler(Dictionary<string, object> payLoad, double expiresMinute, string privateKey)
        {

            var now = DateTime.UtcNow;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new List<Claim>();
            foreach (var key in payLoad.Keys)
            {
                var tempClaim = new Claim(key, payLoad[key]?.ToString());
                claims.Add(tempClaim);
            }


            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                notBefore: now,
                //expires: now.Add(TimeSpan.FromMinutes(expiresMinute)),
                expires: now.Add(TimeSpan.FromMilliseconds(30000)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey)), SecurityAlgorithms.RsaSsaPssSha256Signature));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        //private string BuildParas(IDictionary<string, string> paras)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    if (paras != null && paras.Count > 0)
        //    {
        //        int num = 0;
        //        foreach (string text in paras.Keys)
        //        {
        //            if (num > 0)
        //            {
        //                stringBuilder.AppendFormat("&{0}={1}", text, Uri.EscapeDataString(paras[text]));
        //            }
        //            else
        //            {
        //                stringBuilder.AppendFormat("{0}={1}", text, Uri.EscapeDataString(paras[text]));
        //            }
        //            num++;
        //        }
        //    }
        //    return stringBuilder.ToString();
        //}
        public string CreateSignedToken(string data, string pemFile, IDictionary<string, object> customparas)
        {
            double num = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds + 30000;
            Dictionary<string, object> dictionary = new Dictionary<string, object>
            {
                {
                    "sub",
                    "chanjet"
                },
                {
                    "exp",
                    num
                },
                {
                    "datas",
                    this.GetMd5(data).ToLower()
        }
    };
            foreach (string key in customparas.Keys)
            {
                dictionary.Add(key, customparas[key]);
            }
            string privateKey = File.ReadAllText(pemFile);
            RSACryptoServiceProvider key2 = Crypto.DecodeRsaPrivateKey(privateKey, "");
            return JWT.Encode(dictionary, key2, JwsAlgorithm.PS256, null);
        }
        //public bool ValidateToken(string token, string src, string pemFile)
        //{
        //    bool result;
        //    try
        //    {
        //        string publicKey = File.ReadAllText(pemFile);
        //        RSACryptoServiceProvider key = Crypto.DecodeX509PublicKey(publicKey);
        //        Dictionary<string, object> dictionary = JWT.Decode<Dictionary<string, object>>(token, key);
        //        object obj = dictionary["datas"];
        //        Console.WriteLine(string.Format(obj.ToString(), new object[0]));
        //        result = object.Equals(this.GetMd5(src), obj);
        //    }
        //    catch (Exception ex)
        //    {
        //        //LoggerAdapter.Error(string.Format("ValidateToken fail:{0}", ex.Message));
        //        result = false;
        //    }
        //    return result;
        //}
        //public IDictionary<string, object> GetCustomParaFromToken(string token, string src, string pemFile, List<string> keys)
        //{
        //    IDictionary<string, object> dictionary = new Dictionary<string, object>();
        //    IDictionary<string, object> result;
        //    try
        //    {
        //        string publicKey = File.ReadAllText(pemFile);
        //        RSACryptoServiceProvider key = Crypto.DecodeX509PublicKey(publicKey);
        //        Dictionary<string, object> dictionary2 = JWT.Decode<Dictionary<string, object>>(token, key);
        //        object obj = dictionary2["datas"];
        //        Console.WriteLine(string.Format(obj.ToString(), new object[0]));
        //        if (object.Equals(this.GetMd5(src), obj))
        //        {
        //            foreach (string key2 in keys)
        //            {
        //                dictionary.Add(key2, dictionary2[key2]);
        //            }
        //        }
        //        result = dictionary;
        //    }
        //    catch (Exception ex)
        //    {
        //        //LoggerAdapter.Error(string.Format("ValidateToken fail:{0}", ex.Message));
        //        result = dictionary;
        //    }
        //    return result;
        //}
        public string GetMd5(string requestdatas)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(requestdatas));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}
