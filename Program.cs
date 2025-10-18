// Program.cs

using Treasure_Bay.Classes;
using System.Threading;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

class Program
{
    public class UserLoginRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    static async Task Main(string[] args)
    {
        HttpListener origin = new HttpListener();
        origin.Prefixes.Add("http://localhost:8080/");
        origin.Start();
        System.Console.WriteLine("Started Server. Listening at port 8080.");
        while (true)
        {
            var res = await origin.GetContextAsync();
            _ = Task.Run(async () =>
            {
                var req = res.Request;
                var resp = res.Response;
                var type = req.HttpMethod;
                var path = req.Url?.AbsolutePath ?? "/";
                System.Console.WriteLine($"Received {type} request for {path}"); // Debug
                var body = $"Shiny String";
                string requestBody = "";

                switch(path) // Routing Switch
                {
                    case "/test":
                        resp.StatusCode = 200;
                        body = $"This is the {path} page.";
                        break;
                    case "/hello":
                        resp.StatusCode = 200;
                        body = "Hello World!";
                        break;
                    case "/api/users/login":
                        using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                        {
                            requestBody = await reader.ReadToEndAsync();
                        }
                        var loginData = JsonConvert.DeserializeObject<UserLoginRequest>(requestBody);
                        if (loginData == null || string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password))
                        {
                            resp.StatusCode = 400;
                            body = $"Error 400: Username or password missing or malformed.";
                        }
                        else
                        {
                            resp.StatusCode = 200;
                            body = $"Login attempt for user: {loginData.Username} with password: {loginData.Password}";
                        }
                        break;
                    default:
                        resp.StatusCode = 404;
                        body = "Error 404: Page not found";
                        break;
                }
                var bytes = Encoding.UTF8.GetBytes(body);
                resp.ContentType = "text/plain; charset=utf-8";
                resp.ContentLength64 = bytes.Length;
                await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                resp.Close();
            });
        }
        // Legacy Code from first Class Presentation to demonstrate OOP functionalities, pay no heed.
        // TypeFast("###------------------------------###");
        // PromptSlowly("#   Welcome to the Treasure Bay!   #");
        // TypeFast("###------------------------------###");

        // // User Registration (placeholder)

        // PrintSlowly("\nAhoy Matey, be ye new to these parts eh?");
        // PrintSlowly("What's yer name?");
        // System.Console.Write("\n\n");
        // PromptSlowlyEx(">Enter Username: ");
        // string username = Console.ReadLine();
        // User user1 = new User(username, 1, "hashed_password_trust_tm");
        // PrintSlowly($"\n\nThat so {user1.Username}?");
        // PrintSlowly("Well welcome aboard!");
        // PrintSlowly("Ah, but what have ye there?");
        // PrintSlowly("Why yes! It must be a Treasure!");
        // PrintSlowly("What's its name?\n");

        // // Media Registration (placeholder)

        // PromptSlowlyEx(">Enter Media Title: ");
        // string mediaTitle = Console.ReadLine();
        // MediaEntry media = new MediaEntry(42, mediaTitle, "An awesome treasure.", 2025, user1);
        // PrintSlowly($"\n\n{media.Title}, a truly fine name for such a specimen!");
        // PrintSlowly($"Tell me more about this {media.Title} of yours!\n");
        // PromptSlowlyEx(">Description: ");
        // string description = Console.ReadLine();
        // media.Description = description;

        // // Rating stuffu

        // PrintSlowly("\n\nA fine tale! But what's yer final word on it?");
        // PrintSlowly("How many doubloons... eh... 'stars' be it worth? (1-5)");
        // PromptSlowlyEx("\n>Enter Rating (1-5): ");
        // string starString = Console.ReadLine();
        // int stars = int.Parse(starString);

        // // Comment

        // PrintSlowly("\n\nAnd any last words for the ship's log?");
        // PromptSlowlyEx("\n>Enter Comment: ");
        // string comment = Console.ReadLine();
        // user1.PostRating(media, stars, comment); // Post rating
        // PrintSlowly("\nSplendid! We've marked it in the ledger.");
        // PrintSlowly("Let's check the scrolls to be sure...");

        // // Verification

        // TypeFast("\n--- Ledger Verification ---");
        // PrintSlowly($"Captain '{user1.Username}' has {user1.Ratings.Count} entry in their logbook.");
        // PrintSlowly($"The treasure '{media.Title}' has been rated {media.Ratings.Count} time(s).");

        // // Deets
        // PrintSlowly($"There are {media.Ratings.Count} Ratings");
        // foreach (Rating rating in media.Ratings)
        // {
        //     PrintSlowly($"\nThe first entry reads: {rating.StarValue} stars.");
        //     PrintSlowly($"The log says: '{rating.Comment}'");
        //     PrintSlowly($"Signed by: {rating.Reviewer.Username}");
        // }
        // // See yourself out

        // PrintSlowly("\n\nOur business be done here. Press Enter to set sail.");
        // Console.ReadLine();
    }

    // Inactive Legacy Methods for initial OOP demonstration, pay no heed.

    // static void PrintSlowly(string text, int delay = 40)
    // {
    //     foreach (char c in text)
    //     {
    //         Console.Write(c);
    //         Thread.Sleep(delay); // sleeps for 40ms
    //     }
    //     Console.WriteLine(); // ol'reliable \n
    // }

    // static void PromptSlowly(string text, int delay = 20)
    // {
    //     foreach (char c in text)
    //     {
    //         Console.Write(c);
    //         Thread.Sleep(delay); // sleeps for 70ms
    //     }
    //     Console.WriteLine(); // it hasn't changed, you know?
    // }

    // static void TypeFast(string text, int delay = 5)
    // {
    //     foreach (char c in text)
    //     {
    //         Console.Write(c);
    //         Thread.Sleep(delay); // sleeps for 5ms
    //     }
    //     Console.WriteLine(); // still here?
    // }

    // static void PromptSlowlyEx(string text, int delay = 20)
    // {
    //     foreach (char c in text)
    //     {
    //         Console.Write(c);
    //         Thread.Sleep(delay); // sleeps for 20ms
    //     }
    //     // There, I removed it. Happy now?
    // }
}