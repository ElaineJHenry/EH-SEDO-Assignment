using EH_SEDO_Assignment.Models;

namespace EH_SEDO_Assignment.Data
{
    public interface IDatabaseRepository
    {
        Task<List<UserListInfo>> GetUserInfoList();
    }
}
