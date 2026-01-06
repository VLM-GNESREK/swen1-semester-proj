// Controllers/RatingController.cs

using System.Net;
using Newtonsoft.Json;
using Treasure_Bay.Classes;
using Treasure_Bay.Services;

namespace Treasure_Bay.Controllers
{
    public class RatingCreateRequest
    {
        public int stars { get; set; }
        public string? comment { get; set; }
        public int media_id { get; set; }
    }
    public class RatingController : BaseController
    {
        
        // ## VARIABLES ##

        private RatingService _ratingService;
        private MediaService _mediaService;

        // ## METHODS ##

        public RatingController(RatingService ratingService, MediaService mediaService, AuthService authService) : base(authService)
        {
            _ratingService = ratingService;
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
                case "/api/ratings":
                    if (method == "POST")
                    {
                        using(var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                        {
                            requestBody = await reader.ReadToEndAsync();
                        }

                        var ratingData = JsonConvert.DeserializeObject<RatingCreateRequest>(requestBody);

                        if (ratingData == null || ratingData.media_id == 0 || ratingData.stars == 0)
                        {
                            await SendResponseAsync(resp, "Error 400: Rating and valid Media ID are required.", 400);
                            break;
                        }

                        MediaEntry? media = _mediaService.FindMedia(ratingData.media_id);

                        if (media == null)
                        {
                            await SendResponseAsync(resp, "Error 404: Media not found.", 404);
                            break;
                        }

                        try
                        {
                            var newRating = _ratingService.CreateRating(user, media, ratingData.stars, ratingData.comment ?? "");
                            
                            string jsonResponse = JsonConvert.SerializeObject(newRating);
                            await SendResponseAsync(resp, jsonResponse, 201, "application/json");
                        }
                        catch (InvalidOperationException ex)
                        {
                            await SendResponseAsync(resp, $"Error 409: {ex.Message}", 409);
                        }
                    }
                    else if(method == "GET")
                    {
                        var topParam = req.QueryString["top"];
                        if(topParam != null && int.TryParse(topParam, out int count))
                        {
                            List<MediaEntry> topList = _ratingService.GetTopRatedMedia(_mediaService.GetAllMedia(), count);

                            string jsonResponse = JsonConvert.SerializeObject(topList);
                            await SendResponseAsync(resp, jsonResponse, 200, "application/json");
                            break;
                        }

                        var mediaIDParam = req.QueryString["mediaID"];
                        if(mediaIDParam != null && int.TryParse(mediaIDParam, out int mediaID))
                        {
                            MediaEntry? media = _mediaService.FindMedia(mediaID);
                            if(media == null)
                            {
                                await SendResponseAsync(resp, "Error 404: Media not found.", 404);
                                break;
                            }

                            var ratings = _ratingService.GetRatingsByMedia(media);
                            string jsonResponse = JsonConvert.SerializeObject(ratings);
                            await SendResponseAsync(resp, jsonResponse, 200, "application/json");
                            break;
                        }

                        await SendResponseAsync(resp, "Error 400: Specify 'top' or 'mediaID' parameter.", 400);
                    }
                    break;
            }
        }
    }
}