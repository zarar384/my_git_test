use("shopDb");
const dropDb = true;
if (dropDb) db.dropDatabase();

const test1 = false;
const test2 = true;

// Create transaction session
const session = db.getMongo().startSession();
const sessionDb = session.getDatabase("shopDb");
session.startTransaction();

try {
    // Create the users collection with validation rules
    if (!db.getCollectionNames().includes("users")) {
        sessionDb.createCollection("users",
            {
                validator: {
                    $jsonSchema: {
                        bsonType: "object",
                        required: ["name", "email", "age"],
                        properties: {
                            name: {
                                bsonType: "string",
                                description: "Name is required and must be a string"
                            },
                            email: {
                                bsonType: "string",
                                pattern: "^.+@.+$",
                                description: "Email is required and must be a valid email address"
                            },
                            age: {
                                bsonType: "int",
                                minimum: 13,
                                maximum: 90,
                                description: "Age is required and must be an integer between 13 and 90"
                            },
                            address: {
                                bsonType: "object",
                                required: ["city", "street"],
                                properties: {
                                    city: {
                                        bsonType: "string",
                                        description: "City is required and must be a string"
                                    },
                                    street: {
                                        bsonType: "string",
                                        description: "Street is required and must be a string"
                                    }
                                },
                                description: "Address is an optional object with city and street"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "CreatedAt is an optional date field"
                            }
                        }
                    }
                }
            }
        );
    }

    if (!db.getCollectionNames().includes("products")) {
        // Create the products collection with validation rules
        sessionDb.createCollection("products",
            {
                validator: {
                    $jsonSchema: {
                        bsonType: "object",
                        required: ["name", "price", "category", "stock"],
                        properties: {
                            name: {
                                bsonType: "string",
                                maxLength: 100,
                                description: "Name is required and must be a string"
                            },
                            price: {
                                bsonType: "double",
                                minimum: 0.99,
                                description: "Price is required and must be a double"
                            },
                            category: {
                                bsonType: "string",
                                maxLength: 50,
                                description: "Category is required and must be a string"
                            },
                            tags: {
                                bsonType: "array",
                                items: {
                                    bsonType: "string",
                                    maxLength: 30,
                                    description: "Each tag must be a string"

                                }
                            },
                            stock: {
                                bsonType: "int",
                                minimum: 0,
                                description: "Stock is required and must be a non-negative integer"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "CreatedAt is an optional date field"
                            }
                        },
                        description: "Products collection with validation rules for name, price, category, and optional tags and createdAt"
                    }
                }
            }
        );
    }

    if (!db.getCollectionNames().includes("orders")) {
        // Create the orders collection with validation rules
        sessionDb.createCollection("orders",
            {
                validator: {
                    $jsonSchema: {
                        bsonType: "object",
                        required: ["userId", "products"],
                        properties: {
                            userId: {
                                bsonType: "objectId",
                                description: "UserId is required and must be an ObjectId referencing the users collection"
                            },
                            products: {
                                bsonType: "array",
                                items: {
                                    bsonType: "object",
                                    required: ["productId", "quantity", "priceAtPurchase"],
                                    properties: {
                                        productId: {
                                            bsonType: "objectId",
                                            description: "ProductId is required and must be an ObjectId referencing the products collection"
                                        },
                                        quantity: {
                                            bsonType: "int",
                                            minimum: 1,
                                            description: "Quantity is required and must be a positive integer"
                                        },
                                        priceAtPurchase: {
                                            bsonType: "double",
                                            minimum: 0.01,
                                            description: "PriceAtPurchase is required and must be a positive double"
                                        }
                                    },
                                    description: "Each product must be an object with productId, quantity, and priceAtPurchase"
                                },
                                description: "Products is required and must be an array of objects with productId, quantity, and priceAtPurchase"
                            },
                            totalAmount: {
                                bsonType: "double",
                                description: "TotalAmount is an optional double field that can be calculated based on the products in the order"
                            },
                            status: {
                                bsonType: "string",
                                enum: ["pending", "completed", "cancelled"],
                                description: "Status is an optional string field with allowed values"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "CreatedAt is an optional date field"
                            }
                        },
                        description: "Orders collection with validation rules for userId, products, totalAmount, and optional status and createdAt"
                    }
                }
            }
        );
    }

    if (!db.getCollectionNames().includes("reviews")) {
        sessionDb.createCollection("reviews",
            {
                validator: {
                    $jsonSchema: {
                        bsonType: "object",
                        required: ["userId", "productId", "rating"],
                        properties: {
                            userId: {
                                bsonType: "objectId",
                                description: "UserId is required and must be an ObjectId referencing the users collection"
                            },
                            productId: {
                                bsonType: "objectId",
                                description: "ProductId is required and must be an ObjectId referencing the products collection"
                            },
                            rating: {
                                bsonType: "int",
                                minimum: 1,
                                maximum: 5,
                                description: "Rating is required and must be an integer between 1 and 5",
                            },
                            comment: {
                                bsonType: "string",
                                maxLength: 500,
                                description: "Comment is an optional string field with a maximum length of 500 characters"
                            },
                            createdAt: {
                                bsonType: "date",
                                description: "CreatedAt is an optional date field"
                            },
                        },
                        description: "Reviews collection with validation rules for userId, productId, rating, and optional comment and createdAt"
                    }
                }
            }
        );
    }

    if (db.users.countDocuments() === 0) {
        // Insert user documents
        sessionDb.users.insertMany(
            [
                {
                    name: "Alice Smith",
                    email: "alice.smith@example.com",
                    age: 28,
                    address: {
                        city: "New York",
                        street: "123 Main St"
                    },
                    createdAt: new Date()
                },
                {
                    name: "Bob Johnson",
                    email: "bob.johnson@example.com",
                    age: 35,
                    address: {
                        city: "Los Angeles",
                        street: "456 Elm St"
                    },
                    createdAt: new Date()
                },
                {
                    name: "Charlie Brown",
                    email: "ebaka@sex.com",
                    age: 22,
                    address: {
                        city: "Chicago",
                        street: "789 Oak St"
                    },
                    createdAt: new Date()
                }
            ]
        );
    }

    if (db.products.countDocuments() === 0) {
        // Insert product documents
        sessionDb.products.insertMany(
            [
                {
                    name: "Wireless Mouse",
                    price: 29.99,
                    category: "Electronics",
                    stock: 100,
                    tags: ["wireless", "mouse", "computer accessories"],
                    createdAt: new Date()
                },
                {
                    name: "Bluetooth Headphones",
                    price: 59.99,
                    category: "Electronics",
                    stock: 50,
                    tags: ["bluetooth", "headphones", "audio"],
                    createdAt: new Date()
                },
                {
                    name: "Coffee Maker",
                    price: 89.99,
                    category: "Home Appliances",
                    stock: 30,
                    tags: ["coffee maker", "kitchen", "appliances"],
                    createdAt: new Date()
                },
                {
                    name: "Yoga Mat",
                    price: 19.99,
                    category: "Fitness",
                    stock: 200,
                    tags: ["yoga mat", "fitness", "exercise"],
                    createdAt: new Date()
                }
            ]
        );
    }

    if (db.orders.countDocuments() === 0) {
        // Insert order documents
        sessionDb.orders.insertMany(
            [
                {
                    userId: sessionDb.users.findOne({ name: "Alice Smith" })._id,
                    products: [
                        {
                            productId: sessionDb.products.findOne({ name: "Wireless Mouse" })._id,
                            quantity: 1,
                            priceAtPurchase: sessionDb.products.findOne({ name: "Wireless Mouse" }).price
                        }
                    ],
                    status: "pending",
                    createdAt: new Date()
                },
                {
                    userId: sessionDb.users.findOne({ name: "Bob Johnson" })._id,
                    products: [
                        {
                            productId: sessionDb.products.findOne({ name: "Bluetooth Headphones" })._id,
                            quantity: 3,
                            priceAtPurchase: sessionDb.products.findOne({ name: "Bluetooth Headphones" }).price
                        },
                        {
                            productId: sessionDb.products.findOne({ name: "Coffee Maker" })._id,
                            quantity: 1,
                            priceAtPurchase: sessionDb.products.findOne({ name: "Coffee Maker" }).price
                        }
                    ],
                    status: "pending",
                    createdAt: new Date()
                },
                {
                    userId: sessionDb.users.findOne({ name: "Charlie Brown" })._id,
                    products: [
                        {
                            productId: sessionDb.products.findOne({ name: "Yoga Mat" })._id,
                            quantity: 50,
                            priceAtPurchase: sessionDb.products.findOne({ name: "Yoga Mat" }).price
                        }
                    ],
                    status: "pending",
                    createdAt: new Date()
                },
            ]
        );
    }

    // Update totalAmount for orders if not already set
    if (db.orders.countDocuments({ totalAmount: { $gt: 0 } }) === 0) {
        // sessionDb.orders.find().forEach(function (order) {
        //     var totalAmount = 0;

        //     // Calculate total amount by summing the prices of the products in the order
        //     order.products.forEach(function (product) {
        //         if (product) totalAmount += product.priceAtPurchase * product.quantity;
        //     });

        //     // Update the order document with the calculated total amount
        //     sessionDb.orders.updateOne(
        //         { _id: order._id },
        //         { $set: { totalAmount: totalAmount } }
        //     );
        // });

        sessionDb.orders.aggregate([
            {
                $set: {
                    totalAmount: {
                        $sum: {
                            $map: {
                                input: "$products",
                                as: "p",
                                in: { $multiply: ["$$p.priceAtPurchase", "$$p.quantity"] }
                            }
                        }
                    }
                }
            }
        ]);
    }

    // Update order status to completed for Charlie Brown's order
    sessionDb.orders.updateOne(
        {
            userId: sessionDb.users.findOne({ name: "Charlie Brown" })._id
        },
        {
            $set: { status: "completed" }
        }
    );

    // Update the stock of the products based on the orders that have been completed
    sessionDb.orders.find({ status: "completed" })
        .forEach(function (order) {
            order.products.forEach(function (product) {
                sessionDb.products.updateOne(
                    {
                        _id: product.productId,
                        stock: { $gte: product.quantity } // Ensure there is enough stock before decrementing
                    },
                    {
                        $inc: { stock: -product.quantity }
                    }
                );
            });
        });

    // Add a new tag "bestseller" to the Yoga Mat product if it doesn't already have it
    sessionDb.products.updateOne(
        {
            name: "Yoga Mat"
        },
        {
            $addToSet: { tags: "bestseller" }
        }
    );


    if (sessionDb.reviews.countDocuments() === 0) {

        // Insert review documents
        sessionDb.reviews.insertMany(
            [
                {
                    userId: sessionDb.users.findOne({ name: "Charlie Brown" })._id,
                    productId: sessionDb.products.findOne({ name: "Yoga Mat" })._id,
                    rating: 5,
                    comment: "Great yoga mats! Best quality and very comfortable. " +
                        "Bought 50 for my yoga studio and they are perfect for all levels of practice.",
                    createdAt: new Date()
                },
            ]
        );
    }

    session.commitTransaction();
}
catch (error) {
    print("Error during transaction: " + error);
    session.abortTransaction();
}
finally {
    session.endSession();
}

