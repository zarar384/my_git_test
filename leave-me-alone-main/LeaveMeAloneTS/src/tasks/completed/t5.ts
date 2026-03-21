function productExceptSelf(nums: number[]): number[] {
  var n = nums.length;
  var r = [];

  var prefix = 1;
  r[0] = prefix;

  for (var i = 1; i < n; i++) {
    r[i] = prefix;
    prefix *= nums[i];
  }

  var postfix = 1;
  for (var i = n - 1; i >= 0; i--) {
    r[i] *= postfix;
    postfix *= nums[i];
  }

  return r;
}

var t = [1, 2, 3, 4];
var r = productExceptSelf(t);
var rStr = r.join();

console.log(rStr);
