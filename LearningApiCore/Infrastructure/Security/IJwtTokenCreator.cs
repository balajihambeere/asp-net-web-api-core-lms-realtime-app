namespace LearningApiCore.Infrastructure.Security
{
    using LearningApiCore.DataAccess.Models;
    using System.Threading.Tasks;
    public interface IJwtTokenCreator
    {
        Task<string> CreateToken(string email, AppUser user);
    }
}
