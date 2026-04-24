use('carsPeople');

// insert people data
if (db.people.countDocuments() === 0) {
    db.people.insertMany(
        [
            {
                name: 'John Doe',
                age: 30,
                address: {
                    city: 'New York',
                    street: '5th Avenue',
                },
                hobbies: ['reading', 'traveling'],
                createdAt: new Date(),
            },
            {
                name: "Susan Smith",
                age: 25,
                address: {
                    city: "Los Angeles",
                    street: "Sunset Boulevard",
                },
                hobbies: ["cooking", "yoga"],
                createdAt: new Date(),
            }
        ]
    );
}

// insert cars data
if (db.cars.countDocuments() === 0) {
    db.cars.insertMany(
        [
            {
                brand: 'Toyota',
                model: 'Camry',
                year: 2020,
                ownerId: db.people.findOne({ name: 'John Doe' })._id,
                tags: ['sedan', 'family'],
                specs: {
                    horsepower: 203,
                    fuelType: 'gasoline',
                }
            },
            {
                brand: 'Honda',
                model: 'Civic',
                year: 2019,
                ownerId: db.people.findOne({ name: 'Susan Smith' })._id,
                tags: ['compact', 'economy'],
                specs: {
                    horsepower: 158,
                    fuelType: 'gasoline',
                }
            }
        ]
    );
}

if (db.drives.countDocuments() === 0) {
    // create drives (history of car ownership)
    db.drives.insertMany(
        [
            {
                personId: db.people.findOne({ name: 'John Doe' })._id,
                carId: db.cars.findOne({ model: 'Camry' })._id,
                distance: 15000,
                date: new Date('2021-01-01'),
            },
            {
                personId: db.people.findOne({ name: 'Susan Smith' })._id,
                carId: db.cars.findOne({ model: 'Civic' })._id,
                distance: 12000,
                date: new Date('2021-06-01'),
            }
        ]
    );
}

// find a peopl with age greater than 28
console.log("People with age greater than 28:");
console.log(db.people.find({ age: { $gt: 28 } }).toArray());

// update a person John Doe's age to 31 and push a new hobby 'gaming'
if (db.people.findOne({ name: 'John Doe' })
    && !db.people.findOne({ name: 'John Doe' }).hobbies.includes('gaming')) {

    db.people.updateOne(
        { name: 'John Doe' },
        {
            $set: { age: 31 },
            $push: { hobbies: 'gaming' }
        }
    );
}

// create an index on the 'name' field of the people collection
if (!db.people.getIndexes().some(index => index.key.name === 1)) {
    db.people.createIndex({ name: 1 });
}

// aggregate to calculate total distance driven by each person
console.log("Total distance driven by each person:");
console.log(db.drives.aggregate([
    {
        $group: {
            _id: "$personId",
            totalDistance: { $sum: "$distance" }
        }
    }
]).toArray());

// lookup to join people with their cars
console.log("People with their cars:");
console.log(db.people.aggregate(
    [
        {
            $lookup: {
                from: "cars",
                localField: "_id",
                foreignField: "ownerId",
                as: "cars"
            }
        }
    ]
).toArray());

// find cars with horsepower greater than 100 and tagged as 'sedan'
console.log("Cars with horsepower greater than 100 and tagged as 'sedan':");
var result = db.cars.find({
    "specs.horsepower": { $gt: 100 },
    tags: { $in: ["sedan"] }
}).toArray();

console.log(result);
result; // to display the result in the shell
