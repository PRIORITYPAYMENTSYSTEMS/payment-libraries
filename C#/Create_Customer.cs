using System;
using RestSharp;

namespace GitHub_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://sandbox.api.mxmerchant.com/checkout/v3/customer?echo=true");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic QVBJVmlkZW9HYW1lczpSYWNlQ2FyczEyMyE=");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\n\t\"merchantId\": \"merchantId\",\n\t\"name\": \"Johnny Appleseed\",\n\t\"firstName\": \"Johnny\",\n\t\"lastName\": \"Appleseed\",\n\t\"address1\": \"2001 Westside Parkway\",\n\t\"city\": \"Alpharetta\",\n\t\"state\": \"Georgia\",\n\t\"zip\": \"30004\"\n}\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
