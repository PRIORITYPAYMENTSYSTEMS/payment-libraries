using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace Card_Account_Transactions
{
    public class newOAuth
    {

        ConfigFactory ConfigFactory = new ConfigFactory();
        OAuth_Utilities Utilitay = new OAuth_Utilities();

        public string[] endpointData;
        public IList<Card_Account_Transactions.OAuth_Utilities.QueryParameter> queryParameters;
        public NameValueCollection tokens;
        public string startDate;
        public string endDate;
        public string datePostedFilter;

        public Dictionary<string, string> headers;
        public List<OAuth_Utilities.QueryParameter> parameters;

        public newOAuth(string[] endpointData, NameValueCollection tokens = null, string startDate = null, string endDate = null, string datePostedFilter = null)
        {
            this.endpointData = endpointData;
            this.tokens = tokens;
            this.datePostedFilter = datePostedFilter;
            this.startDate = startDate;
            this.endDate = endDate;
            
        }

        public static class Nonce_Stamp
        {

             public static string getTimeStamp(){
                 int stamp = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                 string sendTime = stamp.ToString();
                 return sendTime;
             }

             public static string GenerateNonce()
             {
                 Random random = new Random();
                 return random.Next(123400, 9999999).ToString();
             }

        }

        public List<OAuth_Utilities.QueryParameter> setupParams()
        {

            List<OAuth_Utilities.QueryParameter> parameters = new List<OAuth_Utilities.QueryParameter>();
            if (this.startDate != null)
            {

                if (this.datePostedFilter != null)
                {
                    parameters.Add(new OAuth_Utilities.QueryParameter("datePostedFilter", "true"));
                }
                parameters.Add(new OAuth_Utilities.QueryParameter("endDate", this.endDate));
                parameters.Add(new OAuth_Utilities.QueryParameter("startDate", this.startDate));

            }

            parameters.Add(new OAuth_Utilities.QueryParameter("oauth_version", "1.0"));
            parameters.Add(new OAuth_Utilities.QueryParameter("oauth_nonce", newOAuth.Nonce_Stamp.GenerateNonce()));
            parameters.Add(new OAuth_Utilities.QueryParameter("oauth_timestamp", newOAuth.Nonce_Stamp.getTimeStamp()));

            if(this.tokens != null)
            {
                parameters.Add(new OAuth_Utilities.QueryParameter("oauth_token", this.tokens["oauth_token"]));
            }

            parameters.Add(new OAuth_Utilities.QueryParameter("oauth_signature_method", "HMAC-SHA1"));
            parameters.Add(new OAuth_Utilities.QueryParameter("oauth_consumer_key", ConfigFactory.consumerKey));

            return parameters;

        }

        public static class RequestNormalizer
        {

            public static string NormalizeRequestParameters(IList<Card_Account_Transactions.OAuth_Utilities.QueryParameter> parameters)
            {
                StringBuilder sb = new StringBuilder();
                OAuth_Utilities.QueryParameter p = null;
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

        }

        public string GenerateSignatureBase( string[] endPointData)
        {
           parameters = setupParams();
            parameters.Sort(new OAuth_Utilities.QueryParameterComparer());

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", endPointData[1].ToUpper());
            signatureBase.AppendFormat("{0}&", Utilitay.UrlEncode(endPointData[0]));
            signatureBase.AppendFormat("{0}", Utilitay.UrlEncode(newOAuth.RequestNormalizer.NormalizeRequestParameters(parameters)));

            return signatureBase.ToString();

        }
      
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
          OAuth_Utilities O = new OAuth_Utilities();    
            return O.ComputeHash(hash, signatureBase);
        }

        public string GenerateSignature(string[] endpointData)
        {
           string baseString = GenerateSignatureBase(endpointData);

            HMACSHA1 hmacsha1 = new HMACSHA1();
           SHA1 sha1 = new SHA1CryptoServiceProvider();
            OAuth_Utilities O = new OAuth_Utilities();

            hmacsha1.Key = O.keyBuilder(this.tokens);
           string signature = GenerateSignatureUsingHash(baseString, hmacsha1);

            return signature;
        }

        public void recaptureParams()
        {
            string signature = GenerateSignature(this.endpointData);
            this.parameters.Add(new OAuth_Utilities.QueryParameter("oauth_signature", Utilitay.UrlEncode(signature)));
        }

        public string createHeaders()
        {

            recaptureParams();

            this.parameters.Sort(new OAuth_Utilities.QueryParameterComparer());

            StringBuilder headerString = new StringBuilder();
            headerString.AppendFormat("{0}", (Utilitay.createHeader(parameters)));

            string h = "OAuth " + headerString;
            return h;
            
        }
    }
}
