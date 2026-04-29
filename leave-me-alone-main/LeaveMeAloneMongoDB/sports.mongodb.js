use("sports")
db.sports.drop();

// import two documents using upsert
db.sports.updateOne(
  { title: "Lacrosse" },          // filter
  { $set: { requiresTeam: true } },
  { upsert: true }
);

db.sports.updateOne(
  { title: "Tennis" },
  { $set: { requiresTeam: false } },
  { upsert: true }
);

print("Update all sports which do require a team")
db.sports.updateMany(
    { requiresTeam: true },
    {
        $set: { minimumAmountOfPlayers: 0 },
    }
);

print("Update all sports that require a team by increasing the minimum amount of players by 1")
db.sports.updateMany(
    { requiresTeam: true },
    {
        $inc: { minimumAmountOfPlayers: 10 },
    }
);

db.sports.find().pretty();