if (test1) {

    // Find users with age greater than 25 and print their name and email
    print("\nUsers with age greater than 25:");
    db.users.find({
        age: { $gt: 25 }
    }).forEach(function (user) {
        print("User: " + user.name + ", Email: " + user.email);
    })

    // Find products with price less than 50 and print their name and price
    print("\nProducts with price less than $50:");
    db.products.find({
        price: { $lt: 50 }
    }).forEach(function (product) {
        print("Product: " + product.name + ", Price: $" + product.price);
    })

    // Find orders with status "completed" and print the user's name and total amount
    print("\nCompleted orders with user name and total amount:");
    db.orders.find({
        status: "completed"
    }).forEach(function (order) {
        var user = db.users.findOne({ _id: order.userId });
        print("Order for user: " + user.name + ", Total Amount: $" + order.totalAmount);
    });

    // Lookup to join orders with users
    print("\nOrders with user details:");
    var completedOrders = db.orders.aggregate(
        [
            {
                $match: {
                    status: "completed"
                }
            },
            {
                $lookup: {
                    from: "users",
                    localField: "userId",
                    foreignField: "_id",
                    as: "user"
                }
            },
            {
                $lookup: {
                    from: "products",
                    localField: "products.productId",
                    foreignField: "_id",
                    as: "products"
                }
            }
        ]
    ).toArray();

    print(completedOrders);

    // Lookup to join orders with users and products for pending orders
    print("\nPending orders with user and product details:");
    var pendingOrders = db.orders.aggregate(
        [
            {
                $match: {
                    status: "pending"
                }
            },
            {
                $lookup: {
                    from: "users",
                    localField: "userId",
                    foreignField: "_id",
                    as: "user"
                },
            },
            {
                $lookup: {
                    from: "products",
                    let: { orderProducts: "$products" },
                    pipeline: [
                        {
                            $match: {
                                $expr: {
                                    $in: ["$_id", "$$orderProducts.productId"]
                                }
                            }
                        },
                    ],
                    as: "productDetails"
                }
            }
        ]
    ).toArray();

    print(pendingOrders);

    // Get current product stock and total quantity ordered (all time)
    db.orders.aggregate(
        [
            {
                // Flatten the products array (one document per product in order)
                $unwind: "$products"
            },
            {
                // Group by productId and sum total ordered quantity
                $group: {
                    _id: "$products.productId",
                    totalOrdered: { $sum: "$products.quantity" }
                }
            },
            {
                // Join with products collection to get product details
                $lookup: {
                    from: "products",
                    localField: "_id",
                    foreignField: "_id",
                    as: "productDetails"
                }
            },
            {
                // Convert productDetails array to object
                $unwind: "$productDetails"
            },
            {
                // Shape final output
                $project: {
                    _id: 0,
                    productId: "$_id",
                    productName: "$productDetails.name",
                    currentStock: "$productDetails.stock",
                    totalOrdered: 1 // include totalOrdered in output
                }
            }
        ]
    );

    pendingOrders
}

