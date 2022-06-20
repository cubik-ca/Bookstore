﻿using Bookstore.EventSourcing;

namespace Bookstore.Domain.Subject.Country;

public class Name : Value<Name>
{
    private string Value { get; set; }

    private Name(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException(nameof(value));
        Value = value;
    }

    public static Name FromString(string? value) => new(value);

    public static implicit operator string?(Name? name) => name?.Value;
}