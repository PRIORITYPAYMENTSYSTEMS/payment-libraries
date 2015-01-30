using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace Card_Account_Transactions
{
    public class OAuth_Utilities
    {
        public string UrlEncode(string value)
        {
            string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        public  byte[] keyBuilder(NameValueCollection tokens)
        {
            ConfigFactory ConfigFactory = new ConfigFactory();
            SHA1 sha1 = new SHA1CryptoServiceProvider();

            string key = string.Format("{0}&", UrlEncode(ConfigFactory.consumerSecret));
            if(tokens != null)
            {
                key += UrlEncode(tokens["oauth_token_secret"]);
            }

            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            return keyBytes;
        }

        public  string createHeader(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            OAuth_Utilities.QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();
        }

        public  string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        public class QueryParameter
        {
            private string name = null;
            private string value = null;

            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        public class QueryParameterComparer : IComparer<QueryParameter>
        {

            #region IComparer<QueryParameter> Members

            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }

        public string sendRequest(string[] endpointData, string headers, PaymentFactory JSON = null, List<OAuth_Utilities.QueryParameter> qp = null)
        {

            HttpWebRequest r;
            if (qp != null)
            {
                qp.Sort(new OAuth_Utilities.QueryParameterComparer());
                
                endpointData[0] = endpointData[0] + "?" + newOAuth.RequestNormalizer.NormalizeRequestParameters(qp);
                Console.WriteLine(endpointData[0]);
                r = (HttpWebRequest)WebRequest.Create(endpointData[0]);
            }
            else
            {
                r = (HttpWebRequest)WebRequest.Create(endpointData[0]);
            }

            string responseFromServer = "";    
            r = (HttpWebRequest)WebRequest.Create(endpointData[0]);
            r.Method = endpointData[1];
            r.Headers.Add("Authorization", headers);

            if (JSON != null)
            {
                r.ContentType = "application/json";
                MemoryStream stream1 = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(PaymentFactory));

                ser.WriteObject(stream1, JSON);
                byte[] json = stream1.ToArray();
                stream1.Close();
                string actualJson = Encoding.UTF8.GetString(json, 0, json.Length).ToString();
                using (var streamWriter = new StreamWriter(r.GetRequestStream()))
                {
                    streamWriter.Write(actualJson);
                }

                var httpResponse = (HttpWebResponse)r.GetResponse();
                NameValueCollection H = httpResponse.Headers;
                Stream dataStream = r.GetRequestStream();
                dataStream = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = H["Location"];

            }
            else
            {
                Stream dataStream = r.GetRequestStream();
                WebResponse response = r.GetResponse();

                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }

            return responseFromServer;
        }
    }
}
