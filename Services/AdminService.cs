// Services/AdminService.cs

using Treasure_Bay.Repositories;

namespace Treasure_Bay.Services
{
    public class AdminService
    {
        private AdminRepository _adminRepo;

        public AdminService(AdminRepository adminRepository)
        {
            _adminRepo = adminRepository;
        }

        public void ResetDatabase()
        {
            _adminRepo.ResetDatabase();
        }

        public void DropDatabase()
        {
            _adminRepo.DropDatabase();
        }
    }
}