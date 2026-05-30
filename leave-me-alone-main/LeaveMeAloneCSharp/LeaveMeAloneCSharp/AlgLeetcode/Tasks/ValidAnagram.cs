public static class ValidAnagram{
    public static bool IsAnagram(string s, string t)
    {
        if(s.Length != t.Length) return false;

        var countMap = new Dictionary<char, int>();

        for(int i=0; i<s.Length; i++){
            if(countMap.ContainsKey(s[i]))
                countMap[s[i]]++;
            else 
                countMap[s[i]] = 1;

            if(countMap.ContainsKey(t[i]))
                countMap[t[i]]--;
            else 
                countMap[t[i]] = -1;
        } 

        foreach(var count in countMap.Values){
            if(count != 0){
                return false;
            }
        }

        return true;
    }
}