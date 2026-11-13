namespace OrderSystem.Api.Contracts.Requests;
using System.ComponentModel.DataAnnotations;

public sealed record CreateCustomerRequest(
    [property: Required] string Name,
    [property: Required]
    [property: EmailAddress] string Email);