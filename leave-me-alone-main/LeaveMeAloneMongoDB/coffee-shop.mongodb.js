use("coffee-shop");

const dropDatabase = true;
if (dropDatabase)
    db.dropDatabase();

// USERS
if (!db.getCollectionNames().includes("users")) {
    db.createCollection("users");
}


// USER PROFILES (one-to-one)
if (!db.getCollectionNames().includes("userProfiles")) {
    db.createCollection("userProfiles", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["userId", "favoriteDrink", "loyaltyPoints", "membershipLevel"],
                properties: {
                    userId: {
                        bsonType: "objectId",
                        description: "must be an ObjectId and is required"
                    },
                    favoriteDrink: {
                        bsonType: "string",
                        description: "must be a string and is required"
                    },
                    loyaltyPoints: {
                        bsonType: "int",
                        minimum: 0,
                        description: "must be a non-negative integer and is required"
                    },
                    membershipLevel: {
                        bsonType: "string",
                        enum: ["Bronze", "Silver", "Gold", "Platinum"],
                        description: "must be one of the predefined membership levels and is required"
                    }
                }
            }
        }
    });
}


// PRODUCTS
if (!db.getCollectionNames().includes("products")) {
    db.createCollection("products", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["name", "price", "category"],
                properties: {
                    name: {
                        bsonType: "string",
                        maxLength: 100,
                        description: "must be a string and is required"
                    },
                    price: {
                        bsonType: "decimal",
                        minimum: 0,
                        description: "must be a non-negative decimal and is required"
                    },
                    category: {
                        bsonType: "string",
                        enum: ["Coffee", "Tea", "Pastry", "Other"],
                        description: "must be one of the predefined categories and is required"
                    }
                }
            }
        }
    });
}


// ORDERS 
if (!db.getCollectionNames().includes("orders")) {
    db.createCollection("orders", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["userId", "products", "paymentMethod", "status"],
                properties: {
                    userId: {
                        bsonType: "objectId",
                        description: "must be an ObjectId and is required"
                    },

                    products: {
                        bsonType: "array",
                        minItems: 1,
                        items: {
                            bsonType: "object",
                            required: ["productId", "quantity", "price"],
                            properties: {
                                productId: {
                                    bsonType: "objectId",
                                    description: "must be an ObjectId and is required"
                                },
                                quantity: {
                                    bsonType: "int",
                                    minimum: 1,
                                    description: "must be a positive integer and is required"
                                },
                                price: {
                                    bsonType: "decimal",
                                    minimum: 0,
                                    description: "must be a non-negative decimal and is required"
                                }
                            }
                        }
                    },

                    paymentMethod: {
                        bsonType: "string",
                        enum: ["Cash", "Credit Card", "Debit Card", "Mobile Payment"],
                        description: "must be one of the predefined payment methods and is required"
                    },

                    status: {
                        bsonType: "string",
                        enum: ["pending", "completed", "cancelled"],
                        description: "must be one of the predefined order statuses and is required"
                    },

                    totalAmount: {
                        bsonType: "decimal",
                        minimum: 0,
                        description: "must be a non-negative decimal and is required"
                    },

                    createdAt: {
                        bsonType: "date",
                        description: "must be a date and is required"
                    }
                }
            }
        }
    });
}


// INGREDIENTS
if (!db.getCollectionNames().includes("ingredients")) {
    db.createCollection("ingredients", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["name", "stock", "unit"],
                properties: {
                    name: {
                        bsonType: "string",
                        maxLength: 100,
                        description: "must be a string and is required"
                    },
                    stock: {
                        bsonType: "decimal",
                        minimum: 0,
                        description: "must be a non-negative decimal and is required"
                    },
                    unit: {
                        bsonType: "string",
                        enum: ["shots", "ml", "grams"],
                        description: "must be one of the predefined units and is required"
                    }
                }
            }
        }
    });
}


// PRODUCT INGREDIENTS (many-to-many)
if (!db.getCollectionNames().includes("productIngredients")) {
    db.createCollection("productIngredients", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["productId", "ingredientId", "amount"],
                properties: {
                    productId: {
                        bsonType: "objectId",
                        description: "must be an ObjectId and is required"
                    },
                    ingredientId: {
                        bsonType: "objectId",
                        description: "must be an ObjectId and is required"
                    },
                    amount: {
                        bsonType: "decimal",
                        minimum: 0,
                        description: "must be a non-negative decimal and is required"
                    }
                }
            }
        }
    });
}


// FAVORITES (Many-to-Many)
if (!db.getCollectionNames().includes("favorites")) {
    db.createCollection("favorites", {
        validator: {
            $jsonSchema: {
                bsonType: "object",
                required: ["userId", "productId", "createdAt"],
                properties: {
                    userId: {
                        bsonType: "objectId",
                        description: "must be an ObjectId and is required"
                    },
                    productId: {
                        bsonType: "objectId",
                        description: "must be an ObjectId and is required"
                    },
                    createdAt: {
                        bsonType: "date",
                        description: "must be a date and is required"
                    }
                }
            }
        }
    });
}


