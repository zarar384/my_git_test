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
        // rating: 4.7,
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

// indexes for efficient querying
db.books.createIndex({ isbn: 1}, {unique: true});
db.books.createIndex({genre: 1, rating: -1}); // -1 for descending order
db.books.createIndex({title: "text", tags: "text"}, {name: "text_search_index"}); // define a name for the text index
db.members.createIndex({location: "2dsphere"}); // for geospatial queries, where 2dsphere is used for spherical geometry (Earth-like) queries

// TTL index for automatic deletion of old notifications after 30 days
db.createCollection("notifications");
db.notifications.createIndex({ createdAt: 1 }, { expireAfterSeconds: 60 * 60 * 24 * 30 }); // 30 days in seconds

// show indexes for books collection
print("Books collection indexes:");
var bookIndexes = db.books.getIndexes();

printjson(bookIndexes);
bookIndexes;

// FIND queries
print("Book with genre 'Non-Fiction' and rating greater than 4.5:");
var booksGenreNonFiction = db.books.find(
    { genre: "Non-Fiction", rating: { $gt: 4.5 } },
     {title: 1, authors: 1, rating: 1, _id: 0} // projection to show only title, authors and rating
).sort({ rating: -1 }); // sort by rating in descending order

printjson(booksGenreNonFiction.toArray());
booksGenreNonFiction;

print("Books with tag 'war':");
var booksWithTagWar = db.books.find(
    {tags: "war"},
    {title: 1, tags: 1, _id: 0} // projection to show only title and tags
).sort({publishedYear: -1});

printjson(booksWithTagWar.toArray());
booksWithTagWar;

print("Books with tags 'war' and 'classic':");
var booksWithTagsWarClassic = db.books.find(
    {tags: {$all: ["war", "classic"]}},
    {title: 1, _id: 0} // projection to show only title
).sort({publishedYear: -1});

printjson(booksWithTagsWarClassic.toArray());
booksWithTagsWarClassic;

// $expr: compare filds within the same document
print("Expired loans (dueDate passed, returnedAt is empty):"); 
var expiredLoans = db.loans.find(
    {
        $expr:
        {
            $and: [
                { $lt: ["$dueDate", new Date()] }, // dueDate is less than current date
                { $eq: ["$returnedAt", null] } // returnedAt is null
            ]
        }
    }
);

printjson(expiredLoans.toArray());
expiredLoans;

// books on shelf A
print("Books located on shelf A:");
var booksOnShelfA = db.books.aggregate([
    {
        $match: {"location.shelf": "A"}
    },
    {
        $project: {
            shelf: "$location.shelf",
            title: "$title",
            row: "$location.row",
            _id: 0
        }
    },
    {
        $sort: {shelf: 1, row: 1} // sort by shelf and row in ascending order
    }
]);

printjson(booksOnShelfA.toArray());
booksOnShelfA;

// text search $text works only with text indexes
var booksTextSearch = db.books.find(
    { $text: { $search: "world" } },
    { score: { $meta: "textScore" }, title: 1, tags: 1, _id: 0 } // projection to show only title and tags
).sort({ score: { $meta: "textScore" } }); // sort by text score in descending order

printjson(booksTextSearch.toArray());
booksTextSearch;

// $size and $exists operators
// $size is used to match documents where the array field has a specific number of elements
// $exists is used to match documents where the field exists or does not exist
print("Books with exactly 3 tags:");
var booksWithThreeTags = db.books.find(
    { tags: { $size: 3 } },
    { title: 1, tags: 1, _id: 0 } // projection to show only title and tags
).sort({ title: 1 }); // sort by title in ascending order

printjson(booksWithThreeTags.toArray());
booksWithThreeTags;

// books with rating field does not exist
print("Books with rating field not existing:");
var booksWithRatingField = db.books.find(
    { rating: { $exists: false } },
    { title: 1, rating: 1, _id: 0 } 
).sort({ rating: -1 }); 

printjson(booksWithRatingField.toArray());
booksWithRatingField;

// $in and $nin operators
print("Books with author 'Louis-Ferdinand Céline' or 'Svetlana Alexievich' and rating less than 4:");
var booksWithGenreIn = db.books.find(
    {
        $and: [
            { authors: { $in: ["Louis-Ferdinand Céline", "Svetlana Alexievich"] } },
            { rating: { $lt: 4 } }
        ]
    },
    { title: 1, authors: 1, rating: 1, _id: 0 }
).sort({ rating: -1 });

printjson(booksWithGenreIn.toArray());
booksWithGenreIn;

