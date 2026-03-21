public static class ValidParentheses
{
    public static bool IsValid(string s)
    {
        var stack = new Stack<char>();
        var dic = new Dictionary<char, char> {
        { ')', '(' },
        { ']', '[' },
        { '}', '{' }
    };


        foreach (var c in s)
        {
            if (dic.ContainsValue(c))
            {
                stack.Push(c);
            }
            else if (dic.ContainsKey(c))
            {
                if (stack.Count == 0 || stack.Pop() != dic[c])
                {
                    {
                        return false;
                    }
                }
            }
        }

        return stack.Count == 0;
    }
}