const i3 = new ObjectId();

try {
    const u1 = new ObjectId();
    const u2 = new ObjectId();

    // SEED DATA
    // USERS
    db.users.insertMany([
        { _id: u1 }, { _id: u2 }
    ]);

    // USER PROFILES (One-to-One)
    db.userProfiles.insertMany([
        { userId: u1, favoriteDrink: "Latte", loyaltyPoints: NumberInt(150), membershipLevel: "Gold" },
        { userId: u2, favoriteDrink: "Espresso", loyaltyPoints: NumberInt(50), membershipLevel: "Silver" }
    ]);

    const p1 = new ObjectId();
    const p2 = new ObjectId();
    const p3 = new ObjectId();

    // PRODUCTS
    db.products.insertMany([
        { _id: p1, name: "Latte", price: NumberDecimal("4.50"), category: "Coffee" },
        { _id: p2, name: "Espresso", price: NumberDecimal("3.00"), category: "Coffee" },
        { _id: p3, name: "Cappuccino", price: NumberDecimal("4.00"), category: "Coffee" }
    ]);

    const i1 = new ObjectId();
    const i2 = new ObjectId();

    // INGREDIENTS 
    db.ingredients.insertMany([
        { _id: i1, name: "Espresso Shot", stock: NumberDecimal("100.00"), unit: "shots" },
        { _id: i2, name: "Steamed Milk", stock: NumberDecimal("200.00"), unit: "ml" },
        { _id: i3, name: "Foamed Milk", stock: NumberDecimal("150.00"), unit: "ml" }
    ]);

    // PRODUCT INGREDIENTS (Many-to-Many)
    db.productIngredients.insertMany([
        { productId: p1, ingredientId: i1, amount: NumberDecimal("1.00") }, // Latte has 1 espresso shot
        { productId: p1, ingredientId: i2, amount: NumberDecimal("150.00") }, // Latte has 150ml steamed milk
        { productId: p1, ingredientId: i3, amount: NumberDecimal("50.00") }, // Latte has 50ml foamed milk
        { productId: p2, ingredientId: i1, amount: NumberDecimal("1.00") }, // Espresso has 1 espresso shot
        { productId: p3, ingredientId: i1, amount: NumberDecimal("1.00") }, // Cappuccino has 1 espresso shot
        { productId: p3, ingredientId: i2, amount: NumberDecimal("100.00") }, // Cappuccino has 100ml steamed milk
        { productId: p3, ingredientId: i3, amount: NumberDecimal("100.00") } // Cappuccino has 100ml foamed milk
    ]);

    // ORDERS (One-to-Many)
    db.orders.insertMany([
        { userId: u1, products: [{ productId: p1, quantity: NumberInt(2), price: NumberDecimal("4.50") }, { productId: p3, quantity: NumberInt(1), price: NumberDecimal("4.00") }], paymentMethod: "Credit Card", status: "completed", totalAmount: NumberDecimal("12.50"), createdAt: new Date() },
        { userId: u2, products: [{ productId: p2, quantity: NumberInt(1), price: NumberDecimal("3.00") }], paymentMethod: "Cash", status: "pending", totalAmount: NumberDecimal("3.00"), createdAt: new Date() }
    ]);

    // FAVORITES (Many-to-Many)
    db.favorites.insertMany([
        { userId: u1, productId: p1, createdAt: new Date() },
        { userId: u1, productId: p3, createdAt: new Date() },
        { userId: u2, productId: p2, createdAt: new Date() }
    ]);
}
catch (e) {
    print("Error:");

    if (e.writeErrors && e.writeErrors.length > 0) {
        printjson(e.writeErrors?.[0]?.errInfo);
    } else {
        printjson(e);
    }
}
// One-to-One (User and UserProfile) 
print("One-to-One (User and UserProfile):");
var userWithProfile = db.users.aggregate([
    {
        $lookup: {
            from: "userProfiles",
            localField: "_id", // from users
            foreignField: "userId", // from userProfiles
            as: "profile"
        }
    }
]).toArray();

printjson(userWithProfile);

// One-to-Many (User and Orders)
print("One-to-Many (User and Orders):");
var userWithOrders = db.users.aggregate([
    {
        $lookup: {
            from: "orders",
            localField: "_id", // from users
            foreignField: "userId", // from orders
            as: "orders"
        }
    }
]).toArray();

printjson(userWithOrders);

