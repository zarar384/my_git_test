public static class TwoSum2
{
    //1ms Beats 32.11%
    public static int[] TwoSum2Dic(int[] nums, int target)
    {
        var dic = new Dictionary<int, int>();

        for (var i = 0; i < nums.Length; i++)
        {
            var complete = target - nums[i];

            if (dic.ContainsKey(complete))
            {
                return new int[] { dic[complete], i + 1 };
            }

            dic[nums[i]] = i + 1;
        }

        return new int[] { };
    }

    public static int[] TwoSum(int[] numbers, int target)
    {
        var newNumbers = numbers.Select((v,i)=> new {v, i})
            .OrderBy(x=>x.v).ToArray();
        var l = 0;
        var r = newNumbers.Length - 1;

        while (l < r)
        {
            int sum = newNumbers[l].v + newNumbers[r].v;

            if (sum == target)
            {
                return new int[] { newNumbers[l].i + 1,  newNumbers[r].i + 1 };
            }

            if (sum < target)
            {
                l++;
            }
            else
            {
                r--;
            }
        }

        return new int[] { };
    }
}