var MinStack = /** @class */ (function () {
    function MinStack() {
        this._stack = [];
        this._minStack = [];
    }
    MinStack.prototype.push = function (val) {
        this._stack.push(val);
        if (this._minStack.length === 0 ||
            val <= this._minStack[this._minStack.length - 1])
            this._minStack.push(val);
    };
    MinStack.prototype.pop = function () {
        var remValue = this._stack.pop();
        if (remValue === this._minStack[this._minStack.length - 1])
            this._minStack.pop();
    };
    MinStack.prototype.top = function () {
        return this._stack[this._stack.length - 1];
    };
    MinStack.prototype.getMin = function () {
        return this._minStack[this._minStack.length - 1];
    };
    return MinStack;
}());
/**
 * Your MinStack object will be instantiated and called as such:
 * var obj = new MinStack()
 * obj.push(val)
 * obj.pop()
 * var param_3 = obj.top()
 * var param_4 = obj.getMin()
 */
var minStack = new MinStack();
minStack.push(-2);
minStack.push(0);
minStack.push(-3);
var ms = minStack.getMin(); // return -3
console.log(ms);
minStack.pop();
ms = minStack.top(); // return 0
console.log(ms);
ms = minStack.getMin(); // return -2
console.log(ms);
