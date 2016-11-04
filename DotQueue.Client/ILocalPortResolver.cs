namespace DotQueue.Client
{
    internal interface ILocalPortResolver
    {
        int FindFreePort();
    }
}