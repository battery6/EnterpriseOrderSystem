namespace OrderSystem.Domain.Exceptions;

public sealed class OrderCannotBeEmptyException()
    : InvalidOperationException(
        $"Orders must contain at least one item.");