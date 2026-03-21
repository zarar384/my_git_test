function isPalindrome(s) {
    if (s.length === 0)
        return true;
    return s.split("").reverse().toString() === s;
}
var s = "A man, a plan, a canal: Panama";
var rr = isPalindrome(s);
console.log(rr);
