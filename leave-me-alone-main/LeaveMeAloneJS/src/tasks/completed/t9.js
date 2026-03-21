function longestConsecutive(nums) {
    if (nums.length === 0)
        return 0;
    var l = 1;
    for (var _i = 0, nums_1 = nums; _i < nums_1.length; _i++) {
        var n_1 = nums_1[_i];
        var a = n_1;
        for (var x = 0; x < nums.length; x++) {
            var t = a - 1;
            if (nums.includes(t)) {
                if (!r.includes(t)) {
                    t += 1;
                    l = Math.max(l, t);
                }
                else {
                    break;
                }
                a = t;
            }
            else {
                break;
            }
        }
    }
    return l;
}
var n = [0];
var b = longestConsecutive(n);
console.log(b);
