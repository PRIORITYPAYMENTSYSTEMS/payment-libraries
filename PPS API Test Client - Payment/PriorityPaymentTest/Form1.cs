using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace PriorityPaymentTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        public static string baseURL = "https://sandbox.api.mxmerchant.com/checkout/v3";

        public static readonly string endPointRequestToken = baseURL + "/oauth/1a/requesttoken";
        public static readonly string endPointAccessToken = baseURL + "/oauth/1a/accesstoken";
        public static readonly string createPayment = baseURL + "/payment";

        public static readonly string getPayment = baseURL + "/payment/{0}";
        public static readonly string getPayments = baseURL + "/payment";

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            string ConsumerKey = System.Configuration.ConfigurationManager.AppSettings["key"];
            string ConsumerSecret = System.Configuration.ConfigurationManager.AppSettings["secret"];

            var queryString = new Dictionary<string, string>();
            queryString.Add("echo", "true");

            var p = new Payment();
            p.merchantId = System.Configuration.ConfigurationManager.AppSettings["merchantid"];
            p.tenderType = "Card";
            p.amount = "0.01";
            p.cardAccount = new CardAccount();
            p.cardAccount.number = "{{INSERT}}";
            p.cardAccount.expiryMonth = "{{INSERT}}";
            p.cardAccount.expiryYear = "{{INSERT}}";
            p.cardAccount.cvv = "{{INSERT}}";
            p.cardAccount.avsZip = "{{INSERT}}";
            p.cardAccount.avsStreet = "{{INSERT}}";

            PpsApiRequest apiRequest = new PpsApiRequest(baseURL, ConsumerKey, ConsumerSecret, AuthenticationMethod.OAuth);
            using (var httpRequest = apiRequest.BuildRequest(createPayment, queryString, System.Net.Http.HttpMethod.Post, p))
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = httpClient.SendAsync(httpRequest).Result;

                var json = response.Content.ReadAsStringAsync().Result;
                textBox1.Text = json;
            }

            Cursor.Current = Cursors.Default;

        }
        
    }
}



