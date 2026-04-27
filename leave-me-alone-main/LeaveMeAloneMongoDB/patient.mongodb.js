use("patientDb");
let drop = true;
if (drop) db.patients.drop();

// create patient collection and insert data
if (db.collections.find({ name: "patients" }).count() === 0) {
    db.createCollection("patients",
        {
            validator: {
                $jsonSchema: {
                    bsonType: "object",
                    required: ["firstName", "lastName", "age", "history"],
                    properties: {
                        firstName: {
                            bsonType: "string",
                            description: "First name must be a string and is required"
                        },
                        lastName: {
                            bsonType: "string",
                            description: "Last name must be a string and is required"
                        },
                        age: {
                            bsonType: "int",
                            description: "Age must be an integer and is required"
                        },
                        history: {
                            bsonType: "array",
                            description: "History must be an array and is required"
                        }
                    },
                    description: "Patient document must have firstName, lastName, age, and history fields"
                }
            }
        }
    );
}

if (db.patients.countDocuments() === 0) {
    db.patients.insertMany(
        [
            {
                firstName: "Alice",
                lastName: "Johnson",
                age: 30,
                history: [
                    {
                        date: new Date("2023-01-15"),
                        diagnosis: "Flu",
                        treatment: "Rest and hydration"
                    }
                ]
            },
            {
                firstName: "Bob",
                lastName: "Smith",
                age: 45,
                history: [
                    {
                        date: new Date("2022-12-10"),
                        diagnosis: "Hypertension",
                        treatment: "Medication and lifestyle changes"
                    },
                    {
                        date: new Date("2023-02-20"),
                        diagnosis: "Diabetes",
                        treatment: "Medication and diet control"
                    },
                    {
                        date: new Date("2023-03-05"),
                        diagnosis: "High Cholesterol",
                        treatment: "Medication and diet control"
                    }
                ]
            },
            {
                firstName: "Charlie",
                lastName: "Brown",
                age: 25,
                history: [
                    {
                        date: new Date("2023-03-01"),
                        diagnosis: "Allergy",
                        treatment: "Antihistamines"
                    }
                ]
            }
        ]);
}


// update one patient document
db.patients.updateOne(
    { firstName: "Alice", lastName: "Johnson" },
    {
        $set:
        {
            age: 31,
            firstName: "Alice",
            history: [
                {
                    date: new Date("2023-01-15"),
                    diagnosis: "Flu",
                    treatment: "Rest and hydration"
                }
            ]
        }
    }
);


// find all patients with age greater than 30
db.patients.find({ age: { $gt: 30 } }).forEach(patient => {
    print(`Patient: ${patient.firstName} ${patient.lastName}, Age: ${patient.age}`);
});

// delete all
db.patients.deleteMany({});