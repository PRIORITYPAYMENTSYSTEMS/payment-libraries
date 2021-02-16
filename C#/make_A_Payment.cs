using System;
using RestSharp;

namespace GitHub_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient("https://sandbox.api.mxmerchant.com/checkout/v3/payment?echo=true");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Basic e3tVc2VybmFtZX19Ont7UGFzc3dvcmR9fQ==");
            request.AddParameter("application/json", "{\n\t\"merchantId\": \"516158976\",\n\t\"amount\": 45.00,\n\t\"paymentType\": \"sale\",\n\t\"tenderType\": \"cash\"\n\t}\n}\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }
    }
}
