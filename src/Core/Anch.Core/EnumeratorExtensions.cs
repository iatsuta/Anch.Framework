namespace Anch.Core;

public static class EnumeratorExtensions
{
    extension<T>(IEnumerator<T> source)
    {
        public IEnumerable<T> ReadToEnd()
        {
            while (source.MoveNext())
            {
                yield return source.Current;
            }
        }

        public IEnumerable<T> ReadMany(int count)
        {
            return Enumerable.Range(0, count).Select(_ => source.ReadSingle());
        }

        public T ReadSingle()
        {
            if (source.MoveNext())
            {
                return source.Current;
            }
            else
            {
                throw new Exception("enumerator finished");
            }
        }
    }
}