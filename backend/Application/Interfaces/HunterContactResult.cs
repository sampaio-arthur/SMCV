namespace SMCV.Application.Interfaces;

public record HunterContactResult(
    string CompanyName,
    string Domain,
    string Email,
    string? ContactName,
    string? Position
);
