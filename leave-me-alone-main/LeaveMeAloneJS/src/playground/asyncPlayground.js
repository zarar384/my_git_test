export function runAsyncPlayground() {
    console.log("Async playground started");

    Promise.resolve(5)
        .then(x => x * 2)
        .then(console.log);
}