/*arr [2, 13, 11, 7]; target 9;
0: 9-2 = 7
[2, 0]
1: 9-13 = -4
[2, 0], [13, 1]
2: 9-11 = -2
[2, 0], [13, 1], [11,2]
3: 9-7 = 2
Output: [0,3]
*/
function twoSum(nums, target) {
    var numIndices = new Map();
    for (var i = 0; i < nums.length; i++) {
        var complement = target - nums[i];
        console.log("".concat(target, " - ").concat(nums[i], " = ").concat(complement));
        if (numIndices.has(complement)) {
            numIndices.forEach(mapConsole);
            return [numIndices.get(complement), i];
        }
        numIndices.set(nums[i], i);
    }
    throw new Error("No solution found");
}
function mapConsole(key, value) {
    console.log("m[".concat(key, "] = ").concat(value));
}
var nums = [2, 13, 11, 7];
var target = 9;
var result = twoSum(nums, target);
console.log("Output:", result);
