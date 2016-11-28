namespace DotQueue.Persistence.SQLite.Tests
{
    class DummyMessage
    {
        public long AccountNumber { get; set; }
        public string Name { get; set; }
        public int ZipCode { get; set; }
        public override bool Equals(object obj)
        {
            if ((obj as DummyMessage) == null)
                return false;
            var instance = obj as DummyMessage;
            return instance.AccountNumber == AccountNumber && instance.Name == Name && instance.ZipCode == ZipCode;
        }
    }
}
