namespace OrderSystem.Api.Contracts.Requests;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public sealed record ChangeProductPriceRequest(
    [property: JsonRequired]
    [param: Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Price cannot be negative.")]
    decimal CurrentPrice);