namespace Games4Trade.Services
{
    public interface ILoginService
    {
        string ComputeHash(string salt, string password);
        string GetSalt();
    }
}