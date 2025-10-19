// Controllers/BaseController.cs

using System.Net;
using System.Text;
using System.Collections.Generic;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Controllers
{
    public class BaseController
    {
        // ## COMMON COLLECTIONS ##
        protected static Dictionary<string, User> tokenDatabase = new Dictionary<string, User>();

        // ## COMMON METHODS ##

        protected async Task SendResponseAsync(HttpListenerResponse resp, string body, int statusCode, string contentType = "text/plain; charset=utf-8")
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            resp.StatusCode = statusCode;
            resp.ContentType = contentType;
            resp.ContentLength64 = bytes.Length;
            await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            resp.Close();
        }
        protected User? Authenticate(HttpListenerRequest req)
        {
            string? authHeader = req.Headers["Authorization"];
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                return null;
            }
            string token = authHeader.Substring(7);
            if (tokenDatabase.TryGetValue(token, out User? user))
            {
                return user;
            }
            return null;
        }
    }
}