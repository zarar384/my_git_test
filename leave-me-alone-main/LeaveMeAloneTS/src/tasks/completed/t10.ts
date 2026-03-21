function isPalindrome2(s: string): boolean {
  if (s.length === 0) return false;

  var t = (a: string, reverse: boolean = false): string => {
    var pattern = /[^a-zA-Z0-9]/g;
    var r = s
      .replace(pattern, "")
      .split("")
      .map((i) => i.trim());
    if (reverse === true) {
      r = r.reverse();
    }
    return r.join("").toLocaleLowerCase();
  };

  return t(s) == t(s, true);
}

var s = "A man, a plan, a canal: Panama";
var rr = isPalindrome2(s);
console.log(rr);
