public static class TwoSum
{
    //29ms Beats 49.65%
    public static int[] TwoSumBruteForce(int[] nums, int target)
    {
        for (var i = 0; i < nums.Length; i++)
        {
            for (var j = i + 1; j < nums.Length; j++)
            {
                if (nums[i] + nums[j] == target)
                {
                    return new int[] { i, j };
                }
            }
        }

        return new int[] { };
    }

    //1ms Beats 98.13%
    public static int[] TwoSumDic(int[] nums, int target)
    {
        var dic = new Dictionary<int, int>();

        for(var i = 0; i < nums.Length; i++){
            var complete = target - nums[i];

            if(dic.ContainsKey(complete)){
                return new int[]{dic[complete], i};
            }

            dic[nums[i]] = i; 
        }

        return new int[] { };
    }
}