use("boxOffice");

// import movies data
// mongoimport data/boxoffice.json -d boxOffice -c movies --jsonArray --drop

print("Movies with rating greater than 9 and runtime less than 100 minutes:");
db.movies.find({
    rating: { $gt: 9 },
    runtime: { $lt: 100 }
}).pretty();

print("All movies with the genre 'action' and 'drama':");
db.movies.find({
    genre: { $in: ["action", "drama"] }
}).pretty();

print("All movies where visitors exceeded expectedVisitors:");
db.movies.find({
    $expr: { $gt: ["$visitors", "$expectedVisitors"] }
}).pretty()

print("All movies with excatly 2 genres:");
db.movies.find({
    genre: { $size: 2 }
}).pretty();

print("All movies wich aired in 2018");
db.movies.find({
    "meta.aired": 2018
}).pretty();

print("All movies wich have ratings greater than 8 but lower than 10:");
db.movies.find({
    ratings: { $elemMatch: { $gt: 8, $lt: 10 } }
}).pretty();