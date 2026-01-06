// HttpServer.cs

using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Treasure_Bay.Controllers;

public class HttpServer
{
    
    // ## VARIABLES AND COLLECTIONS ##

    private HttpListener _listener;
    private UserController _userController;
    private MediaController _mediaController;
    private RatingController _ratingController;
    private Dictionary<string, BaseController> _routes;

    // ## METHODS ##
    // Constructor

    public HttpServer(string url, UserController userController, MediaController mediaController, RatingController ratingController)
    {
        
        this._userController = userController;
        this._mediaController = mediaController;
        this._ratingController = ratingController;
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
        _routes = new Dictionary<string, BaseController>();
        _routes["/api/users"] = userController;
        _routes["/api/media"] = mediaController;
        _routes["/api/ratings"] = ratingController;
    }

    // Server Initialisation

    public async Task Start()
    {
        _listener.Start();
        System.Console.WriteLine("Started Server. Listening...");
        while (true)
        {
            var res = await _listener.GetContextAsync();
            _ = Task.Run(async () =>
            {
                var req = res.Request;
                var resp = res.Response;
                try
                {
                    var type = req.HttpMethod;
                    var path = req.Url?.AbsolutePath ?? "/";
                    System.Console.WriteLine($"Received {type} request for {path}");
                    var matchingRoute = _routes.FirstOrDefault(route => path.StartsWith(route.Key));

                    if(matchingRoute.Value != null)
                    {
                        await matchingRoute.Value.HandleRequest(req, resp);
                    }
                    else
                    {
                        resp.StatusCode = 404;
                        string body = "Error 404: Page not found";
                        var bytes3 = Encoding.UTF8.GetBytes(body);
                        resp.ContentType = "text/plain; charset=utf-8";
                        resp.ContentLength64 = bytes3.Length;
                        await resp.OutputStream.WriteAsync(bytes3, 0, bytes3.Length);
                        resp.Close();
                    }
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine($"CRITICAL ERROR handling request: {ex.Message}");
                    System.Console.WriteLine(ex.StackTrace);

                    try
                    {
                        resp.StatusCode = 500;
                        resp.Close();
                    }
                    catch
                    {}
                }
            });
        }
    }
}