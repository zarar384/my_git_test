import executionTime from "../helpers";

function topKFrequent(nums: number[], k: number): number[] {
  const countMap = new Map<number, number>();
  nums.forEach((num) => {
    if (countMap.has(num)) {
      countMap.set(num, countMap.get(num)! + 1);
    } else {
      countMap.set(num, 1);
    }
  });

  var countArray = Array.from(countMap.entries());

  countArray.sort((a, b) => b[1] - a[1]);

  var result = countArray.slice(0, k).map((entry) => entry[0]);

  return result;
}

var nums = [1, 2];
var k = 2;
var t = executionTime(() => topKFrequent(nums, k), true);
console.log(`Execution time: ${t} ms`);
