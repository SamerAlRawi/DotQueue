namespace DotQueue.Client
{
    internal class LocalPortResolver : ILocalPortResolver
    {
        public int FindFreePort()
        {
            return 3345;
        }
    }
}