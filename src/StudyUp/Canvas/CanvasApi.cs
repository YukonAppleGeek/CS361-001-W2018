using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace StudyUp.Canvas
{
    public static class CanvasApi {
        private static HttpClient _client;
        private static HttpClient client {
            get {
                if (_client == null) {
                    _client = new HttpClient() {
                        BaseAddress = new Uri("http://oregonstate.instructure.com/api/")
                    };

                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                return _client;
            }
        }

        private static HttpRequestMessage CreateTokenRequest(string requestUri, string token) {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(client.BaseAddress, requestUri)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return request;
        }

        private static async Task<CanvasApiException> CreateApiException(HttpResponseMessage response) {
            if ((int)response.StatusCode >= 500) return new CanvasApiException(response.StatusCode, null);

            var content = await response.Content.ReadAsStringAsync();
            return new CanvasApiException(response.StatusCode, JObject.Parse(content));
        }

        public static async Task<JObject> GetUserInfo(string token) {
            var response = await client.SendAsync(CreateTokenRequest("v1/users/self", token));
            
            if (!response.IsSuccessStatusCode) {
                throw await CreateApiException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        public static async Task<JArray> GetUserCourses(string token) {
            // Returns first 75 results, headers should be checked to see if more pages exist
            var response = await client.SendAsync(CreateTokenRequest("v1/courses?include[]=term&per_page=75", token));

            if (!response.IsSuccessStatusCode)
            {
                throw await CreateApiException(response);
            }

            var content = await response.Content.ReadAsStringAsync();
            return JArray.Parse(content);
        }
    }
}