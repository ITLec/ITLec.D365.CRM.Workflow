using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITLec.D365.CRM.Toolkit
{
    internal static class IEnumerableExtenstions
    {

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }

        /*
        [Serializable]
        private sealed class <>c__0<TSource>
		{
			public static readonly IEnumerableExtenstions.<>c__0<TSource> <>9 = new IEnumerableExtenstions.<>c__0<TSource>();

			public static Func<IEnumerable<TSource>, IEnumerable<TSource>> <>9__0_0;

			internal IEnumerable<TSource> <Batch>b__0_0(IEnumerable<TSource> x)
        {
            return x;
        }
    }

    [CompilerGenerated]
    [Serializable]
    private sealed class <>c__2<TSource, TResult>
		{
			public static readonly IEnumerableExtenstions.<>c__2<TSource, TResult> <>9 = new IEnumerableExtenstions.<>c__2<TSource, TResult>();

			public static Func<TSource, TSource> <>9__2_0;

			internal TSource<BatchImpl> b__2_0(TSource x)
    {
        return x;
    }
}

internal static IEnumerable<IEnumerable<TSource>> Batch<TSource>(this IEnumerable<TSource> source, int size)
{
    Func<IEnumerable<TSource>, IEnumerable<TSource>> arg_24_2;
    if ((arg_24_2 = IEnumerableExtenstions.<> c__0<TSource>.<> 9__0_0) == null)
    {
        arg_24_2 = (IEnumerableExtenstions.<> c__0<TSource>.<> 9__0_0 = new Func<IEnumerable<TSource>, IEnumerable<TSource>>(IEnumerableExtenstions.<> c__0<TSource>.<> 9.< Batch > b__0_0));
    }
    return source.Batch(size, arg_24_2);
}

internal static IEnumerable<TResult> Batch<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
{
    if (source == null)
    {
        throw new ArgumentNullException("source");
    }
    if (size <= 0)
    {
        throw new ArgumentOutOfRangeException("size");
    }
    if (resultSelector == null)
    {
        throw new ArgumentNullException("resultSelector");
    }
    return source.BatchImpl(size, resultSelector);
}

[IteratorStateMachine(typeof(IEnumerableExtenstions.< BatchImpl > d__2 <, >))]
private static IEnumerable<TResult> BatchImpl<TSource, TResult>(this IEnumerable<TSource> source, int size, Func<IEnumerable<TSource>, TResult> resultSelector)
{
    TSource[] array = null;
    int num = 0;
    foreach (TSource current in source)
    {
        if (array == null)
        {
            array = new TSource[size];
        }
        array[num++] = current;
        if (num == size)
        {
            IEnumerable<TSource> arg_AB_0 = array;
            Func<TSource, TSource> arg_AB_1;
            if ((arg_AB_1 = IEnumerableExtenstions.<> c__2<TSource, TResult>.<> 9__2_0) == null)
            {
                arg_AB_1 = (IEnumerableExtenstions.<> c__2<TSource, TResult>.<> 9__2_0 = new Func<TSource, TSource>(IEnumerableExtenstions.<> c__2<TSource, TResult>.<> 9.< BatchImpl > b__2_0));
            }
            yield return resultSelector(arg_AB_0.Select(arg_AB_1));
            array = null;
            num = 0;
        }
    }
    IEnumerator<TSource> enumerator = null;
    if (array != null && num > 0)
    {
        yield return resultSelector(array.Take(num));
    }
    yield break;
    yield break;
}*/
    }
}
