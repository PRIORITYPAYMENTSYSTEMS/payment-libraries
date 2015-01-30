using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System;

namespace Card_Account_Transactions
{
    public class RetrieveTokens
    {
        public NameValueCollection getAccessToken()
        {
            var C = new ConfigFactory();
            var OAuth = new newOAuth(ConfigFactory.endPointRequestToken, null); 
            OAuth_Utilities U = new OAuth_Utilities();

            string Headers = OAuth.createHeaders();
            string request_Token_Response = U.sendRequest(ConfigFactory.endPointRequestToken, Headers);
            NameValueCollection rt = HttpUtility.ParseQueryString(request_Token_Response);

            OAuth = new newOAuth(ConfigFactory.endPointAccessToken, rt);
            Headers = OAuth.createHeaders();
            string access_Token_Response = U.sendRequest(ConfigFactory.endPointAccessToken, Headers);
            NameValueCollection at = HttpUtility.ParseQueryString(access_Token_Response);
            var token = at["oauth_token"];
            var tokenSecret = at["oauth_token_secret"];

            return at;

        }
    }
}