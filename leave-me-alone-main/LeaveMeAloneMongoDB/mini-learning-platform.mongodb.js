const dbName = "LearningPlatform";
use(dbName);
const dropDb = true;
if (dropDb) db.dropDatabase();

const session = db.getMongo().startSession();
const dbSession = session.getDatabase(dbName);
session.startTransaction();

try {
    const collectionNames = db.getCollectionNames();

    const userName1 = "Alice Johnson";
    const userName2 = "Bob Smith";
    const userName3 = "Charlie Brown";

    if (!collectionNames.includes("users")) {
        /*
            {
                _id: ObjectId,
                name: String,
                email: String,
                level: String, // junior, middle, senior
                points: Number,
                createdAt: Date,
            }
        */
        dbSession.createCollection("users",
            {
                validator: {
                    $jsonSchema: {
                        bsonType: "object",
                        required: ["name", "email", "level", "points", "createdAt"],
                        properties: {
                            name: {
                                bsonType: "string",
                                description: "must be a string and is required"
                            },
                            email: {
                                // Simple regex for email validation
                                bsonType: "string",
                                pattern: "^\\S+@\\S+\\.\\S+$",
                                description: "must be a valid email address and is required"
                            },
                            level: {
                                bsonType: "string",
                                enum: ["junior", "middle", "senior"],
                                description: "must be one of 'junior', 'middle', 'senior' and is required"
                            },
                            points: {
                                bsonType: "int",
                                minimum: 0,
                                description: "must be a non-negative integer and is required"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "must be a date and is required"
                            }
                        }
                    }
                }
            });

        dbSession.users.insertMany([
            {
                name: userName1,
                email: "alice.johnson@example.com",
                level: "junior",
                points: 0,
                createdAt: new Date()
            },
            {
                name: userName2,
                email: "bob.smith@example.com",
                level: "middle",
                points: 0,
                createdAt: new Date()
            },
            {
                name: userName3,
                email: "charlie.brown@example.com",
                level: "senior",
                points: 0,
                createdAt: new Date()
            }
        ]);
    }

    const programmerCourse = "Programming 101";
    const designCourse = "Design Basics";
    const marketingCourse = "Marketing Strategies";
    const photographyCourse = "Photography Masterclass";

    if (!collectionNames.includes("courses")) {
        /*
            {
                _id: ObjectId,
                title: String,
                category: String,
                difficulty: String, // easy, medium, hard
                tags: [String],
                price: Number,
                studentsCount: Number,
                createdAt: Date,
            }
        */

        dbSession.createCollection("courses",
            {
                validator: {
                    $jsonSchema: {
                        bsonType: "object",
                        required: ["title", "category", "difficulty", "price", "studentsCount", "createdAt"],
                        properties: {
                            title: {
                                bsonType: "string",
                                description: "must be a string and is required"
                            },
                            category: {
                                bsonType: "string",
                                description: "must be a string and is required"
                            },
                            difficulty: {
                                bsonType: "string",
                                enum: ["easy", "medium", "hard"],
                                description: "must be one of 'easy', 'medium', 'hard' and is required"
                            },
                            tags: {
                                bsonType: "array",
                                items: {
                                    bsonType: "string"
                                },
                                description: "must be an array of strings"
                            },
                            price: {
                                bsonType: "double",
                                minimum: 0.01,
                                description: "must be a positive number and is required"
                            },
                            studentsCount: {
                                bsonType: "int",
                                minimum: 0,
                                description: "must be a non-negative integer and is required"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "must be a date and is required"
                            }
                        }
                    }
                }
            });

        dbSession.courses.insertMany([
            {
                title: programmerCourse,
                category: "Programming",
                difficulty: "easy",
                tags: ["programming", "coding", "basics"],
                price: 49.99,
                studentsCount: 0,
                createdAt: new Date()
            },
            {
                title: designCourse,
                category: "Design",
                difficulty: "medium",
                tags: ["design", "creativity", "visual"],
                price: 79.99,
                studentsCount: 0,
                createdAt: new Date()
            },
            {
                title: marketingCourse,
                category: "Marketing",
                difficulty: "hard",
                tags: ["marketing", "strategy", "business"],
                price: 99.99,
                studentsCount: 0,
                createdAt: new Date()
            },
            {
                title: photographyCourse,
                category: "Photography",
                difficulty: "medium",
                tags: ["photography", "camera", "skills"],
                price: 59.99,
                studentsCount: 0,
                createdAt: new Date()
            }
        ]);
    }

    const userAlice = dbSession.users.findOne({ name: userName1 });
    const userBob = dbSession.users.findOne({ name: userName2 });
    const userCharlie = dbSession.users.findOne({ name: userName3 });

    const courseProgramming = dbSession.courses.findOne({ title: programmerCourse });
    const courseDesign = dbSession.courses.findOne({ title: designCourse });
    const courseMarketing = dbSession.courses.findOne({ title: marketingCourse });
    const coursePhotography = dbSession.courses.findOne({ title: photographyCourse });

    if (!collectionNames.includes("enrollments")) {
        /*
            {
                _id: ObjectId,
                userId: ObjectId,
                courseId: ObjectId,
                progress: Number, // 0 to 100
                enrolledAt: Date,
            }
        */
        dbSession.enrollments.insertMany([
            {
                userId: userAlice._id,
                courseId: courseProgramming._id,
                progress: 20,
                enrolledAt: new Date()
            },
            {
                userId: userAlice._id,
                courseId: courseDesign._id,
                progress: 50,
                enrolledAt: new Date()
            },
            {
                userId: userBob._id,
                courseId: courseMarketing._id,
                progress: 10,
                enrolledAt: new Date()
            },
            {
                userId: userCharlie._id,
                courseId: coursePhotography._id,
                progress: 80,
                enrolledAt: new Date()
            },
            {
                userId: userCharlie._id,
                courseId: courseProgramming._id,
                progress: 60,
                enrolledAt: new Date()
            }
        ]);
    }

    if (!collectionNames.includes("reviews")) {
        /*
            {
                _id: ObjectId,
                userId: ObjectId,
                courseId: ObjectId,
                rating: Number, // 1 to 5
                comment: String,
                createdAt: Date,
            }
        */

        dbSession.reviews.insertMany([
            {
                userId: userAlice._id,
                courseId: courseProgramming._id,
                rating: 5,
                comment: "Great course!",
                createdAt: new Date()
            },
            {
                userId: userBob._id,
                courseId: courseDesign._id,
                rating: 4,
                comment: "Very informative.",
                createdAt: new Date()
            },
            {
                userId: userCharlie._id,
                courseId: courseMarketing._id,
                rating: 3,
                comment: "Good content, but could be better.",
                createdAt: new Date()
            }
        ]);
    }

    session.commitTransaction();
}
catch (error) {
    print("Error occurred during transaction: " + error);
    session.abortTransaction();
}
finally {
    session.endSession();
}

