using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Treasure_Bay.Controllers;

public class HttpServer
{
    
    // ## VARIABLES ##

    private HttpListener _listener;
    private UserController _userController;
    private MediaController _mediaController;

    // ## METHODS ##
    // Constructor

    public HttpServer(string url, UserController userController, MediaController mediaController)
    {
        
        this._userController = userController;
        this._mediaController = mediaController;
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
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
                var type = req.HttpMethod;
                var path = req.Url?.AbsolutePath ?? "/";
                System.Console.WriteLine($"Received {type} request for {path}"); // Debug
                var body = $"Shiny String";

                switch(path) 
                {
                    case "/test":
                        resp.StatusCode = 200;
                        body = $"This is the {path} page.";
                        var bytes = Encoding.UTF8.GetBytes(body);
                        resp.ContentType = "text/plain; charset=utf-8";
                        resp.ContentLength64 = bytes.Length;
                        await resp.OutputStream.WriteAsync(bytes, 0, bytes.Length);
                        resp.Close();
                        break;
                    default: 
                        if (path.StartsWith("/api/users"))
                        {
                            await _userController.HandleRequest(req, resp);
                        }
                        else if(path.StartsWith("/api/media"))
                        {
                            await _mediaController.HandleRequest(req, resp);
                        }
                        else
                        {
                            resp.StatusCode = 404;
                            body = "Error 404: Page not found";
                            var bytes3 = Encoding.UTF8.GetBytes(body);
                            resp.ContentType = "text/plain; charset=utf-8";
                            resp.ContentLength64 = bytes3.Length;
                            await resp.OutputStream.WriteAsync(bytes3, 0, bytes3.Length);
                            resp.Close();
                        }
                        break;
                }
            });
        }
    }
}