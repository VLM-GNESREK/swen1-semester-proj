// Controllers/BaseController.cs

using System.Net;
using System.Text;
using System.Collections.Generic;
using Treasure_Bay.Classes;
using Treasure_Bay.Services;

namespace Treasure_Bay.Controllers
{
    public class BaseController
    {
        // ## COMMON VARIABLES ##

        protected AuthService _authService;

        // ## COMMON METHODS ##

        public BaseController(AuthService authService)
        {
            _authService = authService;
        }

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
            return _authService.GetUserByToken(token);
        }

        public virtual async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            await SendResponseAsync(resp, "Error 404: Responsible Controller not found.", 404);
        }
    }
}