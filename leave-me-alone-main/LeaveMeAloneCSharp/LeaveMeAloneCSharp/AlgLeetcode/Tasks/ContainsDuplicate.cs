public static class ContainsDuplicate
{
    public static bool Run1(int[] nums)
    {
        HashSet<int> set = new HashSet<int>();
        for (int i = 0; i < nums.Length; i++){
            if(set.Contains(nums[i])){
                return true;
            }
            set.Add(nums[i]);
        }

        return false;
    }

    public static bool Run2(int[] nums)
    {
        Array.Sort(nums);

        for(int i = 1; i < nums.Length; i++){
            if(nums[i] == nums[i-1]){
                return true;
            }
        }

        return false;
    }
}