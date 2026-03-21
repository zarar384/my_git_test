function evalRPN(tokens: string[]): number {
  var stack: number[] = [];
  const operators: Map<string, (a: number, b: number) => number> = new Map([
    ["+", (a, b) => a + b],
    ["-", (a, b) => a - b],
    ["*", (a, b) => a * b],
    ["/", (a, b) => Math.trunc(a / b)],
  ]);

  try {
    for (const token of tokens) {
      if (operators.has(token)) {
        if (stack.length < 2) throw new Error();

        const b = stack.pop()!;
        const a = stack.pop()!;
        const operation = operators.get(token)!;
        stack.push(operation(a, b));
      } else {
        const n: number = parseInt(token);
        if (isNaN(n)) throw new Error();
        stack.push(n);
      }
    }

    if (stack.length !== 1) throw new Error();
  } catch {
    return 0;
  }

  return stack[0];
}

export function runEvalRPNDemo() {
  var tokens = [
    "10",
    "6",
    "9",
    "3",
    "+",
    "-11",
    "*",
    "/",
    "*",
    "17",
    "+",
    "5",
    "+",
  ];
  var rre = evalRPN(tokens);
  console.log(rre);
}
