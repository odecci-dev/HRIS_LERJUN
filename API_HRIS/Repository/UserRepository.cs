using API_HRIS.Models;

namespace API_HRIS.Repository
{

    public class UserRepository : IUserRepository
    {
        private readonly ODC_HRISContext _context;
        public UserRepository(ODC_HRISContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(TblUsersModel user)
        { 
            throw new NotImplementedException();
        }
    }
}
