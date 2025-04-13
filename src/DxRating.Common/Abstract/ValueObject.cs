using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DxRating.Common.Abstract;

// ReSharper disable once ConvertIfStatementToReturnStatement

[SuppressMessage("Design", "CA1051:Do not declare visible instance fields")]
public abstract record ValueObject<TV, TK>(TV Value)
    where TV : notnull
    where TK : ValueObject<TV, TK>
{
    [JsonIgnore]
    public readonly TV Value = Value;

    public override string ToString()
    {
        return Value.ToString()!;
    }

    public virtual bool Equals(ValueObject<TV, TK>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    protected virtual bool PrintMembers(StringBuilder builder)
    {
        builder.Append(Value);
        return true;
    }

    public static implicit operator TV(ValueObject<TV, TK> value) => value.Value;

    public static List<TK> Fields { get; } = typeof(TK)
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(x => (TK)x.GetValue(null)!)
        .ToList();

    public static TK From(TV? source)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source), $"NULL is not a valid {typeof(TK).Name}");
        }

        var field = Fields.FirstOrDefault(x => x.Value.Equals(source)) ??
                    throw new ArgumentOutOfRangeException(nameof(source), $"{source} is not a valid {typeof(TK).Name}");
        return field;
    }
}

public class ValueObjectJsonConverter<TV, TK> : JsonConverter<ValueObject<TV, TK>>
    where TV : notnull
    where TK : ValueObject<TV, TK>
{
    public override ValueObject<TV, TK>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        TV? value;
        if (typeof(TV) == typeof(string))
        {
            value = (TV)(object)reader.GetString()!;
        }
        else if (typeof(TV) == typeof(int))
        {
            value = (TV)(object)reader.GetInt32();
        }
        else if (typeof(TV) == typeof(double))
        {
            value = (TV)(object)reader.GetDouble();
        }
        else if (typeof(TV) == typeof(bool))
        {
            value = (TV)(object)reader.GetBoolean();
        }
        else
        {
            throw new NotSupportedException($"Unsupported type: {typeof(TV).Name}");
        }

        return ValueObject<TV, TK>.From(value);
    }

    public override void Write(Utf8JsonWriter writer, ValueObject<TV, TK> value, JsonSerializerOptions options)
    {
        if (typeof(TV) == typeof(string))
        {
            writer.WriteStringValue((string)(object)value.Value);
        }
        else if (typeof(TV) == typeof(int))
        {
            writer.WriteNumberValue((int)(object)value.Value);
        }
        else if (typeof(TV) == typeof(double))
        {
            writer.WriteNumberValue((double)(object)value.Value);
        }
        else if (typeof(TV) == typeof(bool))
        {
            writer.WriteBooleanValue((bool)(object)value.Value);
        }
        else
        {
            throw new NotSupportedException($"Unsupported type: {typeof(TV).Name}");
        }
    }
}
