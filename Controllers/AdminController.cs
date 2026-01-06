// Controllers/AdminController.cs

using System.Net;
using System.Runtime.InteropServices.Marshalling;
using Treasure_Bay.Services;

namespace Treasure_Bay.Controllers
{
    public class AdminController : BaseController
    {
        private AdminService _adminService;

        public AdminController(AdminService adminService, AuthService authService) : base(authService)
        {
            _adminService = adminService;
        }

        public override async Task HandleRequest(HttpListenerRequest req, HttpListenerResponse resp)
        {
            var method = req.HttpMethod;
            var path = req.Url?.AbsolutePath.ToLower() ?? "/";

            // Debugging
            // Console.WriteLine($"[AdminController] Processing Request:");
            // Console.WriteLine($"   Expected Path: '/api/admin/reset'");
            // Console.WriteLine($"   Actual Path:   '{path}'");
            // Console.WriteLine($"   Method:        '{method}'");

            if(path.TrimEnd('/') == "/api/admin/reset" && method == "POST")
            {
                try
                {
                    _adminService.ResetDatabase();
                    await SendResponseAsync(resp, "Database has been reset.", 200);
                }
                catch(Exception ex)
                {
                    await SendResponseAsync(resp, $"Error 500: {ex.Message}", 500);
                }
            }
            else
            {
                await SendResponseAsync(resp, "Error 404: Endpoint not found.", 404);
            }
        }
    }
}