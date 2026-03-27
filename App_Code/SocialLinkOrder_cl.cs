using System;
using System.Collections.Generic;
using System.Linq;

public static class SocialLinkOrder_cl
{
    public static List<MangXaHoi_tb> SortLinks(IEnumerable<MangXaHoi_tb> links, string serializedOrder)
    {
        List<MangXaHoi_tb> source = (links ?? Enumerable.Empty<MangXaHoi_tb>()).ToList();
        if (source.Count <= 1)
            return source.OrderBy(x => x.id).ToList();

        List<long> orderedIds = ParseIds(serializedOrder);
        Dictionary<long, int> rank = new Dictionary<long, int>();
        for (int i = 0; i < orderedIds.Count; i++)
        {
            if (!rank.ContainsKey(orderedIds[i]))
                rank.Add(orderedIds[i], i);
        }

        return source
            .OrderBy(x => rank.ContainsKey(x.id) ? 0 : 1)
            .ThenBy(x => rank.ContainsKey(x.id) ? rank[x.id] : int.MaxValue)
            .ThenBy(x => x.id)
            .ToList();
    }

    public static string Move(string serializedOrder, IEnumerable<MangXaHoi_tb> links, long targetId, int offset)
    {
        List<MangXaHoi_tb> sorted = SortLinks(links, serializedOrder);
        int currentIndex = sorted.FindIndex(x => x.id == targetId);
        if (currentIndex < 0)
            return Serialize(sorted);

        int targetIndex = currentIndex + offset;
        if (targetIndex < 0 || targetIndex >= sorted.Count)
            return Serialize(sorted);

        MangXaHoi_tb current = sorted[currentIndex];
        sorted[currentIndex] = sorted[targetIndex];
        sorted[targetIndex] = current;

        return Serialize(sorted);
    }

    public static string Normalize(string serializedOrder, IEnumerable<MangXaHoi_tb> links)
    {
        return Serialize(SortLinks(links, serializedOrder));
    }

    public static string Serialize(IEnumerable<MangXaHoi_tb> links)
    {
        return string.Join(",",
            (links ?? Enumerable.Empty<MangXaHoi_tb>())
                .Select(x => x.id.ToString())
                .ToArray());
    }

    private static List<long> ParseIds(string serializedOrder)
    {
        return (serializedOrder ?? "")
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(value =>
            {
                long id;
                return long.TryParse(value.Trim(), out id) ? id : 0L;
            })
            .Where(id => id > 0)
            .ToList();
    }
}
