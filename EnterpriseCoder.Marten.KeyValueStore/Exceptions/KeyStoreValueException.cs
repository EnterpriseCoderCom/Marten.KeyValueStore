namespace EnterpriseCoder.Marten.KeyValueStore.Exceptions;

public class KeyStoreValueException : Exception
{
    public KeyStoreValueException(string? message) : base(message)
    {
    }
}