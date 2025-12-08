// Controllers/MediaController.cs

using System.Collections.Generic;
using System.Net;
using System.Linq;
using Newtonsoft.Json;
using Treasure_Bay.Classes;
using Treasure_Bay.Services;

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

        // ## VARIABLES ##

        private MediaService _mediaService;

        // ## METHODS ##

        public MediaController(MediaService mediaService, AuthService authService) : base(authService)
        {
            _mediaService = mediaService;
        }

        public override async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
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

                        var newMedia = _mediaService.CreateMedia(mediaData.Title, mediaData.Description, mediaData.ReleaseYear, user);
                        string jsonResponse = JsonConvert.SerializeObject(newMedia);
                        await SendResponseAsync(resp, jsonResponse, 201, "application/json; charset=utf-8");
                    }
                    else if (method == "GET") // Reading
                    {
                        string jsonResponse = JsonConvert.SerializeObject(_mediaService.GetAllMedia());
                        await SendResponseAsync(resp, jsonResponse, 200, "application/json; charset=utf-8");
                    }
                    else
                    {
                        await SendResponseAsync(resp, $"Error 405: Method not allowed.", 405);
                    }
                    break;
                default:
                    if (path.StartsWith("/api/media/"))
                    {
                        string? idSegment = req.Url?.Segments.Last().TrimEnd('/');
                        if (int.TryParse(idSegment, out int mediaID))
                        {
                            var media = _mediaService.FindMedia(mediaID);

                            if (media == null)
                            {
                                await SendResponseAsync(resp, $"Error 404: Media could not be found.", 404);
                                break;
                            }

                            switch (method)
                            {
                                case "GET":
                                    string jsonResponse = JsonConvert.SerializeObject(media);
                                    await SendResponseAsync(resp, jsonResponse, 200, "application/json; charset=utf-8");
                                    break;
                                case "DELETE": // Deletion
                                    if (media.Creator.UserID != user.UserID)
                                    {
                                        await SendResponseAsync(resp, $"Error 403: You are not authorised to delete another user's media.", 403);
                                    }
                                    else
                                    {
                                        _mediaService.DeleteMedia(media.MediaID);
                                        await SendResponseAsync(resp, "", 204);
                                    }
                                    break;
                                case "PUT": // Updating
                                    if (media.Creator.UserID != user.UserID)
                                    {
                                        await SendResponseAsync(resp, $"Error 403: You are not authorised to alter another user's media.", 403);
                                        break;
                                    }

                                    using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                                    {
                                        requestBody = await reader.ReadToEndAsync();
                                    }
                                    var mediaData = JsonConvert.DeserializeObject<MediaCreateRequest>(requestBody);

                                    if (mediaData == null || string.IsNullOrWhiteSpace(mediaData.Title))
                                    {
                                        await SendResponseAsync(resp, $"Error 400: Title is required.", 400);
                                        break;
                                    }

                                    media.Title = mediaData.Title;
                                    media.Description = mediaData.Description ?? media.Description;
                                    media.ReleaseYear = mediaData.ReleaseYear;
                                    _mediaService.UpdateMedia(media);
                                    string updatedJsonResponse = JsonConvert.SerializeObject(media);
                                    await SendResponseAsync(resp, updatedJsonResponse, 200, "application/json; charset=utf-8");
                                    break;
                                default:
                                    await SendResponseAsync(resp, $"Error 405: Method not allowed.", 405);
                                    break;
                            }
                        }
                        else
                        {
                            await SendResponseAsync(resp, $"Error 400: Invlid media ID.", 400);
                        }
                    }
                    else
                    {
                        await SendResponseAsync(resp, $"Error 404: Media endpoint not found.", 404);
                    }
                    break;
            }
        }
    }
}