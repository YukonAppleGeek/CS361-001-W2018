using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace StudyUp.Canvas
{
    public class CanvasApiException : HttpRequestException
    {
        public HttpStatusCode StatusCode { get; }
        public JObject Response { get; }

        public CanvasApiException() : base() {}

        public CanvasApiException(HttpStatusCode statusCode, JObject response) : base() {
            StatusCode = statusCode;
            Response = response;
        }
    }
}
