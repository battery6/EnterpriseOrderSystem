namespace OrderSystem.Api.Contracts.Requests;
using System.ComponentModel.DataAnnotations;

public sealed record CreateCustomerRequest(
    [param: Required] string Name,
    [param: Required]
    [param: EmailAddress] string Email);