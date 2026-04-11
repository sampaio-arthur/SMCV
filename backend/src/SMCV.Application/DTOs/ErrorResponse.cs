namespace SMCV.Application.DTOs;

public record ErrorResponse(string Error, string? Details = null);
