namespace OrderSystem.Application.Customers.CreateCustomer;

public sealed record CreateCustomerCommand(
    string Name, 
    string Email);