// pagination
const PAGE = 1;
const PAGE_SIZE = 3;
print(`Books on page ${PAGE} with page size ${PAGE_SIZE}:`);
var booksPagination = db.books.find(
    {},
    { title: 1, authors: 1, _id: 0 },
)
.sort({ title: 1 })
.skip((PAGE - 1) * PAGE_SIZE)
.limit(PAGE_SIZE)
.toArray()

printjson(booksPagination);
booksPagination;

// next page
const NEXT_PAGE = 2;
print(`Books on page ${NEXT_PAGE} with page size ${PAGE_SIZE}:`);
var booksNextPage = db.books.find(
    {},
    { title: 1, authors: 1, _id: 0 },
)
.sort({ title: 1 })
.skip((NEXT_PAGE - 1) * PAGE_SIZE)
.limit(PAGE_SIZE)
.toArray()

printjson(booksNextPage);
booksNextPage;

// $set, $inc, $push
print("Increase stock of 'Journey to the End of the Night' by 5 and add a new tag 'classic literature':");
var oldBook = db.books.findOne({ title: journeyToTheEndOfTheNightTitle });
var updateResult = db.books.updateOne(
    { title: journeyToTheEndOfTheNightTitle },
    { 
        $inc: {stock: 5}, // increment stock by 5
      $push: {tags: "classic literature"}, // add a new tag to the tags array
        $set: {lastUpdated: new Date()} // set lastUpdated field to current date
    }
);
var updatedBook = db.books.findOne({ title: journeyToTheEndOfTheNightTitle });

var comparisonResult = {
    oldBookStock: oldBook.stock,
    updatedBookStock: updatedBook.stock,
    oldBookTags: oldBook.tags,
    updatedBookTags: updatedBook.tags,
    previousLastUpdated: oldBook.lastUpdated,
    dateUpdated: updatedBook.lastUpdated.toISOString().split('T')[0]
};
printjson(comparisonResult);
comparisonResult;

// $addToSet 
print("Add a new tag '20th century' to 'Journey to the End of the Night' only if it doesn't already exist:");
var addToSetResult = db.books.updateOne(
    { title: journeyToTheEndOfTheNightTitle },
    { $addToSet: { tags: "20th century" } } // add a new tag to the tags array only if it doesn't already exist
);
var updatedBookAfterAddToSet = db.books.findOne({ title: journeyToTheEndOfTheNightTitle });

print("Add a new tag 'witnesses' to 'The Last Witnesses' only if it doesn't already exist (it already exists, so it will do nothing):");
var addToSetDoNothingResult = db.books.updateOne(
    { title: theLastWitnessesTitle },
    { $addToSet: { tags: "witnesses" } } // is already in the tags array, so it will do nothing
);
var updatedBookAfterAddToSetDoNothing = db.books.findOne({ title: theLastWitnessesTitle });

var addToSetComparisonResult = {
    updatedBookAfterAddToSetTags: updatedBookAfterAddToSet.tags,
    updatedBookAfterAddToSetDoNothingTags: updatedBookAfterAddToSetDoNothing.tags
};
printjson(addToSetComparisonResult);
addToSetComparisonResult;

// $pull - remove existing tag from the tags array
print("Remove the tag 'existentialism' from 'Journey to the End of the Night':");
var pullResult = db.books.updateOne(
    { title: journeyToTheEndOfTheNightTitle },
    { $pull: { tags: "existentialism" } } // remove the tag from the tags array
);
var updatedBookAfterPull = db.books.findOne({ title: journeyToTheEndOfTheNightTitle });

var pullComparisonResult = {
    updatedBookAfterAddToSetTags: updatedBookAfterAddToSet.tags,
    updatedBookAfterPullTags: updatedBookAfterPull.tags
};
printjson(pullComparisonResult);
pullComparisonResult;

// colMod - modify the validation rules for the books collection to add a new enum value for the genre field
print("Modify the validation rules for the books collection to add a new enum value 'Prison Literature' for the genre field:");
// get the current validation rules, modify and then apply the new validation rules using collMod command
var info = db.getCollectionInfos({ name: "books" })[0];
var currentValidator = info.options.validator;
currentValidator.$jsonSchema.properties.genre.enum.push("Prison Literature");

var collModResult = db.runCommand({
    collMod: "books",
    validator: currentValidator,
    validationAction: "error"
});

// upsert - if the book does not exist, it will be created; if it exists, it will be updated
print("Upsert a book 'Cheshezhopitsa' (it does not exist, so it will be created):");
const cheshezhopitsaTitle = "Cheshezhopitsa";
var upsertResult = db.books.updateOne(
    { isbn: "9781234567890" }, // filter by ISBN
    {
        $set: {
            title: cheshezhopitsaTitle,
            authors: ["Nekras Ryzhijr"],
            isbn: "9781234567890",
            genre: "Prison Literature",
            pages: 200,
            publishedYear: 2024,
            stock: 5,
            tags: ["prison literature"],
            location: { shelf: "D", row: 1 }
        }
    },
    { upsert: true } // if the book does not exist, it will be created
);

