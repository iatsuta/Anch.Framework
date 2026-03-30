namespace CommonFramework.Parsing;

public interface IParingInput<in TSelf>
{
    public static abstract int CompareInfo(TSelf input1, TSelf input2);
}