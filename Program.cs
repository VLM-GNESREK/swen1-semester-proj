// Program.cs

using Treasure_Bay.Controllers;
using Treasure_Bay.Classes;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Treasure_Bay.Services;
using Treasure_Bay.Repositories;

class Program
{
    static async Task Main(string[] args)
    {
        DataBaseSetup.InitialiseDatabase();
        string serverURL = "http://localhost:8080/";
        IUserRepository userRepository = new UserRepository();
        IMediaRepository mediaRepository = new MediaRepository();
        IRatingRepository ratingRepository = new RatingRepository();
        UserService userService = new UserService(userRepository);
        AuthService authService = new AuthService();
        MediaService mediaService = new MediaService(mediaRepository);
        RatingService ratingService = new RatingService(ratingRepository);
        UserController userController = new UserController(userService, authService);
        MediaController mediaController = new MediaController(mediaService, authService);
        RatingController ratingController = new RatingController(ratingService, mediaService, authService);
        HttpServer origin = new HttpServer(serverURL, userController, mediaController, ratingController);
        await origin.Start();
    }
}