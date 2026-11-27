namespace OrderSystem.Application.Customers.ChangeCustomerEmail;

public sealed record ChangeCustomerEmailCommand(
    Guid Id,
    string Email
);