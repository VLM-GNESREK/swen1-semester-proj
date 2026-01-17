// Controllers/UserController.cs

using System.Net;
using System.Linq;
using Newtonsoft.Json;
using BCrypt.Net;
using Treasure_Bay.Classes;
using Treasure_Bay.DTO;
using Treasure_Bay.Services;
using System.Collections;
using Npgsql;

namespace Treasure_Bay.Controllers
{
    public class UserLoginRequest // Helper Class
    {
        // ## PROPERTIES ##
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class FavouriteRequest // Helper Class
    {
        // ## PROPERTIES ##
        public int? MediaID { get; set; }
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
            User? currentUser = null;
            var method = req.HttpMethod;
            string path = req.Url?.AbsolutePath.ToLower() ?? "/";
            string requestBody = "";

            if (!path.EndsWith("/login") && !path.EndsWith("/register"))
            {
                currentUser = Authenticate(req);
                if (currentUser == null)
                {
                    await SendResponseAsync(resp, "Error 401: Unauthorised", 401); 
                    return;
                }
            }

            try
            {
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
                    case "/api/users/favorites":
                    case "/api/users/favourites":
                        if (method == "GET")
                        {
                            var favs = _userService.GetFavourites(currentUser!.UserID);
                            string jsonResponse = JsonConvert.SerializeObject(favs);
                            await SendResponseAsync(resp, jsonResponse, 200, "application/json");
                        }
                        else if(method == "POST")
                        {
                            using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                            {
                                requestBody = await reader.ReadToEndAsync();
                            }

                            try 
                            {
                                var favData = JsonConvert.DeserializeObject<FavouriteRequest>(requestBody);

                                if (favData == null || favData.MediaID == null || favData.MediaID <= 0)
                                {
                                    await SendResponseAsync(resp, "Error 400: Invalid Media ID provided.", 400);
                                    return;
                                }
                                try
                                {
                                    _userService.AddFavourite(currentUser!.UserID, favData.MediaID!.Value);
                                    await SendResponseAsync(resp, "Favorite added successfully.", 201);
                                }
                                catch(PostgresException ex) when (ex.SqlState == "23503")
                                {
                                    await SendResponseAsync(resp, $"Error 404: Media not found.", 404);
                                }
                                catch(PostgresException ex) when (ex.SqlState == "23505")
                                {
                                    await SendResponseAsync(resp, $"Error 409: Already in favourites.", 409);
                                }
                            }
                            catch (JsonException)
                            {
                                await SendResponseAsync(resp, "Error 400: Malformed JSON.", 400);
                            }
                        }
                        else if(method == "DELETE")
                        {
                            await SendResponseAsync(resp, "Error 405: Use DELETE /api/users/favourites/{id} instead", 405);
                        }
                        else
                        {
                            await SendResponseAsync(resp, "Error 405: Method not allowed.", 405);
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
                        if(path.StartsWith("/api/users/favourites/") && method == "DELETE")
                        {
                            string lastSegment = path.Split('/').Last();
                            if(int.TryParse(lastSegment, out int mediaID))
                            {
                                _userService.RemoveFavourite(currentUser!.UserID, mediaID);
                                await SendResponseAsync(resp, "Removed from favourites.", 200);
                            }
                            else
                            {
                                await SendResponseAsync(resp, "Error 400: Invalid ID format.", 400);
                            }
                        }
                        else
                        {
                            await SendResponseAsync(resp, $"Error 404: Unknown User API endpoint.", 404);
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine($"U/ERROR handling request: {ex.Message}");
                System.Console.WriteLine(ex.StackTrace);
                await SendResponseAsync(resp, $"Error 500: Internal Server Error", 500);
            }
        }
    }
}