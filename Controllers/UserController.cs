// Controllers/UserController.cs

using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using BCrypt.Net;
using Treasure_Bay.Classes;
using System.Runtime.InteropServices;

namespace Treasure_Bay.Controllers
{
    public class UserLoginRequest
    {
        // ## PROPERTIES ##
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class UserController
    {
        // ## COLLECTIONS ##
        private static List<User> userDatabase = new List<User>();
        private static Dictionary<string, User> tokenDatabase = new Dictionary<string, User>();

        // ## METHODS ##

        private async Task SendResponseAsync(HttpListenerResponse resp, string body, int statusCode, string contentType = "text/plain; charset=utf-8")
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            resp.StatusCode = statusCode;
            resp.ContentType = contentType;
            resp.ContentLength64 = bytes.Length;
            await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
            resp.Close();
        }
        private User? Authenticate(HttpListenerRequest req)
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
                                var user = userDatabase.FirstOrDefault(u => u.Username == loginData.Username);
                                if (user == null || !BCrypt.Net.BCrypt.Verify(loginData.Password, user.PasswordHash))
                                {
                                    await SendResponseAsync(resp, $"Error 401: Invalid username or password.", 401);
                                }
                                else
                                {
                                    string token = $"{user.Username}-mrpToken";
                                    tokenDatabase[token] = user;
                                    await SendResponseAsync(resp, token, 200);
                                }
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
                                if (userDatabase.Any(u => u.Username == loginData.Username))
                                {
                                    await SendResponseAsync(resp, $"Error 409: User with this username already exists.", 409);
                                }
                                else
                                {
                                    int newID = userDatabase.Any() ? userDatabase.Max(u => u.UserID) + 1 : 1;
                                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginData.Password);
                                    User newUser = new User(loginData.Username, newID, hashedPassword);
                                    userDatabase.Add(newUser);

                                    string jsonResponse = JsonConvert.SerializeObject(newUser);
                                    await SendResponseAsync(resp, jsonResponse, 201, "application/json; charset=utf-8");
                                }
                            }
                            break;
                        default:
                            await SendResponseAsync(resp, $"Error 405: Method not allowed!", 405);
                            break;
                    }
                    break;
                case "/api/users/profile":
                    if (method == "GET")
                    {
                        User? user = Authenticate(req);
                        if (user == null)
                        {
                            await SendResponseAsync(resp, $"Error 401: Unauthorised. Missing or invalid token", 401);
                        }
                        else
                        {
                            await SendResponseAsync(resp, $"Authenticated as: {user.Username}", 200);
                        }
                    }
                    else
                    {
                        await SendResponseAsync(resp, $"Error 405: Method not allowed.", 405);
                    }
                    break;
                default:
                    await SendResponseAsync(resp, $"Error 404: Unknown User API endpoint.", 404);
                    break;
            }
        }
    }
}