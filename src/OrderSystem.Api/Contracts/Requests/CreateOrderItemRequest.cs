namespace OrderSystem.Api.Contracts.Requests;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public sealed record CreateOrderItemRequest(
    [property: JsonRequired] Guid ProductId,
    [property: JsonRequired]
    [property: Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    int Quantity) : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ProductId == Guid.Empty)
        {
            yield return new ValidationResult(
                "ProductId cannot be empty.", 
                [nameof(ProductId)]);
        }
    }
}