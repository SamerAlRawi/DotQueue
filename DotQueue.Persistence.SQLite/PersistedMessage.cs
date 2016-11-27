using DotQueue.HostLib;

namespace DotQueue.Persistence.SQLite
{
    internal class PersistedMessage : Message
    {
        public long TableId { get; set; }
    }
}