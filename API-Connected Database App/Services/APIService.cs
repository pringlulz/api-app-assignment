using API_Connected_Database_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace API_Connected_Database_App.Services
{
    public class APIService
    //no more than one request per second
    //endpoints:
    // posts.json
    //TODO: rate limiter
    {
        private Uri base_address = new Uri("https://e926.net");
        private static HttpClient client;

        //Store the key outside the project so as not to expose it.
        private String key_location = "C:\\Users\\zmey\\Documents\\e926key.txt";
        private String apiKey;

        private static DateTime lastRequest; //keep track of the last request by the API, if we try and do more than one per second, cancel the action.

        public APIService()
        {
            client = new HttpClient();
            client.BaseAddress = base_address;
            using (StreamReader reader = new(key_location)) {
                apiKey = reader.ReadToEnd();
            } 
            if (String.IsNullOrEmpty(apiKey))
            {
                throw new Exception("Couldn't read API key!");
            }
            //System.Diagnostics.Debug.WriteLine($"apiKey: {apiKey}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(apiKey)));
            client.DefaultRequestHeaders.Add("User-Agent", $"ClassProject/0.1 (by {apiKey.Split(":")[0]})");
            lastRequest = DateTime.Now;

        }

        private void test()
        {
      
            String tagString = new TagStringBuilder().addScoreFilter(Compare.Greater_Than_Or_Equal, 100)
                                .addTagList(new List<String>{ "chikn_nuggit"})
                                .Build();
           
            Dictionary<String, string> queryParameters = new Dictionary<string, string>();
            queryParameters.Add("tags", tagString);
            
            GetAsync(queryParameters);

        }

        static async Task<HttpResponseMessage> DoRequestAsync( String requestString)
        {
            if ((DateTime.Now - lastRequest).TotalSeconds < 1)
            {
                System.Diagnostics.Debug.WriteLine("Too many requests! Slow down!");
                Thread.Sleep(1500);
            } 
            
            lastRequest = DateTime.Now;
            return await client.GetAsync(requestString);
            
        }

        public static async Task<List<Post>> GetAsync(Dictionary<String,string> queryParameters)
        {
            HttpRequestMessage message = new HttpRequestMessage();
            //snafu: different keys should be url-encoded, 
            var urlEncoded = String.Join("&", queryParameters.Select(kvp => kvp.Key + "=" + kvp.Value)); //assume value has already been url-encoded by other stuff
            //var urlEncoded = await new FormUrlEncodedContent(queryParameters).ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"result: {urlEncoded}");
            using (HttpResponseMessage response = await DoRequestAsync(String.Join("&", "posts.json?limit=10", urlEncoded)))
            {
                return await ProcessPostResponse(response);
            }
            
        }
        static async Task<List<Post>> ProcessPostResponse(HttpResponseMessage response)
        {
            //returns post array of format {posts: [{},{}...]}
            System.Diagnostics.Debug.WriteLine(await response.Content.ReadAsStringAsync());
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            using JsonDocument jsonResponse = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            //var dictResponse = await response.Content.ReadFromJsonAsync<Dictionary<String, String>>();
            System.Diagnostics.Debug.WriteLine("raw text" + jsonResponse.RootElement.GetProperty("posts").GetRawText());
            var posts = JsonSerializer.Deserialize<List<Post>>(jsonResponse.RootElement.GetProperty("posts").GetRawText());

            posts.ForEach(p => System.Diagnostics.Debug.WriteLine(p.ToString()));

            return posts;
        }


    }
}
