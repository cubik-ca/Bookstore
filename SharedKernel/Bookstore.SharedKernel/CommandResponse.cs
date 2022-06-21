namespace Bookstore.SharedKernel;

public class CommandResponse
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public string? StackTrace { get; set; }
}