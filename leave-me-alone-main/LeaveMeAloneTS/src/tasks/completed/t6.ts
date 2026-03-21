class ListNode {
  val: number;
  next: ListNode | null;
  constructor(val?: number, next?: ListNode | null) {
    this.val = val === undefined ? 0 : val;
    this.next = next === undefined ? null : next;
  }
}

function addTwoNumbers(
  l1: ListNode | null,
  l2: ListNode | null
): ListNode | null {
  let dummyHead = new ListNode(0);
  let p = l1,
    q = l2,
    current = dummyHead;
  let carry = 0;

  while (p !== null || q !== null) {
    let x = p !== null ? p.val : 0;
    let y = q !== null ? q.val : 0;
    let sum = carry + x + y;
    carry = Math.floor(sum / 10);
    current.next = new ListNode(sum % 10);
    current = current.next;
    if (p !== null) p = p.next;
    if (q !== null) q = q.next;
  }

  if (carry > 0) {
    current.next = new ListNode(carry);
  }

  return dummyHead.next;
}

function listNodeToArrayRecursive(
  node: ListNode | null,
  result: number[] = []
): number[] {
  if (node === null) {
    return result;
  }

  result.push(node.val);
  return listNodeToArrayRecursive(node.next, result);
}

// var l1 = [2,4,3], l2 = [5,6,4]
let a1 = new ListNode(2);
let a2 = new ListNode(4);
let a3 = new ListNode(3);
a1.next = a2;
a2.next = a3;

let b1 = new ListNode(5);
let b2 = new ListNode(6);
let b3 = new ListNode(4);
b1.next = b2;
b2.next = b3;

let c = addTwoNumbers(a1, b1);
var z = listNodeToArrayRecursive(c);

console.log(z.join());
