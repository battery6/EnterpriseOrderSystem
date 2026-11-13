namespace OrderSystem.Api.Contracts.Requests;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public sealed record CreateOrderRequest(
    [property: JsonRequired] Guid CustomerId,
    [property: JsonRequired] List<CreateOrderItemRequest> OrderItems);