// Many-to-Many (Products and Ingredients)
print("Many-to-Many (Products and Ingredients):");
var productsWithIngredients = db.products.aggregate([
    {
        $lookup: {
            from: "productIngredients",
            let: { pId: "$_id", }, // from products
            pipeline: [
                {
                    $match: {
                        $expr: {
                            $eq: ["$productId", "$$pId"] // from productIngredients join with products
                        }
                    }
                },
                {
                    $project: {
                        amount: 1,
                        ingredientId: 1, // for next lookup
                        _id: 0 // ignore
                    }
                }
            ],
            as: "ingredients"
        }
    },
    {
        $lookup: {
            from: "ingredients",
            let: { iIds: "$ingredients.ingredientId" }, // from previous lookup
            pipeline: [
                {
                    $match: {
                        $expr: {
                            $in: ["$_id", "$$iIds"]
                        }
                    }
                },
                {
                    $project: {
                        name: 1,
                        stock: 1,
                        unit: 1,
                        _id: 0 // ignore
                    }
                }
            ],
            as: "ingredientDetails"
        }
    }
]).toArray();

printjson(productsWithIngredients);

// all products with Foamed Milk
print("All products with Foamed Milk:");
var productsWithFoamedMilk = db.products.aggregate([
    {
        $lookup: {
            from: "productIngredients",
            let: { pId: "$_id" },
            pipeline: [
                {
                    $match: {
                        $expr: { // expression to match both productId and ingredientId for foamed milk
                            $and: [
                                { $eq: ["$productId", "$$pId"] },
                                { $eq: ["$ingredientId", i3] } // i3 is Foamed Milk
                            ]
                        }
                    }
                },
                {
                    $project: {
                        _id: 0, // ignore
                        amount: 1
                    }
                }
            ],
            as: "foamedMilkInfo"
        }
    },
    {
        $match: {
            "foamedMilkInfo": { $ne: [] } // filter products that have foamed milk  
        }
    }
]).toArray();

printjson(productsWithFoamedMilk);

// Many-to-Many (Users and Products through Favorites)
print("Many-to-Many (Users and Products through Favorites):");
var usersWithFavoriteProducts = db.users.aggregate(
    [
        {
            $lookup: {
                from: "favorites",
                localField: "_id", // from users
                foreignField: "userId", // from favorites
                as: "favorites"
            },
        },
        {
            $lookup: {
                from: "products",
                let: { favoriteProductIds: "$favorites.productId" }, // from previous lookup
                pipeline: [
                    {
                        $match: {
                            $expr: {
                                $in: ["$_id", "$$favoriteProductIds"]
                            }
                        }
                    },
                    {
                        $project: {
                            name: 1,
                            price: 1,
                            category: 1,
                            _id: 0 // ignore
                        }
                    }
                ],
                as: "favoriteProducts"
            }
        },
        {
            $project: {
                name: 1,
                favorites: 1,
                favoriteProducts: 1,
                _id: 0, // ignore
            }
        },
    ]
).toArray();

printjson(usersWithFavoriteProducts);

// calculate total amount spent by each user on completed orders
print("Total amount spent by each user on completed orders:");
var totalSpentByUser = db.users.aggregate(
    [
        {
            $lookup: {
                from: "orders",
                localField: "_id",
                foreignField: "userId",
                as: "orders"
            }
        },
        {
            $unwind: "$orders" // unwind orders to filter by status and sum totalAmount
        },
        {
            $match: {
                "orders.status": "completed"
            }
        },
        {
            $group: {
                _id: "$_id",
                totalSpent: { $sum: "$orders.totalAmount" }
            }
        },
        {
            $project: {
                _id: 0,
                userId: "$_id",
                totalSpent: 1
            }
        }
    ]
).toArray();

printjson(totalSpentByUser);

// update stock of ingredients used in a completed order (e.g., for user u1's completed order)
var productIngredientsResult = [];

print("Updating stock of ingredients used in completed orders:");
var completedOrder = db.orders.find({ status: "completed" })
    .forEach(order => {
        order.products.forEach(product => {
            var productIngredients = db.productIngredients.find({ productId: product.productId }).toArray();
            productIngredients.forEach(pi => {
                var amountUsed = NumberDecimal((pi.amount.toString() * product.quantity).toString());
                var currentStock = db.ingredients.findOne({ _id: pi.ingredientId }).stock;
                var success = amountUsed <= currentStock ? "Success" : "Failed - Insufficient Stock";
                var obj = { productId: product.productId, ingredientId: pi.ingredientId, amountUsed: amountUsed, currentStock: `${currentStock}-${amountUsed}=${currentStock - amountUsed}`, status: success };
               
                productIngredientsResult.push(obj);
                printjson(obj);

                db.ingredients.updateOne(
                    {
                        _id: pi.ingredientId,
                        stock: { $gte: amountUsed }
                    },
                    {
                        $inc: { stock: -amountUsed }
                    }
                );
            });
        });
    });

productIngredientsResult;