var ListNode = /** @class */ (function () {
    function ListNode(val, next) {
        this.val = val === undefined ? 0 : val;
        this.next = next === undefined ? null : next;
    }
    return ListNode;
}());
function addTwoNumbers(l1, l2) {
    var dummyHead = new ListNode(0);
    var p = l1, q = l2, current = dummyHead;
    var carry = 0;
    while (p !== null || q !== null) {
        var x = p !== null ? p.val : 0;
        var y = q !== null ? q.val : 0;
        var sum = carry + x + y;
        carry = Math.floor(sum / 10);
        current.next = new ListNode(sum % 10);
        current = current.next;
        if (p !== null)
            p = p.next;
        if (q !== null)
            q = q.next;
    }
    if (carry > 0) {
        current.next = new ListNode(carry);
    }
    return dummyHead.next;
}
function listNodeToArrayRecursive(node, result) {
    if (result === void 0) { result = []; }
    if (node === null) {
        return result;
    }
    result.push(node.val);
    return listNodeToArrayRecursive(node.next, result);
}
// var l1 = [2,4,3], l2 = [5,6,4]
var a1 = new ListNode(2);
var a2 = new ListNode(4);
var a3 = new ListNode(3);
a1.next = a2;
a2.next = a3;
var b1 = new ListNode(5);
var b2 = new ListNode(6);
var b3 = new ListNode(4);
b1.next = b2;
b2.next = b3;
var c = addTwoNumbers(a1, b1);
var z = listNodeToArrayRecursive(c);
console.log(z.join());