var cheshezhopitsaBook = db.books.findOne({ isbn: "9781234567890" });

print("Upsert again the book 'Cheshezhopitsa' (it exists, so it will be updated):");
var upsertUpdateResult = db.books.updateOne(
    { isbn: "9781234567890" }, // filter by ISBN
    {
        $set: {
            tags: ["prison literature", "russian literature"],
        }
    },
    { upsert: true } // if the book does not exist, it will be created
);

var cheshezhopitsaBookUpdated = db.books.findOne({ isbn: "9781234567890" });

var upsertComparisonResult = {
    cheshezhopitsaBookTags: cheshezhopitsaBook.tags,
    cheshezhopitsaBookUpdatedTags: cheshezhopitsaBookUpdated.tags
};

printjson(upsertComparisonResult);
upsertComparisonResult;

// updateMany + $mul - mutiply the stock of all books in the genre "Fiction" by 2
print("Multiply the stock of all books in the genre 'Fiction' by 2:");
var updateManyResult = db.books.updateMany(
    { genre: "Fiction" },
    { $mul: { stock: 2 } } // multiply the stock by 2
);

var updatedFictionBooks = db.books.find(
    { genre: "Fiction" },
    {
        title: 1, stock: 1, _id: 0,
        isMultipliedByTwo: // stock%2 == 0, then it is multiplied by 2
        {
            $eq: [
                { $mod: ["$stock", 2] }, 0  // $mod (modulo) returns the remainder after division by 2
            ] 
        }
    }
).sort({ title: 1 });

printjson(updatedFictionBooks.toArray());
updatedFictionBooks;

// findOneAndUpdate
print("Find one book 'Guignol's Band' and update location to shelf 'E' and row 1:");
var findOneAndUpdateResult = db.books.findOneAndUpdate(
    { title: guignolsBandTitle },
    { $set: { "location.shelf": "E", "location.row": 1 } },
    {
        returnDocument: "after", // return the updated document
        projection: { title: 1, location: 1, _id: 0 }
    },
);

printjson(findOneAndUpdateResult);
findOneAndUpdateResult;

// base pipeline for aggregation: match -> group -> project -> sort
print("Aggregation pipeline: Count the number of books per genre and sort by count in descending order:");
var aggregationPipeline = db.books.aggregate([
    {
        $match: { stock: { $gt: 0 } } // match books with stock greater than 0
    },
    {
        $group: {
            _id: "$genre", 
            count: { $sum: 1 } 
        }
    },
    {
        $project: {
            genre: "$_id",
            count: 1,
        }
    },
    {
        $sort: { count: -1 } 
    }
]);

printjson(aggregationPipeline.toArray());
aggregationPipeline;

// $lookup - join books and loans collections to get the book title and member name for each loan
/*
SQL equivalent:
SELECT books.title, members.name, loans.borrowedAt, loans.dueDate, loans.returnedAt, loans.status
FROM loans
JOIN books ON loans.bookId = books._id
JOIN members ON loans.memberId = members._id;
*/
print("Aggregation pipeline: Join books and loans collections to get the book title and member name for each loan:");
var lookupPipeline = db.loans.aggregate([
    {
        $lookup: {
            from: "books",
            localField: "bookId",
            foreignField: "_id",
            as: "book"
        }
    },
    {
        $unwind: "$book" // unwind the book array to get a single book document
    },
    {
        $lookup: {
            from: "members",
            localField: "memberId",
            foreignField: "_id",
            as: "member"
        }
    },
    {
        $unwind: "$member" // unwind the member array to get a single member document
    },
    {
        $project: {
            bookTitle: "$book.title",
            memberName: "$member.name",
            borrowedAt: 1,
            dueDate: 1,
            returnedAt: 1,
            status: 1,
            _id: 0
        }
    },
    {
        $sort: { borrowedAt: -1 } // sort by borrowedAt in descending order
    }
]);

printjson(lookupPipeline.toArray());
lookupPipeline;

