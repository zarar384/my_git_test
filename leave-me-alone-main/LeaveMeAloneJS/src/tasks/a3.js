function evalRPN(tokens) {
    var stack = [];
    var operators = new Map([
        ["+", function (a, b) { return a + b; }],
        ["-", function (a, b) { return a - b; }],
        ["*", function (a, b) { return a * b; }],
        ["/", function (a, b) { return Math.trunc(a / b); }],
    ]);
    try {
        for (var _i = 0, tokens_1 = tokens; _i < tokens_1.length; _i++) {
            var token = tokens_1[_i];
            if (operators.has(token)) {
                if (stack.length < 2)
                    throw new Error();
                var b = stack.pop();
                var a = stack.pop();
                var operation = operators.get(token);
                stack.push(operation(a, b));
            }
            else {
                var n = parseInt(token);
                if (isNaN(n))
                    throw new Error();
                stack.push(n);
            }
        }
        if (stack.length !== 1)
            throw new Error();
    }
    catch (_a) {
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