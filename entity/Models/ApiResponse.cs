namespace entity.Models;

public interface IApiResponse
{
    string? Code { get; set; }
    string? Message { get; set; }
    object? Data { get; set; }
}
public record ApiResponse : IApiResponse
{
    public string? Code { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}

