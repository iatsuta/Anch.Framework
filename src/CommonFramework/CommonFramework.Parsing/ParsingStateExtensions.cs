namespace CommonFramework.Parsing;

public static class ParsingStateExtensions
{
    public static TValue GetValue<TInput, TValue>(this ParsingState<TInput, TValue> state)
    {
        if (state.HasError)
        {
            throw new Exception($"Can't parse: {state.Rest}");
        }
        else
        {
            return state.Value;
        }
    }
}