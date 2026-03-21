public static class GroupAnagrams
{
    // 25ms Beats 41.62%
    public static IList<IList<string>> Run1(string[] strs)
    {
        var l = new List<IList<string>>();
        var dic = new Dictionary<string, List<string>>();
        foreach (var str in strs)
        {
            // var s = string.Join("", str.ToCharArray().Order());
            var key = str.ToCharArray().Order().ToArray();
            var s = new string(key);
            if (!dic.ContainsKey(s))
            {
                dic[s] = new List<string>();
            }
            dic[s].Add(str);
        }

        l.AddRange(dic.Values);

        return l;
    }

    //49ms Beats 11.71%
    public static IList<IList<string>> Run2(string[] strs)
    {
        var dic = new Dictionary<string, List<string>>();

        foreach (var str in strs)
        {
            var count = new Dictionary<char, int>();
            foreach (var c in str)
            {
                if (!count.ContainsKey(c))
                {
                    count[c] = 0;
                }
                count[c]++;
            }

            var key = string.Join("", count.OrderBy(c => c.Key));
            if (!dic.ContainsKey(key))
            {
                dic[key] = new List<string>();
            }
            dic[key].Add(str);
        }

        return dic.Values.Select(list => (IList<string>)list).ToList();
    }

    public static IList<IList<string>> Run3(string[] strs)
    {
        var dic = new Dictionary<string, List<string>>();

        foreach (var str in strs)
        {
            int[] count = new int[256]; // UTF-16 = 65536 symbols
            foreach (var c in str)
            {
                count[c]++;
            }

            var key = string.Join("", count);
            if (!dic.ContainsKey(key))
            {
                dic[key] = new List<string>();
            }
            dic[key].Add(str);
        }

        return dic.Values.Select(list => (IList<string>)list).ToList();
    }

    //43ms Beats 18.07%
    public static IList<IList<string>> Run4(string[] strs)
    {
        var dic = new Dictionary<string, List<string>>();
        foreach (var str in strs)
        {
            var key = new string(str.OrderBy(c => c).ToArray());
            if (!dic.ContainsKey(key))
            {
                dic[key] = new List<string>();
            }
            dic[key].Add(str);
        }

        return dic.Values.Select(list => (IList<string>)list).ToList();
    }

   //25ms Beats 41.62%
    public static IList<IList<string>> Run5(string[] strs)
    {
        var dic = new Dictionary<string, List<string>>();

        foreach (var str in strs)
        {
            int[] count = new int[26];
            foreach (var c in str)
            {
                count[c - 'a']++;
            }

            var sb = new System.Text.StringBuilder();
            for(int i = 0; i < 26; i++){
                sb.Append('#');
                sb.Append(count[i]);
            }

            var key = string.Join(",", count);
            if (!dic.ContainsKey(key))
            {
                dic[key] = new List<string>();
            }
            dic[key].Add(str);
        }

        return dic.Values.ToList<IList<string>>();
    }

    //6ms Beats 99.08%
    public static IList<IList<string>> Run6(string[] strs)
    {
        var dic = new Dictionary<string, List<string>>();

        foreach (var str in strs)
        {
            char[] count = new char[26];
            foreach (var c in str)
            {
                count[c - 'a']++;
            }


            var key = new string(count);
            if (!dic.ContainsKey(key))
            {
                dic[key] = new List<string>();
            }
            dic[key].Add(str);
        }

        return dic.Values.ToList<IList<string>>();
    }
}