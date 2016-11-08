namespace DotQueue.HostLib
{
    public interface IApiTokenValidator
    {
        bool IsValidToken(string token);
    }
}