using PetHaven.Data.Model;

namespace PetHaven.Authentication
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
