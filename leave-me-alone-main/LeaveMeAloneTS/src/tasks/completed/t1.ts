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
function twoSum(nums: number[], target: number): number[] {
  const numIndices = new Map<number, number>();

  for (let i = 0; i < nums.length; i++) {
    const complement = target - nums[i];
    console.log(`${target} - ${nums[i]} = ${complement}`);
    if (numIndices.has(complement)) {
      numIndices.forEach(mapConsole);
      return [numIndices.get(complement)!, i];
    }
    numIndices.set(nums[i], i);
  }

  throw new Error("No solution found");
}

function mapConsole(key, value) {
  console.log(`m[${key}] = ${value}`);
}

const nums = [2, 13, 11, 7];
const target = 9;
const result = twoSum(nums, target);
console.log("Output:", result);