db.courses.createIndex({ title: 1 }, { unique: true });

// Update studentsCount for each course based on enrollments
db.courses.aggregate([
    {
        $lookup: {
            from: "enrollments",
            localField: "_id",
            foreignField: "courseId",
            as: "enrollments"
        }
    },
    {
        $set: {
            studentsCount: {
                $ifNull: [
                    { $size: "$enrollments" },
                    0
                ]
            }
        }
    },
    {
        // exclude enrollments array from the final output since only need to update studentsCount
        $project: {
            enrollments: 0
        }
    },
    {
        // use $merge to update the courses collection with the new studentsCount values
        $merge: {
            into: "courses",
            on: "_id",
            whenMatched: "merge", // merge the updated studentsCount value with existing course document
            whenNotMatched: "insert" // this case should not happen
        }
    }
]);

// lookup to get user and course details for each enrollment
db.enrollments.aggregate([
    {
        $lookup: {
            from: "courses",
            localField: "courseId",
            foreignField: "_id",
            as: "courseDetails"
        }
    },
    {
        $lookup: {
            from: "users",
            localField: "userId",
            foreignField: "_id",
            as: "userDetails"
        }
    },
    {
        $unwind: "$courseDetails"
    },
    {
        $unwind: "$userDetails"
    },
    {
        $project: {
            _id: 0,
            userName: "$userDetails.name",
            courseTitle: "$courseDetails.title",
            studentCount: "$courseDetails.studentsCount",
            progress: 1,
            enrolledAt: 1
        }
    }
]).pretty();

// course with price > 50 and studentsCount > 0
db.courses.find({
    price: { $gt: 50 },
    studentsCount: { $gt: 0 }
},
    {
        title: 1,
        price: 1,
        studentsCount: 1,
    }).pretty();

// agregate to calculate count of users by level
db.users.aggregate([
    {
        $group: {
            _id: "$level",
            count: { $sum: 1 }
        }
    },
    {
        $project: {
            _id: 0,
            level: "$_id",
            count: 1
        }
    }
]).pretty();

// agregate to calculate average price of courses by category
db.courses.aggregate([
    {
        $group: {
            _id: "$category",
            avgPrice: { $avg: "$price" }
        }
    },
    {
        $project: {
            _id: 0,
            category: "$_id",
            averagePrice: { $round: ["$avgPrice", 2] }
        }
    }
]).pretty();

// lookup to get course details for each review and filter reviews with rating >= 4
db.reviews.aggregate([
    {
        $lookup: {
            from: "courses",
            localField: "courseId",
            foreignField: "_id",
            as: "courseDetails"
        }
    },
    {
        $unwind: "$courseDetails"
    },
    {
        $match: {
            rating: { $gte: 4 }
        }
    },
    {
        $project: {
            _id: 0,
            courseTitle: "$courseDetails.title",
            rating: 1,
            comment: 1,
            createdAt: 1
        }
    }
]).pretty();