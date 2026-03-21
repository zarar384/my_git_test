function isValid(s) {
    var sArr = s.split("");
    if (sArr.length === 0)
        return false;
    var matchingBrackets = {
        ")": "(",
        "}": "{",
        "]": "[",
        "(": ")",
        "{": "}",
        "[": "]",
    };
    var isAllValidChars = sArr.every(function (char) {
        return char in matchingBrackets || Object.values(matchingBrackets).includes(char);
    });
    if (!isAllValidChars)
        return false;
    var stack = [];
    for (var _i = 0, sArr_1 = sArr; _i < sArr_1.length; _i++) {
        var char = sArr_1[_i];
        if (char in matchingBrackets)
            stack.push(char);
        else {
            var top_1 = stack.pop();
            if (top_1 !== matchingBrackets[char])
                return false;
        }
    }
    return stack.length === 0;
}
console.log(isValid("()")); // true
console.log(isValid("()[]{}")); // true
console.log(isValid("(]")); // false
console.log(isValid("([)]")); // false
console.log(isValid("{[]}")); // true
