// function twoSum(numbers: number[], target: number): number[] {
//   if (numbers.length === 0) return null;
//   var t = [];
//   var r = numbers.filter((x) => x <= target);
//   var a = r.length > 0 ? Math.max(...r.map((x) => x)) : numbers[0];
//   t.push(a);
//   r = r.filter((x) => x != a).map((x) => x);
//   const i = (e, v) => e === v;
//   for (var z of r) {
//     if (z + a === target) {
//       t.push(z);
//     }
//   }

//   if (t.length < 2) return null;

//   var tt = [];
//   for (var n of t.reverse()) {
//     tt.push(numbers.findIndex((x) => i(x, n)) + 1);
//   }

//   return tt;
// }

function twoSum(numbers: number[], target: number): number[] {
  const map = new Map<number, number>();
  let i = 0;

  for (var n of numbers) {
    const c = target - n;
    if (map.has(c)) {
      return [map.get(c)! + 1, i + 1];
    }
    map.set(n, i);
    i++;
  }
  return [];
}

var t = [2, 7, 11, 15];
var r = twoSum(t, 9);
console.log(r.join());

t = [2, 3, 4];
r = twoSum(t, 6);
console.log(r.join());

// t = [-1, 0];
// r = twoSum(t, -1);
// console.log(r.join());

t = [0, 0, 3, 4];
r = twoSum(t, 0);
console.log(r.join());
