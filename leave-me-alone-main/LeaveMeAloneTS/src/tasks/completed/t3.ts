/*
Symbol       Value
I             1
V             5
X             10
L             50
C             100
D             500
M             1000
*/
function romanToInt(s: string): number {
  const romanMap: { [key: string]: number } = {
    I: 1,
    V: 5,
    X: 10,
    L: 50,
    C: 100,
    D: 500,
    M: 1000,
  };

  let resInt: number = 0;
  let previos: number = 0;

  for (let i = s.length - 1; i >= 0; i--) {
    const current = romanMap[s[i]];
    if (current >= previos) {
      resInt += current;
    } else {
      resInt -= current;
    }
    previos = current;
  }
  return resInt;
}

const roman: string = "MCMXCIV";
const resultInt: number = romanToInt(roman);
console.log(resultInt);
