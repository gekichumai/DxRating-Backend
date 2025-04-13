using System.Collections;

namespace DxRating.Worker.Jobs.Utils;

public static class DictionaryUtils
{
    public static Dictionary<TKey, List<TValue>> Merge<TKey, TValue>(params Dictionary<TKey, List<TValue>>[] sources)
        where TKey : notnull
    {
        var final = new Dictionary<TKey, List<TValue>>();

        foreach (var source in sources)
        {
            foreach (var (k, v) in source)
            {
                if (final.ContainsKey(k) is false)
                {
                    final.Add(k, []);
                }

                final[k].AddRange(v);
            }
        }

        final = final.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Distinct().ToList());

        return final;
    }
}