// $lookup with pipeline 
/* SQL equivalent:
SELECT books.title, members.name, loans.borrowedAt, loans.dueDate, loans.returnedAt
FROM loans
JOIN books ON loans.bookId = books._id
JOIN members ON loans.memberId = members._id
WHERE loans.status = 'borrowed' AND members.membershipType = 'Premium';
*/
print("Aggregation pipeline: All borrowed loans with book title and member name, only for Premium members:");
var lookupWithPipeline = db.loans.aggregate([
    {
        $match: { status: "borrowed" } // filter only borrowed loans
    },
    {
        $lookup: {
            from: "books",
            let: { bookId: "$bookId" },
            pipeline: [
                { $match: { 
                    $expr: {
                        $and: [
                            { 
                                $eq: ["$_id", "$$bookId"] ,
                            },
                        ]
                    }
                }
                 }, 
                { $project: { title: 1, _id: 0 } }
            ],
            as: "book"
        },
    },
    {
        $unwind: "$book"
    },
    {
        $lookup: {
            from: "members",
            let: { memberId: "$memberId" },
            pipeline: [
                { $match: { $expr: { 
                    $and: [
                        { $eq: ["$_id", "$$memberId"] },
                        { $eq: ["$membershipType", "Premium"] } 
                    ]
                 } } },
                { $project: { name: 1, _id: 0 } }
            ],
            as: "member"
        },
    },
    {
        $unwind: "$member"
    },
    {
        $project: {
            bookTitle: "$book.title",
            memberName: "$member.name",
            borrowedAt: 1,
            dueDate: 1,
            returnedAt: 1
        }
    }
]);

printjson(lookupWithPipeline.toArray());
lookupWithPipeline;

// $unwind + $group - count the number of books per author
print("Aggregation pipeline: Count the number of books per author:");
var unwindGroupPipeline = db.books.aggregate([
    {
        $unwind: "$authors" // unwind the authors array to get a single author document
    },
    {
        $group: {
            _id: "$authors",
            count: { $sum: 1 } // count the number of books per author
        }
    },
    {
        $project: {
            author: "$_id",
            count: 1,
            _id: 0
        }
    },
    {
        $sort: { count: -1 } // sort by count in descending order
    }
]);

printjson(unwindGroupPipeline.toArray());
unwindGroupPipeline;

// $bucket - group books by ranges
// push new book 'Anthology of Tolkien' with 1200 pages to the books collection
const anthologyOfTolkienTitle = "Anthology of Tolkien";
db.books.insertOne({
    title: anthologyOfTolkienTitle,
    authors: ["J.R.R. Tolkien"],
    isbn: "9781234567891",
    genre: "Fiction",
    pages: 1200,
    publishedYear: 2024,
    stock: 5,
    tags: ["fantasy", "classic"],
    location: { shelf: "D", row: 2 }
});

print("Aggregation pipeline: Group books by number of pages into buckets:");
var bucketPipeline = db.books.aggregate([
    {
        $bucket: {
            groupBy: "$pages",
            boundaries: [0, 200, 400, 600, 1000], // define the ranges
            default: "1000+", // default bucket for values outside the boundaries
            output: {
                count: { $sum: 1 }, // count the number of books in each bucket
                titles: { $push: "$title" } // push the titles of the books in each bucket
            }
        }
    }
]);

printjson(bucketPipeline.toArray());
bucketPipeline;

// $facet - multiple aggregations in a single query
print("Aggregation pipeline: Multiple aggregations in a single query:");
// update the stock of 'Anthology of Tolkien' to 0 to test the outOfStock facet
db.books.updateOne(
    { title: anthologyOfTolkienTitle },
    { $set: { stock: 0 } }
);

var facetPipeline = db.books.aggregate([
    {
        $facet: {
            // Count the number of books per genre
            byGenre: [
                { $group: { _id: "$genre", count: { $sum: 1 } } },
                { $sort: { count: -1 } }
            ],
            // average rating of books
            avgRating: [
                { $group: { _id: null, avg: { $avg: "$rating" } } },
                { $project: { _id: 0, avg: { $round: ["$avg", 2] } } }
            ],
            // books withou stock
            outOfStock: [
                { $match: { stock: 0 } },
                { $project: { title: 1, _id: 0 } }
            ]
        }
    }
]);

printjson(facetPipeline.toArray());
facetPipeline;

// $addFields + $cond - add a new field 'availability' and 'isClassic' based on the stock and publishedYear of the book
print("Aggregation pipeline: Add a new field 'availability' and 'isClassic' based on the stock and publishedYear of the book:");
var addFieldsPipeline = db.books.aggregate([
    {
        $addFields: {
            availability: {
                $cond: {
                    if: { $gt: ["$stock", 0] }, // if stock is greater than 0
                    then: "available", // then availability is "available"
                    else: "out of stock" // else availability is "out of stock"
                }
            },
            isClassic: {
                $cond: {
                    if: { $lt: ["$publishedYear", 1970] }, // if publishedYear is less than 1970
                    then: true, // then isClassic is true
                    else: false // else isClassic is false
                }
            }
        }
    },
    {
        $project: {
            _id: 0, title: 1, stock: 1, availability: 1, publishedYear: 1, isClassic: 1
        }
    }
]);

printjson(addFieldsPipeline.toArray());
addFieldsPipeline;