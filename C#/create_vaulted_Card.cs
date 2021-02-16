using System;
using RestSharp;

namespace GitHub_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://sandbox.api.mxmerchant.com/checkout/v3/customercardaccount?id=10000000787686&echo=true");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QVBJVmlkZW9HYW1lczpSYWNlQ2FyczEyMyE=");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\t\"code\": \"card\",\n\t\"number\": \"4242424242424242\",\n\t\"expiryMonth\": \"12\",\n\t\"expiryYear\": \"2022\",\n\t\"avsZip\": \"30004\",\n\t\"avsStreet\": \"2001 Westside Parkway\",\n\t\"cvv\": \"211\",\n\t\"name\": \"Traci Edmund's Card\"\n}\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
