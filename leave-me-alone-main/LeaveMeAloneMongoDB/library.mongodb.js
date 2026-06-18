use("LibraryDB");
db.dropDatabase();

// Books collection
db.createCollection("books",{
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["title", "authors", "isbn", "genre", "pages", "publishedYear", "stock"],
            properties: {
                title: {
                    bsonType: "string",
                    minLength: 1,
                    description: "Title of the book must be a non-empty string"
                },
                authors: {
                    bsonType: "array",
                    minItems: 1,
                    items: {
                        bsonType: "string",
                    },
                    description: "Authors must be an array of non-empty strings"
                },
                isbn: {
                    bsonType: "string",
                    pattern: "^[0-9]{13}$", // International Standard Book Number (ISBN). Example: "9781234567890"
                    description: "ISBN must be a 13-digit string"
                },
                genre: {
                    bsonType: "string",
                    enum: ["Fiction", "Non-Fiction", "Science Fiction", "Fantasy", "Biography", "History", "Children"],
                    description: "Genre must be one of the predefined categories"
                },
                pages: {
                    bsonType: "number",
                    minimum: 1,
                    description: "Pages must be a positive integer"
                },
                publishedYear: {
                    bsonType: "number",
                    minimum: 1450, // The year the printing press was invented,
                    maximum: new Date().getFullYear(), // Current year
                    description: "Published year must be a valid year"
                },
                rating: {
                    bsonType: "number",
                    minimum: 0.0,
                    maximum: 5.0
                },
                stock: {
                    bsonType: "number",
                    minimum: 0,
                },
                tags: {
                    bsonType: "array",
                    items: {
                        bsonType: "string"
                    },
                    description: "Tags must be an array of strings"
                },
                location: {
                    bsonType: "object",
                    required: ["row", "shelf"], // Location of the book in the library
                    properties: {
                        shelf: {
                            bsonType: "string",
                            description: "Shelf must be a string"
                        },
                        row: {
                            bsonType: "number",
                            minimum: 1,
                            description: "Row must be a positive integer"
                        }
                    }
                }
            }
        }
    },
    validationAction: "error" // Reject documents that do not meet the schema
});

// Luis-Ferdinand Céline's books
const journeyToTheEndOfTheNightTitle = "Journey to the End of the Night";
const deathOnTheInstallmentPlanTitle = "Death on the Installment Plan";
const guignolsBandTitle = "Guignol's Band";
const fableForAnotherTimeTitle = "Fable for Another Time";
const castleToCastleTitle = "Castle to Castle";
const northTitle = "North";
const rigadoonTitle = "Rigadoon";

db.books.insertMany([
    {
        title: journeyToTheEndOfTheNightTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189756",
        genre: "Fiction",
        pages: 505,
        publishedYear: 1932,
        rating: 4.5,
        stock: 10,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 1 }
    },
    {
        title: deathOnTheInstallmentPlanTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189763",
        genre: "Fiction",
        pages: 400,
        publishedYear: 1936,
        rating: 4.0,
        stock: 5,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 2 }
    },
    {
        title: guignolsBandTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189770",
        genre: "Fiction",
        pages: 350,
        publishedYear: 1944,
        rating: 3.5,
        stock: 7,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 3 }
    },
    {
        title: fableForAnotherTimeTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189787",
        genre: "Fiction",
        pages: 300,
        publishedYear: 1952,
        rating: 4.2,
        stock: 8,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 4 }
    },
    {
        title: castleToCastleTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189794",
        genre: "Fiction",
        pages: 450,
        publishedYear: 1957,
        rating: 4.1,
        stock: 6,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 5 }
    },
    {
        title: northTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189800",
        genre: "Fiction",
        pages: 400,
        publishedYear: 1960,
        rating: 3.9,
        stock: 4,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 6 }
    },
    {
        title: rigadoonTitle,
        authors: ["Louis-Ferdinand Céline"],
        isbn: "9780141189817",
        genre: "Fiction",
        pages: 350,
        publishedYear: 1969,
        rating: 3.8,
        stock: 3,
        tags: ["classic", "war", "existentialism"],
        location: { shelf: "A", row: 7 }
    }
]);

// Svetlana Alexievich's books
const warsUnwomanlyFaceTitle = "War's Unwomanly Face";
const voicesFromChernobylTitle = "Voices from Chernobyl";
const secondHandTimeTitle = "Second-Hand Time";
const theLastWitnessesTitle = "The Last Witnesses";
const zinkyBoysTitle = "Zinky Boys";

