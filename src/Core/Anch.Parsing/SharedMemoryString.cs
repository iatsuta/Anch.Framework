namespace Anch.Parsing;

public record SharedMemoryString(ReadOnlyMemory<char> Chars) : IComparable<SharedMemoryString>
{
    public SharedMemoryString(string value)
        : this(value.AsMemory())
    {
    }

    public ReadOnlySpan<char> Span => this.Chars.Span;

    public bool IsEmpty => this.Chars.IsEmpty;

    public int Length => this.Chars.Length;

    public ValueTuple<SharedMemoryString, SharedMemoryString> Split(int index)
    {
        return (this[..index], this[index..]);
    }

    public SharedMemoryString Slice(int start)
    {
        return new SharedMemoryString(this.Chars[start..]);
    }

    public SharedMemoryString Slice(int start, int length)
    {
        return new SharedMemoryString(this.Chars.Slice(start, length));
    }

    public ParsingState<SharedMemoryString, SharedMemoryString> ToError()
    {
        return this.ToError<SharedMemoryString>();
    }

    public ParsingState<SharedMemoryString, TValue> ToError<TValue>()
    {
        return new ParsingState<SharedMemoryString, TValue>(default!, this, true);
    }

    public ParsingState<SharedMemoryString, TValue> ToSuccess<TValue>(TValue value)
    {
        return new ParsingState<SharedMemoryString, TValue>(value, this);
    }

    public override string ToString()
    {
        return this.ToString(true);
    }

    public string ToString(bool fullString)
    {
        return this.ToString(fullString ? this.Length : 1000);
    }

    public string ToString(int charLimit)
    {
        return new string(this.Chars.Span[.. Math.Min(this.Length, charLimit)]);
    }

    public override int GetHashCode()
    {
        return this.Length.GetHashCode();
    }

    public virtual bool Equals(SharedMemoryString? other)
    {
        return other is not null && this.Equals(other, StringComparison.Ordinal);
    }

    public int CompareTo(SharedMemoryString? other) => this.Length.CompareTo(other?.Length);

    public bool Equals(SharedMemoryString pattern, StringComparison stringComparison) => this.Chars.Span.Equals(pattern.Chars.Span, stringComparison);


    public static implicit operator string(SharedMemoryString value) => value.ToString();

    public static implicit operator SharedMemoryString(string value) => new(value);

    public static implicit operator ReadOnlySpan<char>(SharedMemoryString value) => value.Chars.Span;

    public static SharedMemoryString Empty { get; } = string.Empty;
}