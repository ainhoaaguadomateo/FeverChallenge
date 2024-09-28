using System.Text.Json.Serialization;

namespace Fever.Domain.Features.GetEvents.Models;

public sealed record GetEventResponseDto
{
    public List<EventResultDTO> Events { get; set; } = [];
}

public sealed record EventResultDTO
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = null!;

    [JsonPropertyName("start_date")]
    public DateOnly StartDate { get; set; }

    [JsonPropertyName("start_time")]
    public TimeOnly StartTime { get; set; }

    [JsonPropertyName("end_date")]
    public DateOnly EndDate { get; set; }

    [JsonPropertyName("end_time")]
    public TimeOnly EndTime { get; set; }

    [JsonPropertyName("min_price")]
    public decimal MinPrice { get; set; }

    [JsonPropertyName("max_price")]
    public decimal MaxPrice { get; set; }
}

public sealed record ApiResponse<T>
{
    public T? Data { get; set; }

    public ErrorResponse? Error { get; set; }

    public static ApiResponse<T> Success(T data)
    {
        return new ApiResponse<T> { Data = data, Error = null };
    }

    public static ApiResponse<T> Fail(string errorCode, object? errorMessage)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Error = new ErrorResponse(errorCode, errorMessage),
        };
    }
}

public sealed record ErrorResponse(string Code, object? Message);
