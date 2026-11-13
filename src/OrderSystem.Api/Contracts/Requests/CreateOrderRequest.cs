namespace OrderSystem.Api.Contracts.Requests;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public sealed record CreateOrderRequest(
    [property: JsonRequired] Guid CustomerId,
    [property: JsonRequired] List<CreateOrderItemRequest> OrderItems)
    : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CustomerId == Guid.Empty)
        {
            yield return new ValidationResult(
                "CustomerId cannot be empty.", 
                [nameof(CustomerId)]);
        }

        if (OrderItems is null)
        {
            yield return new ValidationResult(
                "Order items are required.", 
                [nameof(OrderItems)]);
            yield break; // No need to continue validation if OrderItems is null
        }

        if(OrderItems.Count == 0)
        {
            yield return new ValidationResult(
                "Order must contain at least one order item.", 
                [nameof(OrderItems)]);
        }

        if (OrderItems!.Any(item => item is null))
        {
            yield return new ValidationResult(
                "Order items cannot contain null values.", 
                [nameof(OrderItems)]);
        }
    }
}