namespace PVIMS.API.Services
{
    public interface ITokenFactory
    {
        string GenerateToken(int size = 32);
    }
}
