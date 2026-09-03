using Unity.Netcode;

public record PlayerId : INetworkSerializable
{
    public int Id { get; }

    public PlayerId(int id)
    {
        Id = id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
    }
}