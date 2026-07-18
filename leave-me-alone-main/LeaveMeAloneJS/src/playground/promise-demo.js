// This function returns a promise that resolves to another promise.
// The inner promise resolves after 1 second with the value 'resolved'.
// The outer promise resolves immediately with the inner promise,
// so awaiting the outer promise waits until the inner promise resolves.
function alreadyResolvedPromise() {
    return new Promise((resolveOuter) => {
        const inner = new Promise((resolveInner) => {
            console.log("Inner: pending...");

            setTimeout(() => {
                console.log("Inner promise resolved");
                resolveInner('resolved');
            }, 1000);
        });

        console.log("Outer now follows inner");
        resolveOuter(inner);
        console.log("resolveOuter returned immediately");
    });
}

// This function demonstrates awaiting a promise that resolves to another promise.
async function runAlreadyResolvedPromiseDemo() {
    const outerPromise = alreadyResolvedPromise();

    console.log('waiting...');
    var result = await outerPromise;
    console.log('result:', result);
}

// This function returns a promise that resolves to the square of a number after a delay.
function squareNumChainPromise(num, times) {
    if (!Number.isInteger(num))
        return Promise.reject(new Error("num must be an integer"));

    if (!Number.isInteger(times) || times < 0)
        return Promise.reject(new Error("times must be a non-negative integer"));

    let myPromise = Promise.resolve(num);

    for (let i = 0; i < times; i++) {
        myPromise = myPromise.then(res =>
            new Promise(resolve => {
                setTimeout(() => {
                    const squared = res * res;
                    console.log(`Squaring ${res}... result ${squared}`);
                    resolve(squared);
                }, 1000);
            })
        );
    }

    return myPromise;
}

// This function demonstrates chaining promises to square a number multiple times.
async function runSquareNumChainPromiseDemo(num = 2, times = 3) {
    try {
        const result = await squareNumChainPromise(num, times);
        console.log('Final result:', result);
    } catch (error) {
        console.error('Error:', error.message);
    }
}

class UserInput {
    constructor(name = '', email = '', age = 0) {
        this.name = name;
        this.email = email;
        this.age = age;
    }
}

// Airport registration input validation.
// Validates a UserInput object by chaining promise-based validation steps.
// Each validation returns a promise that resolves with the original user object
// if the validation succeeds, or rejects with an error if it fails.
// The validations ensure that:
// * name is a non-empty string,
// * email contains the '@' character,
// * age is a positive integer.
//
// @param {UserInput} userInput - The user input to validate.
// @returns {Promise<UserInput>} A promise that resolves with the validated user input
// or rejects with a validation error.
function userInputValidationChainPromiseDemo(userInput) {

    if (!(userInput instanceof UserInput)) {
        return Promise.reject(new Error("Invalid user input"));
    }

    function validateName(user) {
        return new Promise((resolve, reject) => {
            if (
                typeof user.name === "string" &&
                user.name.trim() !== ""
            ) {
                resolve(user);
            } else {
                reject(new Error("Invalid name"));
            }
        });
    }

    function validateEmail(user) {
        return new Promise((resolve, reject) => {
            if (
                typeof user.email === "string" &&
                user.email.includes("@")
            ) {
                resolve(user);
            } else {
                reject(new Error("Invalid email"));
            }
        });
    }

    function validateAge(user) {
        return new Promise((resolve, reject) => {
            if (
                Number.isInteger(user.age) &&
                user.age > 14
            ) {
                resolve(user);
            } else {
                reject(new Error("Invalid age"));
            }
        });
    }

    return Promise.resolve(userInput)
        .then(validateName)
        .then(validateEmail)
        .then(validateAge)
        .catch(error => {
            console.error("Validation failed:", error.message);
            throw error;
        });
}

// This function demonstrates validating user input using a chain of promises.
async function runUserInputValidationChainPromiseDemo() {
    const aliceSuccess = new UserInput("Alice", "alice@example.com", 25);
    const bobFailure = new UserInput("Bob", "bobexample.com", 13);

    const aliceResult = userInputValidationChainPromiseDemo(aliceSuccess);
    const bobResult = userInputValidationChainPromiseDemo(bobFailure);

    const results = await Promise.allSettled([aliceResult, bobResult]);

    results.forEach((result, index) => {
        const name = index === 0 ? "Alice" : "Bob";

        if (result.status === "fulfilled") {
            console.log(`${name} validation succeeded:`, result.value);
        } else {
            console.error(`${name} validation failed:`, result.reason.message);
        }
    });
}

export default {
    runAlreadyResolvedPromiseDemo,
    runSquareNumChainPromiseDemo,
    runUserInputValidationChainPromiseDemo,
};