function isPalindrome(num: number): boolean {
  const strA: string[] = num.toString().split("");
  const first = strA[0];
  const last = strA[strA.length - 1];
  if (first === last) return true;
  return false;
}

function isPalindromeFixed(num: number): boolean {
  const str = num.toString();
  const nStr = str.split("").reverse().join("");
  console.log(`${str} is ${nStr} = ${str === nStr}`);
  return str === nStr;
}

console.log(isPalindromeFixed(11001));