if (test2) {
    const session2 = db.getMongo().startSession();
    const sessionDb2 = session2.getDatabase("shopDb");
    session2.startTransaction();

    const alice = db.users.findOne({ name: "Alice Smith" });

    try {
        // Checkout process for Alice Smith's order
        const items = [
            { name: "Wireless Mouse", quantity: 2 },
            { name: "Yoga Mat", quantity: 1 }
        ];

        const products = items.map(i => {
            const p = db.products.findOne({ name: i.name });
            if (!p) throw new Error("Product not found: " + i.name);
            if (p.stock < i.quantity) throw new Error("Not enough stock for product: " + i.name);

            return {
                productId: p._id,
                quantity: i.quantity,
                priceAtPurchase: p.price
            };
        });

        // totalAmount = sum[i] (products[i].priceAtPurchase * products[i].quantity)
        const totalAmount = products.reduce((sum, p) => sum + (p.priceAtPurchase * p.quantity), 0);

        // Insert the order document
        const order = {
            userId: alice._id,
            products: products,
            totalAmount: totalAmount,
            status: "pending",
            createdAt: new Date()
        };

        db.orders.insertOne(order);

        // Decrement the stock of the products
        products.forEach(p => {
            const res = db.products.updateOne(
                {
                    _id: p.productId, stock: { $gte: p.quantity } // Ensure there is enough stock before decrementing
                },
                {
                    $inc: { stock: -p.quantity }
                }
            );

            if (res.matchedCount === 0) throw new Error("Failed to update stock for productId: " + p.productId);
        });

        session2.commitTransaction();
        session2.endSession();
    } catch (error) {
        print("Error during checkout: " + error.message);
        session2.abortTransaction();
        session2.endSession();
    }

    // Verify the order was created and stock was updated
    const aliceLastOrder = db.orders
        .aggregate([
            { $match: { userId: alice._id } },
            { $sort: { createdAt: -1 } },
            { $limit: 1 },

            // join user
            {
                $lookup: {
                    from: "users",
                    localField: "userId", // orders.userId
                    foreignField: "_id",
                    as: "userProfile"
                }
            },

            // unwrap user
            // because lookup always returns an array, so extract the first element
            {
                $addFields: {
                    userProfile: { $arrayElemAt: ["$userProfile", 0] }
                }
            },

            // unwind products 
            // because products is an array 
            // need to get one document per product
            { $unwind: "$products" },

            // join each product
            {
                $lookup: {
                    from: "products",
                    localField: "products.productId", // orders.products.productId
                    foreignField: "_id",
                    as: "productInfo"
                }
            },

            // unwrap product
            // because lookup always returns an array, so extract the first element
            {
                $addFields: {
                    productInfo: { $arrayElemAt: ["$productInfo", 0] }
                }
            },

            // final shape
            {
                $project: {
                    _id: 0,

                    // order
                    totalAmount: 1,
                    status: 1,
                    createdAt: 1,

                    // user
                    userName: "$userProfile.name",
                    userEmail: "$userProfile.email",

                    // product
                    productName: "$productInfo.name",
                    quantity: "$products.quantity",
                    priceAtPurchase: "$products.priceAtPurchase"
                }
            }
        ])
        .toArray();

    print("\nAlice's Last Order:");
    print(aliceLastOrder);
    aliceLastOrder

    // Charlie Brown tries to order 15 Bluetooth Headphones
    const bluetoothHeadphones = db.products.findOne({ name: "Bluetooth Headphones" });
    const charlie = db.users.findOne({ name: "Charlie Brown" });

    db.orders.insertMany(
        [
            {
                userId: charlie._id,
                products: [
                    {
                        productId: bluetoothHeadphones._id,
                        quantity: 15,
                        priceAtPurchase: bluetoothHeadphones.price
                    }
                ],
                status: "pending",
                createdAt: new Date()
            }
        ]
    )

    // Update the order status to completed and calculate totalAmount for Charlie Brown's order
    db.orders.updateOne(
        {
            userId: charlie._id,
            "products.productId": bluetoothHeadphones._id
        },
        [
            {
                $set: {
                    status: "completed",
                    totalAmount: {
                        $toDouble: {
                            $sum: {
                                $map: {
                                    input: "$products",
                                    as: "p",
                                    in: {
                                        $multiply: [
                                            "$$p.priceAtPurchase",
                                            "$$p.quantity"
                                        ]
                                    }
                                }
                            }
                        }
                    }
                }
            }
        ]
    );

    var charlieLastOrder = db.orders.aggregate(
        { $match: { userId: charlie._id, "products.productId": bluetoothHeadphones._id } },
        {
            $lookup: {
                from: "users",
                localField: "userId",
                foreignField: "_id",
                as: "userProfile"
            }
        },
        {
            $addFields: {
                userProfile: { $arrayElemAt: ["$userProfile", 0] }
            }
        },
        { $unwind: "$products" },
        {
            $lookup: {
                from: "products",
                localField: "products.productId",
                foreignField: "_id",
                as: "productInfo"
            }
        },
        {
            $addFields: {
                productInfo: { $arrayElemAt: ["$productInfo", 0] }
            }
        },
        {
            $project: {
                _id: 1,
                totalAmount: 1,
                status: 1,
                userName: "$userProfile.name",
                productName: "$productInfo.name",
                quantity: "$products.quantity",
                priceAtPurchase: "$products.priceAtPurchase"
            }
        }
    ).toArray();

    print("\nCharlie's Last Order:");
    print(charlieLastOrder);
    charlieLastOrder
}