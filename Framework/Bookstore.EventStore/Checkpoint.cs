namespace Bookstore.EventStore;

public class Checkpoint
{
    public string? Id { get; init; }

    public ulong? Position { get; set; }
}