namespace Bookstore.EventStore;

public class Checkpoint
{
    public string? Id { get; set; }

    public string DbId
    {
        get => $"checkpoints/{Id}";
        set { }
    }

    public ulong? Position { get; set; }
}