db.books.insertMany([
    {
        title: warsUnwomanlyFaceTitle,
        authors: ["Svetlana Alexievich"],
        isbn: "9780393324820",
        genre: "Non-Fiction",
        pages: 400,
        publishedYear: 1983,
        rating: 4.6,
        stock: 12,
        tags: ["war", "oral history", "women"],
        location: { shelf: "B", row: 1 }
    },  
    {
        title: voicesFromChernobylTitle,
        authors: ["Svetlana Alexievich"],
        isbn: "9780393324837",
        genre: "Non-Fiction",
        pages: 350,
        publishedYear: 1997,
        rating: 4.7,
        stock: 15,
        tags: ["nuclear disaster", "oral history", "Chernobyl"],
        location: { shelf: "B", row: 2 }
    },
    {
        title: secondHandTimeTitle,
        authors: ["Svetlana Alexievich"],
        isbn: "9780393324844",
        genre: "Non-Fiction",
        pages: 500,
        publishedYear: 2013,
        rating: 4.8,
        stock: 20,
        tags: ["Soviet Union", "oral history", "post-Soviet"],
        location: { shelf: "B", row: 3 }
    },
    {
        title: theLastWitnessesTitle,
        authors: ["Svetlana Alexievich"],
        isbn: "9780393324851",
        genre: "Non-Fiction",
        pages: 300,
        publishedYear: 2015,
        rating: 4.5,
        stock: 10,
        tags: ["war", "oral history", "witnesses"],
        location: { shelf: "B", row: 4 }
    },
    {
        title: zinkyBoysTitle,
        authors: ["Svetlana Alexievich"],
        isbn: "9780393324868",
        genre: "Non-Fiction",
        pages: 250,
        publishedYear: 1989,
        rating: 4.4,
        stock: 8,
        tags: ["war", "oral history", "Afghanistan"],
        location: { shelf: "B", row: 5 }
    }
]);

// Erich Maria Remarque's books
const allQuietOnTheWesternFrontTitle = "All Quiet on the Western Front";
const theRoadBackTitle = "The Road Back";
const threeComradesTitle = "Three Comrades";
const archOfTriumphTitle = "Arch of Triumph";

db.books.insertMany([
    {
        title: allQuietOnTheWesternFrontTitle,
        authors: ["Erich Maria Remarque"],
        isbn: "9780449213940",
        genre: "Fiction",
        pages: 296,
        publishedYear: 1929,
        rating: 4.7,
        stock: 10,
        tags: ["war", "classic", "World War I"],
        location: { shelf: "C", row: 1 }
    },
    {
        title: theRoadBackTitle,
        authors: ["Erich Maria Remarque"],
        isbn: "9780449213957",
        genre: "Fiction",
        pages: 320,
        publishedYear: 1931,
        rating: 4.5,
        stock: 8,
        tags: ["war", "classic", "World War I"],
        location: { shelf: "C", row: 2 }
    },
    {
        title: threeComradesTitle,
        authors: ["Erich Maria Remarque"],
        isbn: "9780449213964",
        genre: "Fiction",
        pages: 288,
        publishedYear: 1936,
        rating: 4.6,
        stock: 12,
        tags: ["war", "classic", "World War I"],
        location: { shelf: "C", row: 3 }
    },
    {
        title: archOfTriumphTitle,
        authors: ["Erich Maria Remarque"],
        isbn: "9780449213971",
        genre: "Fiction",
        pages: 352,
        publishedYear: 1945,
        rating: 4.4,
        stock: 6,
        tags: ["war", "classic", "World War II"],
        location: { shelf: "C", row: 4 }
    }
]);

// Members collection
db.createCollection("members", {
    validator: {
        $jsonSchema: {
            bsonType: "object",
            required: ["name", "email", "membershipType", "joinedAt"],
            properties: {
                name: {
                    bsonType: "string",
                    minLength: 1,
                    description: "Name must be a non-empty string"
                },
                email: {
                    bsonType: "string",
                    pattern: "^.+@.+\\..+$",
                    description: "Email must be a valid email address"
                },
                membershipType: {
                    bsonType: "string",
                    enum: ["Basic", "Premium", "Student"],
                    description: "Membership type must be one of the predefined categories"
                },
                joinedAt: {
                    bsonType: "date",
                    description: "Joined date must be a valid date"
                },
                address: {
                    bsonType: "object",
                    properties: {
                        city: {
                            bsonType: "string",
                            description: "City must be a string"
                        },
                        country: {
                            bsonType: "string",
                            description: "Country must be a string"
                        }
                    }
                },
                // coordinates for geospatial queries
                location: {
                    bsonType: "object",
                    properties: {
                        type: {
                            bsonType: "string",
                            description: "Type must be 'Point'"
                        },
                        coordinates: {
                            bsonType: "array",
                            description: "Coordinates must be an array of two numbers [longitude, latitude]",
                        }
                    }
                }
            }
        }
    }
});                     

