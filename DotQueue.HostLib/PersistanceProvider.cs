namespace DotQueue.HostLib
{
    internal static class PersistanceProvider
    {
        internal static bool PersistMessages { get; set; }
        internal static IPersistenceAdapter Adapter { get; set; }
    }
}