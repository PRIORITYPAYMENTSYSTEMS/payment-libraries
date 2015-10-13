using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PriorityPaymentTest
{
    public enum AuthenticationMethod
    {
        OAuth,
        Basic
    }

    public class PpsApiRequest
    {
        public PpsApiRequest(string baseAddress, string consumerKey, string consumerSecret, AuthenticationMethod authenticationMethod = AuthenticationMethod.Basic)
        {
            this.BASE_ADDRESS = baseAddress;
            this.CONSUMER_KEY = consumerKey;
            this.CONSUMER_SECRET = consumerSecret;
            this.AUTHENTICATION_METHOD = authenticationMethod;

            SHA1 sha1 = new SHA1CryptoServiceProvider();
            this.CONSUMER_SECRET_HASHED = consumerSecret;// ComputeHash(sha1, consumerSecret);

            Init();
        }

        string BASE_ADDRESS = "";
        string CONSUMER_KEY = "";
        string CONSUMER_SECRET = "";
        string CONSUMER_SECRET_HASHED = "";
        AuthenticationMethod AUTHENTICATION_METHOD = AuthenticationMethod.Basic;

        private string OAUTH_REQUEST_TOKEN_URL = "";
        private string OAUTH_ACCESS_TOKEN_URL = "";

        private NameValueCollection oauth_1A_AccessToken = null;

        public HttpRequestMessage BuildRequest(string apiUrl, Dictionary<string, string> queryString, HttpMethod method, object content, string mediaType = null)
        {
            string json = null;

            if (content != null)
            {
                // TODO: You need to implement a ToJson extension method using your favorite JSON serializer
                // request.Content = new StringContent(content.ToJson());

                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(content.GetType());
                ser.WriteObject(stream, content);

                stream.Position = 0;

                StreamReader sr = new StreamReader(stream);
                json = sr.ReadToEnd();
            }

            return BuildRequest(apiUrl, queryString, method, json, mediaType);
        }

        public HttpRequestMessage BuildRequest(string apiUrl, Dictionary<string, string> queryString, HttpMethod method, string json, string mediaType = null)
        {
            if (apiUrl.StartsWith(BASE_ADDRESS, StringComparison.OrdinalIgnoreCase))
                apiUrl = apiUrl.Remove(0, BASE_ADDRESS.Length);

            var requestUrl = string.Format("{0}{1}", BASE_ADDRESS, apiUrl);
            var url = requestUrl;

            if (queryString != null && queryString.Count > 0)
            {
                string concat = "?";
                foreach (string key in queryString.Keys)
                {
                    url += string.Format("{0}{1}={2}", concat, key, queryString[key]);
                    concat = "&";
                }
            }

            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = method
            };

            if (String.IsNullOrWhiteSpace(mediaType))
                mediaType = "application/json";

            if (!string.IsNullOrEmpty(json))
                request.Content = new StringContent(json);

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(mediaType);

            if(this.AUTHENTICATION_METHOD == AuthenticationMethod.OAuth)
            {
                if (oauth_1A_AccessToken == null)
                    oauth_1A_AccessToken = OAuth_1A_AccessToken();

                request = Sign(request, CONSUMER_KEY, CONSUMER_SECRET_HASHED,
                    oauth_1A_AccessToken[Constants.OAuth.V1A.TokenKey],
                    oauth_1A_AccessToken[Constants.OAuth.V1A.TokenSecretKey]);
            }
            else
            {
                string scheme = "BASIC";
                var encoded = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}:{1}", CONSUMER_KEY, CONSUMER_SECRET)));
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, encoded);
            }
            
            return request;
        }

        private void Init()
        {
            // OAUTH_REQUEST_TOKEN_URL = String.Format("{0}oauth/initiate", this.BASE_ADDRESS);
            // OAUTH_ACCESS_TOKEN_URL = String.Format("{0}oauth/token", this.BASE_ADDRESS);

            OAUTH_REQUEST_TOKEN_URL = String.Format("{0}/OAuth/1A/RequestToken", this.BASE_ADDRESS);
            OAUTH_ACCESS_TOKEN_URL = String.Format("{0}/OAuth/1A/AccessToken", this.BASE_ADDRESS);
        }

        private NameValueCollection OAuth_1A_AccessToken()
        {
            if (oauth_1A_AccessToken != null)
                return oauth_1A_AccessToken;

            var requestToken = OAuth_1A_RequestToken();

            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(OAUTH_ACCESS_TOKEN_URL),
                Method = HttpMethod.Post
            };

            string mediaType = "application/x-www-form-urlencoded";
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            authRequest = Sign(authRequest, CONSUMER_KEY, CONSUMER_SECRET_HASHED,
                requestToken[Constants.OAuth.V1A.TokenKey],
                requestToken[Constants.OAuth.V1A.TokenSecretKey]);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

            var response = authResponse.Content.ReadAsStringAsync().Result;

            NameValueCollection results = new NameValueCollection();

            foreach (var result in response.Split('&'))
                results.Add(result.Split('=')[0], result.Split('=')[1]);

            return results;

            //var json = authResponse.Content.ReadAsStreamAsync().Result;

            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NameValueCollection));
            //oauth_1A_AccessToken = (NameValueCollection)ser.ReadObject(json);

            //return oauth_1A_AccessToken;
        }

        private NameValueCollection OAuth_1A_RequestToken()
        {
            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(OAUTH_REQUEST_TOKEN_URL),
                Method = HttpMethod.Get,
            };

            string mediaType = "application/x-www-form-urlencoded";
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            authRequest = Sign(authRequest, CONSUMER_KEY, CONSUMER_SECRET_HASHED, null, null);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

            try
            {
                var response = authResponse.Content.ReadAsStringAsync().Result;

                NameValueCollection results = new NameValueCollection();

                foreach (var result in response.Split('&'))
                    results.Add(result.Split('=')[0], result.Split('=')[1]);

                return results;
                //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NameValueCollection));
                //return (NameValueCollection)ser.ReadObject(json);
            }
            catch
            {
                string result = authResponse.Content.ReadAsStringAsync().Result;
                throw;
            }
        }

        private HttpRequestMessage Sign(HttpRequestMessage request, string consumerKey, string consumerSecret,
            string accessToken = null, string accessSecret = null, string nonce = null, string timestamp = null, string callback = null, string verifier = null)
        {
            nonce = nonce ?? Guid.NewGuid().ToString();
            timestamp = timestamp ?? TimeSpan.FromTicks(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks).TotalSeconds.ToString();

            string scheme = "OAuth";
            string signature = Signature.GetOAuth1ASignature(request, consumerKey, consumerSecret, accessToken, accessSecret, timestamp, nonce, callback, verifier);

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}=\"{1}\"", Constants.OAuth.V1A.ConsumerKeyKey, consumerKey);
            if (!string.IsNullOrEmpty(accessToken)) sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.TokenKey, accessToken);
            sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.TimestampKey, timestamp);
            sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.NonceKey, nonce);
            sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.SignatureKey, signature);
            sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.SignatureMethodKey, Constants.OAuth.V1A.HMACSHA1SignatureType);
            sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.VersionKey, Constants.OAuth.V1A.Version);
            if (!string.IsNullOrWhiteSpace(callback)) sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.CallbackKey, callback);
            if (!string.IsNullOrWhiteSpace(verifier)) sb.AppendFormat(",{0}=\"{1}\"", Constants.OAuth.V1A.VerifierKey, verifier);

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(scheme, sb.ToString());

            return request;
        }

        private class Constants
        {
            public class OAuth
            {
                public class V1A
                {
                    public const string ConsumerKeyKey = "oauth_consumer_key";
                    public const string CallbackKey = "oauth_callback";
                    public const string VersionKey = "oauth_version";
                    public const string SignatureMethodKey = "oauth_signature_method";
                    public const string SignatureKey = "oauth_signature";
                    public const string TimestampKey = "oauth_timestamp";
                    public const string NonceKey = "oauth_nonce";
                    public const string TokenKey = "oauth_token";
                    public const string TokenSecretKey = "oauth_token_secret";
                    public const string VerifierKey = "oauth_verifier";

                    public const string HMACSHA1SignatureType = "HMAC-SHA1";
                    public const string PlainTextSignatureType = "PLAINTEXT";
                    public const string RSASHA1SignatureType = "RSA-SHA1";

                    public const string Version = "1.0";
                }
            }
        }

        public enum SignatureMethod
        {
            Simple,
            OAuthMAC,
            OAuth1A
        }

        public static class Signature
        {
            internal const string DELIMITER = "\n";

            public static string GetSimpleSignature(HttpRequestMessage request, byte[] apiSecret, string token, string n, string ts)
            {
                string normalizedRequest = string.Join(DELIMITER, new string[] { token, ts, n });

                HMACSHA256 hmac = new HMACSHA256(apiSecret);
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedRequest)));
            }

            public static string GetMacSignature(HttpRequestMessage request, byte[] apiSecret, string token, string n, string ts, string bodyHash)
            {
                var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
                var requestMethod = request.Method.ToString().ToUpper();
                var resourcePath = request.RequestUri.AbsolutePath;
                var normalizedParameters = Signature.CreateNormalizedParameters(queryString);

                string normalizedRequest = string.Join(DELIMITER, new string[] { token, ts, n, bodyHash, requestMethod, resourcePath, normalizedParameters });


                HMACSHA256 hmac = new HMACSHA256(apiSecret);
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedRequest)));
            }

            public static string CreateBodyHash(HttpContent httpContent)
            {
                if (httpContent == null) return null;

                string bodyContent = System.Text.UTF8Encoding.UTF8.GetString(httpContent.ReadAsByteArrayAsync().Result);

                return CreateBodyHash(bodyContent);
            }

            public static string CreateBodyHash(string bodyContent)
            {
                // calculate body hash
                SHA256 sha = SHA256Managed.Create();
                var bodyHash = (bodyContent == null) ? null : Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(bodyContent)));

                return bodyHash;
            }

            public static string CreateNormalizedParameters(NameValueCollection queryString)
            {
                // normalize the query string
                List<string> values = new List<string>();
                foreach (string key in queryString.Keys)
                {
                    values.Add(System.Uri.EscapeDataString(string.Concat(key, "=", queryString.Get(key))));
                }

                values.ToList<string>().Sort();
                return string.Join(DELIMITER, values);
            }

            public static string GetOAuth1ASignature(HttpRequestMessage request, string consumerKey, string consumerSecret, string accessToken, string accessSecret, string ts, string n, string callback, string verifier)
            {
                OAuth1ASignature oauth = new OAuth1ASignature();
                string normalizedurl;
                string normalizedqueryparameters;
                string sig = oauth.GenerateSignature(request.RequestUri, consumerKey, consumerSecret, accessToken, accessSecret,
                    request.Method.ToString().ToUpper(), ts, n, OAuth1ASignature.SignatureTypes.HMACSHA1, callback, verifier, out normalizedurl, out normalizedqueryparameters);

                return sig;
            }
        }

        public string ComputeHash(HashAlgorithm hashAlgorithm, string data)
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

        public class OAuth1ASignature
        {

            /// <summary>
            /// Provides a predefined set of algorithms that are supported officially by the protocol
            /// </summary>
            public enum SignatureTypes
            {
                HMACSHA1,
                PLAINTEXT,
                RSASHA1
            }

            /// <summary>
            /// Provides an internal structure to sort the query parameter
            /// </summary>
            protected class QueryParameter
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

            /// <summary>
            /// Comparer class used to perform the sorting of the query parameters
            /// </summary>
            protected class QueryParameterComparer : IComparer<QueryParameter>
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

            protected const string OAuthVersion = "1.0";
            protected const string OAuthParameterPrefix = "oauth_";

            //
            // List of know and used oauth parameters' names
            //        
            protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
            protected const string OAuthCallbackKey = "oauth_callback";
            protected const string OAuthVersionKey = "oauth_version";
            protected const string OAuthSignatureMethodKey = "oauth_signature_method";
            protected const string OAuthSignatureKey = "oauth_signature";
            protected const string OAuthTimestampKey = "oauth_timestamp";
            protected const string OAuthNonceKey = "oauth_nonce";
            protected const string OAuthTokenKey = "oauth_token";
            protected const string OAuthTokenSecretKey = "oauth_token_secret";
            protected const string OAuthVerifierKey = "oauth_verifier";

            protected const string HMACSHA1SignatureType = "HMAC-SHA1";
            protected const string PlainTextSignatureType = "PLAINTEXT";
            protected const string RSASHA1SignatureType = "RSA-SHA1";

            protected Random random = new Random();

            protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            /// <summary>
            /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
            /// </summary>
            /// <param name="parameters">The query string part of the Url</param>
            /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
            private List<QueryParameter> GetQueryParameters(string parameters)
            {
                if (parameters.StartsWith("?"))
                {
                    parameters = parameters.Remove(0, 1);
                }

                List<QueryParameter> result = new List<QueryParameter>();

                if (!string.IsNullOrEmpty(parameters))
                {
                    string[] p = parameters.Split('&');
                    foreach (string s in p)
                    {
                        if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                        {
                            if (s.IndexOf('=') > -1)
                            {
                                string[] temp = s.Split('=');
                                result.Add(new QueryParameter(temp[0], UrlDecode(temp[1])));
                            }
                            else
                            {
                                result.Add(new QueryParameter(s, string.Empty));
                            }
                        }
                    }
                }

                return result;
            }

            public string UrlDecode(string value)
            {
                return System.Uri.UnescapeDataString(value);
            }

            /// <summary>
            /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
            /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
            /// </summary>
            /// <param name="value">The value to Url encode</param>
            /// <returns>Returns a Url encoded string</returns>
            public string UrlEncode(string value)
            {
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

            /// <summary>
            /// Normalizes the request parameters according to the spec
            /// </summary>
            /// <param name="parameters">The list of parameters already sorted</param>
            /// <returns>a string representing the normalized parameters</returns>
            protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
            {
                StringBuilder sb = new StringBuilder();
                QueryParameter p = null;
                for (int i = 0; i < parameters.Count; i++)
                {
                    p = parameters[i];
                    sb.AppendFormat("{0}={1}", p.Name, p.Value);

                    if (i < parameters.Count - 1)
                    {
                        sb.Append("&");
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// Generate the signature base that is used to produce the signature
            /// </summary>
            /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
            /// <param name="consumerKey">The consumer key</param>        
            /// <param name="token">The token, if available. If not available pass null or an empty string</param>
            /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
            /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
            /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
            /// <returns>The signature base</returns>
            public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, string callback, string verifier, out string normalizedUrl, out string normalizedRequestParameters)
            {
                if (token == null)
                {
                    token = string.Empty;
                }

                if (tokenSecret == null)
                {
                    tokenSecret = string.Empty;
                }

                if (string.IsNullOrEmpty(consumerKey))
                {
                    throw new ArgumentNullException("consumerKey");
                }

                if (string.IsNullOrEmpty(httpMethod))
                {
                    throw new ArgumentNullException("httpMethod");
                }

                if (string.IsNullOrEmpty(signatureType))
                {
                    throw new ArgumentNullException("signatureType");
                }

                normalizedUrl = null;
                normalizedRequestParameters = null;

                List<QueryParameter> parameters = GetQueryParameters(url.Query);
                parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
                parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
                parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
                parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
                parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));
                if (callback != null)
                    parameters.Add(new QueryParameter(OAuthCallbackKey, callback));//UrlDecode(callback)));
                if (verifier != null)
                    parameters.Add(new QueryParameter(OAuthVerifierKey, verifier));

                if (!string.IsNullOrEmpty(token))
                {
                    parameters.Add(new QueryParameter(OAuthTokenKey, token));
                }

                parameters.Sort(new QueryParameterComparer());

                normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
                if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
                {
                    normalizedUrl += ":" + url.Port;
                }
                normalizedUrl += url.AbsolutePath;
                normalizedRequestParameters = NormalizeRequestParameters(parameters);

                StringBuilder signatureBase = new StringBuilder();
                signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
                signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
                signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

                return signatureBase.ToString();
            }

            /// <summary>
            /// Generate the signature value based on the given signature base and hash algorithm
            /// </summary>
            /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
            /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
            /// <returns>A base64 string of the hash value</returns>
            public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
            {
                return ComputeHash(hash, signatureBase);
            }

            /// <summary>
            /// Generates a signature using the HMAC-SHA1 algorithm
            /// </summary>		
            /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
            /// <param name="consumerKey">The consumer key</param>
            /// <param name="consumerSecret">The consumer seceret</param>
            /// <param name="token">The token, if available. If not available pass null or an empty string</param>
            /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
            /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
            /// <returns>A base64 string of the hash value</returns>
            public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string callback, string verifier, out string normalizedUrl, out string normalizedRequestParameters)
            {
                return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, callback, verifier, out normalizedUrl, out normalizedRequestParameters);
            }

            /// <summary>
            /// Generates a signature using the specified signatureType 
            /// </summary>		
            /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
            /// <param name="consumerKey">The consumer key</param>
            /// <param name="consumerSecret">The consumer seceret</param>
            /// <param name="token">The token, if available. If not available pass null or an empty string</param>
            /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
            /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
            /// <param name="signatureType">The type of signature to use</param>
            /// <returns>A base64 string of the hash value</returns>
            public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, string callback, string verifier, out string normalizedUrl, out string normalizedRequestParameters)
            {
                normalizedUrl = null;
                normalizedRequestParameters = null;

                switch (signatureType)
                {
                    case SignatureTypes.PLAINTEXT:
                        return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                    case SignatureTypes.HMACSHA1:
                        string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, callback, verifier, out normalizedUrl, out normalizedRequestParameters);

                        HMACSHA1 hmacsha1 = new HMACSHA1();
                        hmacsha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

                        var signature = GenerateSignatureUsingHash(signatureBase, hmacsha1);

                        return signature;
                    case SignatureTypes.RSASHA1:
                        throw new NotImplementedException();
                    default:
                        throw new ArgumentException("Unknown signature type", "signatureType");
                }
            }

            /// <summary>
            /// Generate the timestamp for the signature        
            /// </summary>
            /// <returns></returns>
            public virtual string GenerateTimeStamp()
            {
                // Default implementation of UNIX time of the current UTC time
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt64(ts.TotalSeconds).ToString();
            }

            /// <summary>
            /// Generate a nonce
            /// </summary>
            /// <returns></returns>
            public virtual string GenerateNonce()
            {
                // Just a simple implementation of a random number between 123400 and 9999999
                return random.Next(123400, 9999999).ToString();
            }


            public string ComputeHash(HashAlgorithm hashAlgorithm, string data)
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
        }
    }
}
