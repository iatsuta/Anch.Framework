namespace CommonFramework.Parsing;

public readonly record struct SharedMemoryString(ReadOnlyMemory<char> Chars)
    : IParingInput<SharedMemoryString>
{
    public ReadOnlySpan<char> Span => this.Chars.Span;

    public bool IsEmpty => this.Chars.IsEmpty;

    public int Length => this.Chars.Length;

    public ValueTuple<SharedMemoryString, SharedMemoryString> Split(int index)
    {
        return (this.Slice(0, index), this.Slice(index));
    }

    public SharedMemoryString Slice(int start)
    {
        return new SharedMemoryString(this.Chars.Slice(start));
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
        return this.ToString(false);
    }

    public string ToString(bool fullString)
    {
        return this.ToString(fullString ? this.Length : 1000);
    }

    public string ToString(int charLimit)
    {
        return new string(this.Chars.Span[.. Math.Min(this.Length, charLimit)]);
    }

    private string GetDebuggerDisplay()
    {
        return this.ToString(1000);
    }

    public static implicit operator SharedMemoryString(string value)
    {
        return new SharedMemoryString(value.AsMemory());
    }

    public static implicit operator ReadOnlySpan<char>(SharedMemoryString value)
    {
        return value.Chars.Span;
    }

    public static int CompareInfo(SharedMemoryString input1, SharedMemoryString input2)
    {
        return input2.Length.CompareTo(input1.Length);
    }
}