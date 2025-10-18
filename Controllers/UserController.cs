// Controllers/UserController.cs

using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Controllers
{
    public class UserLoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class UserController
    {
        private static List<User> userDatabase = new List<User>();

        // ## METHODS ##

        private async Task SendResponseAsync(HttpListenerResponse resp, string body, int statusCode)
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            resp.StatusCode = statusCode;
            resp.ContentType = "text/plain; charset=utf-8";
            resp.ContentLength64 = bytes.Length;
            await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            resp.Close();
        }
        public async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var method = req.HttpMethod;
            var path = req.Url?.AbsolutePath ?? "/";
            string requestBody = "";
            switch (path)
            {
                case "/api/users/login":
                    switch (method)
                    {
                        case "POST":
                            using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                            {
                                requestBody = await reader.ReadToEndAsync();
                            }
                            var loginData = JsonConvert.DeserializeObject<UserLoginRequest>(requestBody);
                            if (loginData == null || string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password))
                            {
                                await SendResponseAsync(resp, $"Error 400: Username or password missing or malformed.", 400);
                            }
                            else
                            {
                                await SendResponseAsync(resp, $"Login attempt for user: {loginData.Username} with password: {loginData.Password}", 200);
                            }
                            break;
                        default:
                            await SendResponseAsync(resp, $"Error 405: Method not allowed!", 405);
                            break;
                    }
                    break;
                case "/api/users/register":
                    switch (method)
                    {
                        case "POST":
                            using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                            {
                                requestBody = await reader.ReadToEndAsync();
                            }
                            var loginData = JsonConvert.DeserializeObject<UserLoginRequest>(requestBody);
                            if (loginData == null || string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password))
                            {
                                await SendResponseAsync(resp, $"Error 400: Username or password missing or malformed.", 400);
                            }
                            else
                            {
                                await SendResponseAsync(resp, $"Attempt to register user: {loginData.Username} with password: {loginData.Password}", 200);
                            }
                            break;
                        default:
                            await SendResponseAsync(resp, $"Error 405: Method not allowed!", 405);
                            break;
                    }
                    break;
                default:
                    await SendResponseAsync(resp, $"Error 404: Unknown User API endpoint.", 404);
                    break;
            }
        }
    }
}