using API_HRIS.Models;

namespace API_HRIS.Repository
{
    public interface IUserRepository
    {
        Task<bool> CreateAsync(TblUsersModel user);
        //Task<TblUsersModel> GetByIdAsync(int id);
        //Task<IEnumerable<TblUsersModel>> GetAllAsync();
        //Task<bool> UpdateAsync(TblUsersModel user);
        //Task<bool> DeleteAsync(int id);
    }
}
