// Controllers/MediaController.cs

using System.Collections.Generic;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using Treasure_Bay.Classes;

namespace Treasure_Bay.Controllers
{
    public class MediaCreateRequest // Helper Class
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }
    }
    public class MediaController : BaseController
    {
        private static List<MediaEntry> mediaDatabse = new List<MediaEntry>();
        public async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            User? user = Authenticate(req);

            if (user == null)
            {
                await SendResponseAsync(resp, $"Error 401: Unauthorised.", 401);
                return;
            }

            var method = req.HttpMethod;
            var path = req.Url?.AbsolutePath ?? "/";
            string requestBody = "";

            switch(path)
            {
                case "/api/media":
                    if (method == "POST") // Creation
                    {
                        using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                        {
                            requestBody = await reader.ReadToEndAsync();
                        }

                        var mediaData = JsonConvert.DeserializeObject<MediaCreateRequest>(requestBody);

                        if (mediaData == null || string.IsNullOrWhiteSpace(mediaData.Title) || string.IsNullOrWhiteSpace(mediaData.Description))
                        {
                            await SendResponseAsync(resp, $"Error 400: Title and description are required.", 400);
                            break;
                        }

                        int newID = mediaDatabse.Any() ? mediaDatabse.Max(m => m.MediaID) + 1 : 1;
                        MediaEntry newMedia = new MediaEntry(newID, mediaData.Title, mediaData.Description, mediaData.ReleaseYear, user);
                        mediaDatabse.Add(newMedia);
                        string jsonResponse = JsonConvert.SerializeObject(newMedia);
                        await SendResponseAsync(resp, jsonResponse, 201, "application/json; charset=utf-8");
                    }
                    else if (method == "GET")
                    {
                        string jsonResponse = JsonConvert.SerializeObject(mediaDatabse);
                        await SendResponseAsync(resp, jsonResponse, 200, "application/json; charset=utf-8");
                    }
                    else
                    {
                        await SendResponseAsync(resp, $"Error 405: Method not allowed.", 405);
                    }
                    break;
                default:
                    await SendResponseAsync(resp, $"Error 404: Media Endpoint not found.", 404);
                    break;
            }
        }
    }
}