const aliceName = "Alice Novak";
const bobName = "Bob Kovar";
const carolName = "Carol Dvořák";
const danName = "Dan Horák";

db.members.insertMany([
    {
        name: aliceName,
        email: "alice@example.com",
        membershipType: "Premium",
        joinedAt: new Date("2022-03-15"),
        address: { city: "Prague", country: "CZ" },
        location: { type: "Point", coordinates: [14.4208, 50.0880] }
    },
    {
        name: bobName,
        email: "bob@example.com",
        membershipType: "Student",
        joinedAt: new Date("2023-09-01"),
        address: { city: "Brno", country: "CZ" },
        location: { type: "Point", coordinates: [16.6068, 49.1951] }
    },
    {
        name: carolName,
        email: "carol@example.com",
        membershipType: "Basic",
        joinedAt: new Date("2021-11-20"),
        address: { city: "Prague", country: "CZ" },
        location: { type: "Point", coordinates: [14.4300, 50.0750] }
    },
    {
        name: danName,
        email: "dan@example.com",
        membershipType: "Premium",
        joinedAt: new Date("2020-05-10"),
        address: { city: "Ostrava", country: "CZ" },
        location: { type: "Point", coordinates: [18.2625, 49.8209] }
    }
]);

// Book loans collection
const alice = db.members.findOne({ name: aliceName });
const bob = db.members.findOne({ name: bobName });
const carol = db.members.findOne({ name: carolName });
const dan = db.members.findOne({ name: danName });

const zinkyBoys = db.books.findOne({ title: zinkyBoysTitle });
const rigadoon = db.books.findOne({ title: rigadoonTitle });
const voicesFromChernobyl = db.books.findOne({ title: voicesFromChernobylTitle });
const allQuietOnTheWesternFront = db.books.findOne({ title: allQuietOnTheWesternFrontTitle });
const journeyToTheEndOfTheNight = db.books.findOne({ title: journeyToTheEndOfTheNightTitle });
const secondHandTime = db.books.findOne({ title: secondHandTimeTitle });
const castleToCastle = db.books.findOne({ title: castleToCastleTitle });

db.loans.insertMany([
    {
        memberId: alice._id,
        bookId: zinkyBoys._id,
        borrowedAt: new Date("2024-01-10"),
        dueDate: new Date("2024-02-10"),
        returnedAt: new Date("2024-02-05"),
        status: "returned"
    },
    {
        memberId: bob._id,
        bookId: rigadoon._id,
        borrowedAt: new Date("2024-02-15"),
        dueDate: new Date("2024-03-15"),
        returnedAt: null,
        status: "borrowed"
    },
    {
        memberId: carol._id,
        bookId: voicesFromChernobyl._id,
        borrowedAt: new Date("2024-03-01"),
        dueDate: new Date("2024-04-01"),
        returnedAt: null,
        status: "borrowed"
    },
    {
        memberId: dan._id,
        bookId: allQuietOnTheWesternFront._id,
        borrowedAt: new Date("2024-01-20"),
        dueDate: new Date("2024-02-20"),
        returnedAt: new Date("2024-02-18"),
        status: "returned"
    },
    {
        memberId: alice._id,
        bookId: journeyToTheEndOfTheNight._id,
        borrowedAt: new Date("2024-02-25"),
        dueDate: new Date("2024-03-25"),
        returnedAt: null,
        status: "borrowed"
    },
    {
        memberId: bob._id,
        bookId: secondHandTime._id,
        borrowedAt: new Date("2024-03-05"),
        dueDate: new Date("2024-04-05"),
        returnedAt: null,
        status: "borrowed"
    },
    {
        memberId: carol._id,
        bookId: castleToCastle._id, 
        borrowedAt: new Date("2024-03-10"),
        dueDate: new Date("2024-04-10"),
        returnedAt: null,
        status: "borrowed"
    }
]);