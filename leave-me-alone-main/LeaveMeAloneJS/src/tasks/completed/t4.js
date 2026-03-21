"use strict";
exports.__esModule = true;
var helpers_1 = require("./helpers");
function topKFrequent(nums, k) {
    var countMap = new Map();
    nums.forEach(function (num) {
        if (countMap.has(num)) {
            countMap.set(num, countMap.get(num) + 1);
        }
        else {
            countMap.set(num, 1);
        }
    });
    var countArray = Array.from(countMap.entries());
    countArray.sort(function (a, b) { return b[1] - a[1]; });
    var result = countArray.slice(0, k).map(function (entry) { return entry[0]; });
    return result;
}
var nums = [1, 2];
var k = 2;
var t = (0, helpers_1["default"])(function () { return topKFrequent(nums, k); }, true);
console.log("Execution time: ".concat(t, " ms"));
