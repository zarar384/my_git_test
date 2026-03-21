function twoSum(numbers, target) {
    var t = [];
    var r = numbers.filter(function (x) { return x < target; });
    var a = Math.max.apply(Math, r.map(function (x) { return x; }));
    t.push(a);
    r = r.filter(function (x) { return x != a; }).map(function (x) { return x; });
    var i = function (e, v) { return e === v; };
    for (var _i = 0, r_1 = r; _i < r_1.length; _i++) {
        var z = r_1[_i];
        if (z + a === target) {
            t.push(z);
        }
    }
    if (t.length < 2)
        return null;
    var tt = [];
    for (var _a = 0, _b = t.reverse(); _a < _b.length; _a++) {
        var n = _b[_a];
        tt.push(numbers.findIndex(function (x) { return i(x, n); }) + 1);
    }
    return tt;
}
// var t = [2, 7, 11, 15];
// var r = twoSum(t, 9);
// console.log(r.join());
// t = [2, 3, 4];
// r = twoSum(t, 6);
// console.log(r.join());
var t = [-1, 0];
var r = twoSum(t, -1);
// console.log(r.join());
