// Program.cs

using Treasure_Bay.Controllers;
using Treasure_Bay.Classes;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Treasure_Bay.Services;

class Program
{
    static async Task Main(string[] args)
    {
        DataBaseSetup.InitialiseDatabase();
        string serverURL = "http://localhost:8080/";
        UserService userService = new UserService();
        AuthService authService = new AuthService();
        UserController userController = new UserController(userService, authService);
        MediaController mediaController = new MediaController(authService);
        HttpServer origin = new HttpServer(serverURL, userController, mediaController);
        await origin.Start();
    }
}