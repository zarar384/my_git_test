use('places')
db.places.drop();

// coordinates: [lng, lat]
// polygon: array of points,  first = last
// index: "2dsphere" REQUIRED for geo queries

// constants for coordinates
const lng1 = 14.42076;
const lat1 = 50.08804;

const lng2 = 14.42100;
const lat2 = 50.08750;

const lng3 = 14.41950;
const lat3 = 50.08900;

// user location
const userLng = 14.42050;
const userLat = 50.08850;

// distance in meters
const minDistance = 10;
const maxDistance = 500;

// points for polygon
const P1 = [14.418, 50.087];
const P2 = [14.422, 50.087];
const P3 = [14.422, 50.090];
const P4 = [14.418, 50.090];

print("Pick 3 points and store")

db.places.insertMany([
{
  name: "Place 1",
  location: {
    type: "Point",
    coordinates: [lng1, lat1]
  }
},
{
  name: "Place 2",
  location: {
    type: "Point",
    coordinates: [lng2, lat2]
  }
},
{
  name: "Place 3",
  location: {
    type: "Point",
    coordinates: [lng3, lat3]
  }
}
])

// create index, 2dsphere index is required for geospatial queries on GeoJSON data
db.places.createIndex({ location: "2dsphere" })


print("Find nearest within min/max distance")

db.places.find({
    location: {
        $near: {
            $geometry: {
                type: "Point",
                coordinates: [userLng, userLat]
            },
            $minDistance: minDistance,
            $maxDistance: maxDistance
        }
    }
}).pretty();

print("Find point inside area (polygon)");

db.places.find({
    location: {
        $geoWithin: { // find points within a polygon
            $geometry: { // define the polygon
                type: "Polygon",
                coordinates: [
                    [ 
                        P1, P2, P3, P4, P1 // close the loop by repeating the first point
                    ]
                ]
            }
        }
    }
}).pretty();

print("Store area");

db.areas.insertOne({
    name: "Area 1",
    area: {
        type: "Polygon",
        coordinates: [
            [
                P1, P2, P3, P4, P1 // close the loop by repeating the first point
            ]
        ]
    }
})

// index
db.areas.createIndex({ area: "2dsphere" })

print("Find which area contains point");

db.areas.find({
    area: {
        $geoIntersects: { // find areas that intersect with a point
            $geometry: {
                type: "Point",
                coordinates: [userLng, userLat]
            }
        }
    }
}).pretty();