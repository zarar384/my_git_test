namespace LeaveMeAloneCSharp.AlgLeetcode.Tasks
{
    public static class BinarySearch
    {
        public static int Search(int[] nums, int target)
        {
            int l = 0;
            int r = nums.Length - 1;

            while (l <= r)
            {
                int m = l + (r - l) / 2;
                if (nums[m] == target) return m;
                else if (nums[m] < target) l = m + 1;
                else r = m - 1;
            }

            return -1;
        }

        public static void RunDemo()
        {
            // -1, 0, 3, 5, 9, 12
            // l:0 - r:5 - m:2 - nums[m]:3 - target:9 - 3 < 9 - l:2+1=3 - r:5
            // l:3 - r:5 - m:4 - nums[m]:9 - target:9 - 9 == 9 - return 4
            int[] nums = { -1, 0, 3, 5, 9, 12 };
            int target = 9;
            int result = Search(nums, target);
            Console.WriteLine($"{nums.JoinToString()} -> {target} = {result}");

            //  -1, -1, 1, 1, -1, 1
            // l:0 - r:5 - m:2 - nums[m]:1 - target:1 - 1 == 1 - return 2
            nums = new int[] { -1, -1, 1, 1, -1, 1 };
            target = 1;
            result = Search(nums, target);
            Console.WriteLine($"{nums.JoinToString()} -> {target} = {result}");

            //  -1, +1, 1, 1, +1, 1
            // l:0 - r:5 - m:2 - nums[m]:1 - target:1 - 1 == 1 - return 2
            nums = new int[] { -1, +1, 1, 1, +1, 1 };
            target = 1;
            result = Search(nums, target);
            Console.WriteLine($"{nums.JoinToString()} -> {target} = {result}");

            // -1, 0, 3, 5, 9, 12
            // l:0 - r:5 - m:2 - nums[m]:3 - target:-1 - 3 > -1 - r:2-1=1 - l:0
            // l:0 - r:1 - m:0 - nums[m]:-1 - target:-1 - -1 == -1 - return 0
            nums = new int[] { -1, 0, 3, 5, 9, 12 };
            target = -1;
            result = Search(nums, target);
            Console.WriteLine($"{nums.JoinToString()} -> {target} = {result}");

            // -1, 0, 3, 5, 9, 12
            // l:0 - r:5 - m:2 - nums[m]:3 - target:2 - 3 > 2 - r:2-1=1 - l:0
            // l:0 - r:1 - m:0 - nums[m]:-1 - target:2 - -1 < 2 - l:0+1=1 - r:1
            // l:1 - r:1 - m:1 - nums[m]:0 - target:2 - 0 < 2 - l:1+1=2 - r:1
            // l:2 - r:1 - l > r - return -1
            nums = new int[] { -1, 0, 3, 5, 9, 12 };
            target = 2;
            result = Search(nums, target);
            Console.WriteLine($"{nums.JoinToString()} -> {target} = {result}");

            // 5
            // l:0 - r:0 - m:0 - nums[m]:5 - target:5 - 5 == 5 - return 0
            nums = new int[] { 5 };
            target = 5;
            result = Search(nums, target);
            Console.WriteLine($"{nums.JoinToString()} -> {target} = {result}");
        }
    }
}
