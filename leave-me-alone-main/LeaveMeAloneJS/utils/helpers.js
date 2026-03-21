"use strict";
exports.__esModule = true;
function executionTimeAsync(fn, isLog) {
    if (isLog === void 0) { isLog = false; }
    var start = performance.now();
    try {
        var result = fn();
        var end = performance.now();
        if (isLog)
            console.log(result);
        return end - start;
    }
    catch (error) {
        console.error("Error during function execution: ".concat(error));
    }
}
exports["default"] = executionTimeAsync;
