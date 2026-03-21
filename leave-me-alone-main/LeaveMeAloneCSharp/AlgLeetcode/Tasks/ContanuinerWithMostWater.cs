public static class ContanuinerWithMostWater
{
    //1ms Beats 99.90%
    public static int MaxArea(int[] height)
    {
        var l = 0;
        var r = height.Length - 1;
        var a = 0;

        while (l < r){
            var c1 = height[l];
            var c2 = height[r];

            var w = r - l; 
            var h = Math.Min(c1, c2);
            var s = w * h;

            a = Math.Max(a, s);

            if(c1<c2) l++;
            else r--;
        }

        return a;
    }
}