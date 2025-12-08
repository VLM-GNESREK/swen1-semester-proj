// Controllers/UserController.cs

using System.Net;
using System.Linq;
using Newtonsoft.Json;
using BCrypt.Net;
using Treasure_Bay.Classes;
using Treasure_Bay.DTO;
using Treasure_Bay.Services;

namespace Treasure_Bay.Controllers
{
    public class UserLoginRequest // Helper Class
    {
        // ## PROPERTIES ##
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class UserController : BaseController
    {

        private readonly UserService _userService;

        // ## METHODS ##

        public UserController(UserService userService, AuthService authService) : base(authService)
        {
            _userService = userService;
        }

        public override async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
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
                                var user = _userService.Login(loginData.Username, loginData.Password);
                                if (user == null)
                                {
                                    await SendResponseAsync(resp, $"Error 401: Invalid username or password.", 401);
                                }
                                else
                                {
                                    string token = _authService.GenerateToken(user);
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
                                if (_userService.QueryUserExists(loginData.Username))
                                {
                                    await SendResponseAsync(resp, $"Error 409: User with this username already exists.", 409);
                                }
                                else
                                {
                                    UserResponseDTO userResponse = _userService.Register(loginData.Username, loginData.Password);
                                    string jsonResponse = JsonConvert.SerializeObject(userResponse);
                                    await SendResponseAsync(resp, jsonResponse, 201, "application/json; charset=utf-8");
                                }
                            }
                            break;
                        default:
                            await SendResponseAsync(resp, $"Error 405: Method not allowed.", 405);
                            break;
                    }
                    break;
                case "/api/users/profile":
                    if (method == "GET")
                    {
                        User? user = Authenticate(req);
                        if (user == null)
                        {
                            await SendResponseAsync(resp, $"Error 401: Unauthorised. Missing or invalid token.", 401);
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