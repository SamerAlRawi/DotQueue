namespace DotQueue.HostLib
{
    internal static class TokenValidationProvider
    {
        public static bool CheckAuthorization { get; set; }
        public static IApiTokenValidator Validator { get; set; }
    }
}