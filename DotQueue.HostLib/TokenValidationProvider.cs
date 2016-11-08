namespace DotQueue.HostLib
{
    internal static class TokenValidationProvider
    {
        internal static bool CheckAuthorization { get; set; }
        internal static IApiTokenValidator Validator { get; set; }
    }
}