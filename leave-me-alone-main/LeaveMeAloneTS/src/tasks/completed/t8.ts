function isValid(s: string): boolean {
  const sArr = s.split("");

  if (sArr.length === 0) return false;

  const matchingBrackets = {
    ")": "(",
    "}": "{",
    "]": "[",
  };

  let isAllValidChars = sArr.every(
    (char) =>
      char in matchingBrackets || Object.values(matchingBrackets).includes(char)
  );

  if (!isAllValidChars) return false;

  const stack: string[] = [];

  for (const char of sArr) {
    if (char in matchingBrackets) {
      const t: string = stack.pop();
      if (t !== matchingBrackets[char]) return false;
    } else stack.push(char);
  }

  return stack.length === 0;
}

console.log(isValid("()")); // true
console.log(isValid("()[]{}")); // true
console.log(isValid("(]")); // false
console.log(isValid("([)]")); // false
console.log(isValid("{[]}")); // true
