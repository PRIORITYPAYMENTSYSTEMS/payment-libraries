using System;
using RestSharp;

namespace GitHub_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://sandbox.api.mxmerchant.com/checkout/v3/payment?merchantId={{MerchantId}}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Basic QVBJVmlkZW9HYW1lczpSYWNlQ2FyczEyMyE=");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
