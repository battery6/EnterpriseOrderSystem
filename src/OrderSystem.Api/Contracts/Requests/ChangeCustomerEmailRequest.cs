namespace OrderSystem.Api.Contracts.Requests;
using System.ComponentModel.DataAnnotations;

public sealed record ChangeCustomerEmailRequest(
    [param: Required]
    [param: EmailAddress] string Email);