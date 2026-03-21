public static class ProductOfArrayExceptSelf
{
    //1ms Beats 100.00%
    public static int[] ProductExceptSelf(int[] nums)
    {
        var l = nums.Length;
        var result = new int[l];

        //произведение всех элементов слева от текущего индекса 
        result[0] = 1;
        for (var i = 1; i < l; i++)
        {
            result[i] = result[i - 1] * nums[i - 1];
        }

        //произведение всех элементов справа от текущего индекса 
        int r = 1;
        for (var i = l - 2; i >= 0; i--)
        {
            r *= nums[i + 1];
            result[i] *= r;
        }
        return result;
    }
}