using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Card_Account_Transactions
{
    public class Test
    {
        public static void Main(string[] args)
        {
            var C = new ConfigFactory();
            var OAuth = new newOAuth(ConfigFactory.endPointRequestToken);
            OAuth_Utilities U = new OAuth_Utilities();

            var t = new RetrieveTokens();
            NameValueCollection toke = t.getAccessToken();

            PaymentFactory p = new PaymentFactory();
                p.merchantId = C.merchantId;
                p.tenderType = "Card";
                p.amount = "0.01";
                p.cardAccount = new cardAccount();
                p.cardAccount.number = "4444555566667777";
                p.cardAccount.expiryMonth = "07";
                p.cardAccount.expiryYear = "2016";
                p.cardAccount.cvv = "123";
                p.cardAccount.avsZip = "12345";
                p.cardAccount.avsStreet = "1234";

            OAuth = new newOAuth(ConfigFactory.payment, toke);
            string Headers = OAuth.createHeaders();
            string createdPayment = U.sendRequest(ConfigFactory.payment, Headers, p);

            Console.WriteLine(createdPayment);

        }

    }
}
