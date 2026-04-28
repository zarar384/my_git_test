use('companies-task');

db.companies.drop();

db.companies.insertOne(
    {
        _id: 1,
        name: 'Google',
        country: 'USA',
        employees: 100000,
    }
)

db.companies.insertMany(
    [
        {
            _id: 2,
            name: 'Microsoft',
            country: 'USA',
            employees: 150000,
        },
        {
            _id: 3,
            name: 'Apple',
            country: 'USA',
            employees: 120000,
        }
    ]
);

// db.companies.insertMany(
//     [
//         { // Duplicate _id value, will cause an error
//             _id: 3,
//             name: 'AppleDuplicate',
//             country: 'USA',
//             employees: 120000, 
//         },
//         {
//             _id: 4,
//             name: 'Amazon',
//             country: 'USA',
//             employees: 130000,
//         }
//     ]
// )

try{
// fix using unordered insert
db.companies.insertMany(
    [
        { // Duplicate _id value, will cause an error
            _id: 3,
            name: 'AppleDuplicate',
            country: 'USA',
            employees: 120000,
        },
        {
            _id: 4,
            name: 'Amazon',
            country: 'USA',
            employees: 130000,
        }
    ],
    { ordered: false } // Continue inserting even if there is an error
)
}
catch(e){
    // just log and continue
    print('Error inserting documents:', e.message);
}

// write with journaling guaranteed
db.companies.insertOne(
    {
        _id: 5,
        name: 'Facebook',
        country: 'USA',
        employees: 110000,
    },
    // Ensure the write is acknowledged by the majority of replica set members and journaled
    { writeConcern: { w: 'majority' , j: true } } 
)

// write with journaling not guaranteed
db.companies.insertOne(
    {
        _id: 6,
        name: 'Twitter',
        country: 'USA',
        employees: 5000,
    },
    // Ensure the write is acknowledged by the majority of replica set members but not journaled
    { writeConcern: { w: 'majority' , j: false } } 
)

db.companies.find().pretty();