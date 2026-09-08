using System;
using Unity.Netcode;

public struct ScoreEntry : INetworkSerializable, IEquatable<ScoreEntry>
{
    public PlayerId PlayerId;
    public int Score;

    public bool Equals(ScoreEntry other)
    {
        return PlayerId == other.PlayerId && Score == other.Score;
    }

    public override bool Equals(object obj)
    {
        return obj is ScoreEntry scoreEntry && Equals(scoreEntry);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PlayerId,Score);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref Score);
    }
}

