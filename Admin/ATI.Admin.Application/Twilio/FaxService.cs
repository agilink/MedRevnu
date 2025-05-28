using ATI.Configuration;
using Newtonsoft.Json;
using RestSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using Org.BouncyCastle.Tls.Crypto;
using ATI.Admin.Application;
using System.Web;

namespace ATI.Admin.Application
{
    public class FaxService : ATIAppServiceBase, IFaxService
    {

        private readonly IAppConfigurationAccessor _appConfigurationAccessor;
        private static readonly HttpClient client = new HttpClient();

        public FaxService(IAppConfigurationAccessor appConfigurationAccessor)
        {
            _appConfigurationAccessor = appConfigurationAccessor;
        }

        //Send fax using SFax
        public async Task SendAsync(byte[] data)
        {
            try
            {
                var apiUrl = _appConfigurationAccessor.Configuration["SFax:ApiUrl"];
                var apiKey = _appConfigurationAccessor.Configuration["SFax:ApiKey"];
                var encryptionKey = _appConfigurationAccessor.Configuration["SFax:EncryptionKey"];
                var initVector = _appConfigurationAccessor.Configuration["SFax:InitVector"];
                var userName = _appConfigurationAccessor.Configuration["SFax:UserName"];
                var toFax = _appConfigurationAccessor.Configuration["SFax:RecipientFax"];

                try
                {
                    //SendFax 
                    var message = new HttpRequestMessage();
                    var content = new MultipartFormDataContent();
                    string xst = AESCrypto.GenerateSecurityTokenUrl(
                      userName, apiKey, encryptionKey, initVector, false);//Use Sfax AESCrypto class included in this documentation


                    content.Add(new ByteArrayContent(data), "file", "prescription.pdf");

                    //construct URL
                    string serviceEndpointUrl = "https://api.sfaxme.com/api/";
                    string methodSignature = "SendFax";
                    string token = xst;
                    string recipientName = "FM Main Fax";
                    string coverpage1 = "Default";
                    string subject = "Test";
                    string reference = "Test1234";
                    string trackingid = "1234";
                    // Construct the base service URL endpoint
                    string url = string.Concat
                    (
                        serviceEndpointUrl,
                        HttpUtility.UrlEncode(methodSignature),
                        "?",
                        "token=", HttpUtility.UrlEncode(token),
                        "&apikey=", HttpUtility.UrlEncode(apiKey),
                        // Add the method specific parameters 
                        "&RecipientName=", HttpUtility.UrlEncode(recipientName),
                        "&RecipientFax=", HttpUtility.UrlEncode(toFax)
                        //,
                        //"&OptionalParams=" + "CoverPageName=", HttpUtility.UrlEncode(coverpage1) + ";" 
                        //+ "CoverPageSubject=", HttpUtility.UrlEncode(subject) + ";" + "CoverPageReference=", HttpUtility.UrlEncode(reference) + ";" + "TrackingCode=", HttpUtility.UrlEncode(trackingid)
                    );
                    url = url + "&"; //end of url
                    Console.WriteLine("URL: " + url);
                    //construct and make call
                    message.Method = HttpMethod.Post;
                    message.Content = content;
                    message.RequestUri = new Uri(url);
                    //get the response back in XML instead of JSON use the line below that is commented out
                    //message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
                    var client = new HttpClient()
                    {
                        Timeout = TimeSpan.FromSeconds(300000)
                    };
                    var task = client.SendAsync(message).ContinueWith((t) => ResponseFinished(t.Result));
                    task.Wait();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                throw;
            }
        }
        private void ResponseFinished(HttpResponseMessage response)
        {
            //get response message
            var task = response.Content.ReadAsStringAsync().ContinueWith<string>(o =>
            {
                return o.Result;
            });
            task.Wait();
            string jsonResponseMessage = task.Result.Remove(0, 19);
            string newjsonResponseMessage = jsonResponseMessage.Remove(32);
            Console.WriteLine(newjsonResponseMessage); ;//Store this SendFaxQueueId to inquire about any issues related to faxes being processed.  To get the status of any pending/completed faxes use ReceiveOutboundFax or use the Fax Callback service.


        }
    }

}


