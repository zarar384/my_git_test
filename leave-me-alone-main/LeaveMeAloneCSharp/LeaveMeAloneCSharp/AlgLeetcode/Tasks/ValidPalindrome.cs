public static class ValidPalindrome{
    //16ms Beats 71.70%
    public static bool IsPalindrome(string s){
        s= s.ToLower();
        int l = 0, r = s.Length - 1;

        while (l < r){
            if(!char.IsLetterOrDigit(s[l])){
                l++;
                continue;
            }
            if(!char.IsLetterOrDigit(s[r])){
                r--;
                continue;
            }
            if(s[l]!= s[r]) {
                return false;}
            l++;
            r--;
        }

        return true;
    }
}