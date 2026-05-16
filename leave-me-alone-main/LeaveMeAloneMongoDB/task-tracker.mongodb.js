use("taskTracker")
const drobDb = true;
if (drobDb) db.dropDatabase();

var session = db.getMongo().startSession();
var sessionDb = session.getDatabase("taskTracker");
session.startTransaction();

try {
    // users: 3 users
    sessionDb.users.insertMany([
        {
            name: "Sukashi",
            email: "sukashi@example.com",
            role: "developer",
            createdAt: new Date(),
        },
        {
            name: "Xuesoshi",
            email: "xuesoshi@example.com",
            role: "manager",
            createdAt: new Date(),
        },
        {
            name: "Axuevashi",
            email: "axuevashi@example.com",
            role: "intern",
            createdAt: new Date(),
        }
    ]);

    const sukashiId = sessionDb.users.findOne({ name: "Sukashi" })._id;
    const xuesoshiId = sessionDb.users.findOne({ name: "Xuesoshi" })._id;
    const axuevashiId = sessionDb.users.findOne({ name: "Axuevashi" })._id;

    // projects: 2 projects
    sessionDb.projects.insertMany([
        {
            name: "Learning Mongodb",
            description: "Practive project",
            members: [sukashiId, axuevashiId],
            createdAt: new Date(),
        },
        {
            name: "Simulate active work aktivity",
            description: "Do nothing useful",
            members: [xuesoshiId],
            createdAt: new Date(),
        }
    ]);

    const learningProjectId = sessionDb.projects.findOne({ name: "Learning Mongodb" })._id;
    const simulateProjectId = sessionDb.projects.findOne({ name: "Simulate active work aktivity" })._id;

    // tasks: 7 tasks
    sessionDb.tasks.insertMany([
        {
            title: "Learn MongoDB basics",
            description: "Understand MongoDB data model, CRUD operations, and basic queries.",

            projectId: learningProjectId,
            assigneeId: sukashiId,

            status: "todo",
            priority: "high",

            tags: ["mongodb", "learning"],

            estimatedHours: 5,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
        {
            title: "Learn MongoDB advanced",
            description: "Understand MongoDB aggregation, indexing, and performance optimization.",

            projectId: learningProjectId,
            assigneeId: axuevashiId,

            status: "todo",
            priority: "medium",

            tags: ["mongodb", "advanced"],

            estimatedHours: 8,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
        {
            title: "Simulate work activity",
            description: "Create a script to simulate active work activity for testing purposes.",

            projectId: simulateProjectId,
            assigneeId: xuesoshiId,

            status: "todo",
            priority: "low",

            tags: ["simulation", "testing"],

            estimatedHours: 3,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
        {
            title: "Set up MongoDB environment",
            description: "Install MongoDB and set up the development environment.",

            projectId: learningProjectId,
            assigneeId: sukashiId,

            status: "todo",
            priority: "high",

            tags: ["mongodb", "setup"],

            estimatedHours: 2,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
        {
            title: "Create sample data",
            description: "Insert sample data into MongoDB for testing and learning purposes.",

            projectId: learningProjectId,
            assigneeId: axuevashiId,

            status: "todo",
            priority: "medium",

            tags: ["mongodb", "data"],

            estimatedHours: 4,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
        {
            title: "Write MongoDB queries",
            description: "Practice writing various MongoDB queries to retrieve and manipulate data.",

            projectId: learningProjectId,
            assigneeId: sukashiId,

            status: "todo",
            priority: "high",

            tags: ["mongodb", "queries"],

            estimatedHours: 6,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
        {
            title: "Optimize MongoDB performance",
            description: "Learn about indexing and performance optimization techniques in MongoDB.",

            projectId: learningProjectId,
            assigneeId: axuevashiId,

            status: "todo",
            priority: "high",

            tags: ["mongodb", "performance"],

            estimatedHours: 5,

            history: [
                {
                    status: "todo",
                    newStatus: "in-progress",
                    changedAt: new Date(),
                }
            ],

            createdAt: new Date(),
        },
    ]);

    const learnMongoTaskId = sessionDb.tasks.findOne({ title: "Learn MongoDB basics" })._id;
    const optimizeMongoTaskId = sessionDb.tasks.findOne({ title: "Optimize MongoDB performance" })._id;
    const simulateWorkTaskId = sessionDb.tasks.findOne({ title: "Simulate work activity" })._id;
    const setupMongoTaskId = sessionDb.tasks.findOne({ title: "Set up MongoDB environment" })._id;
    const createDataTaskId = sessionDb.tasks.findOne({ title: "Create sample data" })._id;
    const writeQueriesTaskId = sessionDb.tasks.findOne({ title: "Write MongoDB queries" })._id;
    const learnAdvancedTaskId = sessionDb.tasks.findOne({ title: "Learn MongoDB advanced" })._id;

    // comments: 10 comments
    sessionDb.comments.insertMany([
        {
            taskId: learnMongoTaskId,
            userId: xuesoshiId,
            content: "This is a comment on the Learn MongoDB basics task.",
            createdAt: new Date(),
        },
        {
            taskId: optimizeMongoTaskId,
            userId: sukashiId,
            content: "This is a comment on the Optimize MongoDB performance task.",
            createdAt: new Date(),
        },
        {
            taskId: simulateWorkTaskId,
            userId: axuevashiId,
            content: "This is a comment on the Simulate work activity task.",
            createdAt: new Date(),
        },
        {
            taskId: setupMongoTaskId,
            userId: xuesoshiId,
            content: "This is a comment on the Set up MongoDB environment task.",
            createdAt: new Date(),
        },
        {
            taskId: createDataTaskId,
            userId: sukashiId,
            content: "This is a comment on the Create sample data task.",
            createdAt: new Date(),
        },
        {
            taskId: writeQueriesTaskId,
            userId: axuevashiId,
            content: "This is a comment on the Write MongoDB queries task.",
            createdAt: new Date(),
        },
        {
            taskId: learnAdvancedTaskId,
            userId: xuesoshiId,
            content: "This is a comment on the Learn MongoDB advanced task.",
            createdAt: new Date(),
        },
        {
            taskId: learnMongoTaskId,
            userId: sukashiId,
            content: "Another comment on the Learn MongoDB basics task.",
            createdAt: new Date(),
        },
        {
            taskId: optimizeMongoTaskId,
            userId: axuevashiId,
            content: "Another comment on the Optimize MongoDB performance task.",
            createdAt: new Date(),
        },
        {
            taskId: simulateWorkTaskId,
            userId: xuesoshiId,
            content: "Another comment on the Simulate work activity task.",
            createdAt: new Date(),
        }
    ]);

    session.commitTransaction();
}
catch (e) {
    session.abortTransaction();
    print(e);
}
finally {
    session.endSession();
}

// all tasks with the "todo" status
var todoTasks = db.tasks.find({ status: "todo" }).toArray();
print("Todo tasks:");
printjson(todoTasks);
todoTasks

// all high-priority tasks
var highPriorityTasks = db.tasks.find({ priority: "high" }).toArray();
print("High-priority tasks:");
printjson(highPriorityTasks);
highPriorityTasks

// all tasks with the "backend" tag
var backendTasks = db.tasks.find({ tags: "backend" }).toArray();
print("Tasks with 'backend' tag:");
printjson(backendTasks);
backendTasks

// all tasks assigned to Sukashi
var sukashiTasks = db.tasks.find(
    { assigneeId: db.users.findOne({ name: "Sukashi" })._id }
).toArray();
print("Tasks assigned to Sukashi:");
printjson(sukashiTasks);
sukashiTasks


// update the status of a task and add a comment to it
var taskIdToUpdate = db.tasks.findOne({ title: "Learn MongoDB basics" })._id;
db.tasks.updateOne(
    {
        _id: taskIdToUpdate
    },
    {
        $set: { status: "completed" },
        $push: {
            history: {
                status: "in-progress",
                newStatus: "completed",
                changedAt: new Date(),
            }
        }
    });

db.comments.insertOne({
    taskId: taskIdToUpdate,
    userId: db.users.findOne({ name: "Sukashi" })._id,
    content: "Started working on this task.",
    createdAt: new Date(),
});

var taskAfterUpdate = db.tasks.findOne({ _id: taskIdToUpdate });
print("Task after status update:");
printjson(taskAfterUpdate);
taskAfterUpdate

// create an index on the "status", assigneeId, tags fields to optimize queries
db.tasks.createIndex({ 
    status: 1,
    assigneeId: 1 
});

db.tasks.createIndex({ tags: 1 });

// lookup: task -> assigned user -> project -> all comments on the task
var taskWithDetails = db.tasks.aggregate([
    {
        $match: { title: "Learn MongoDB basics" }
    },
    {
        $lookup: {
            from: "users",
            localField: "assigneeId",
            foreignField: "_id",
            as: "assignee"
        }
    },
    {
        $addFields: {
            assignee: { $arrayElemAt: ["$assignee", 0] }
        }
    },
    {
        $lookup: {
            from: "projects",
            localField: "projectId",
            foreignField: "_id",
            as: "project"
        }
    },
    {
        $addFields: {
            project: { $arrayElemAt: ["$project", 0] }
        }
    },
    {
        $lookup: {
            from: "comments",
            localField: "_id",
            foreignField: "taskId",
            as: "comments"
        }
    },
    {
        $project: {
            title: 1,
            description: 1,
            status: 1,
            priority: 1,
            tags: 1,
            estimatedHours: 1,
            createdAt: 1,

            assignee: {
                name: "$assignee.name",
                email: "$assignee.email",
                role: "$assignee.role"
            },

            project: {
                name: "$project.name",
                description: "$project.description"
            },

            comments: {
                $map: {
                    input: "$comments",
                    as: "comment",
                    in: {
                        content: "$$comment.content",
                        createdAt: "$$comment.createdAt"
                    }
                }
            }
        }
    }
]).toArray();

print("Task with details:");
printjson(taskWithDetails);
taskWithDetails

// aggregation: count the number of tasks in each status
var taskCountByStatus = db.tasks.aggregate([
    {
        $group: {
            _id: "$status",
            count: { $sum: 1 }
        }
    }
]).toArray();

print("Task count by status:");
printjson(taskCountByStatus);
taskCountByStatus

// aggregation: calculate the average estimated hours for tasks in each project
var avgEstimatedHoursByProject = db.tasks.aggregate([
    {
        $group: {
            _id: "$projectId",
            avgEstimatedHours: { $avg: "$estimatedHours" }
        }
    },
    {
        $lookup: {
            from: "projects",
            localField: "_id",
            foreignField: "_id",
            as: "project"
        }
    },
    {
        $addFields: {
            project: { $arrayElemAt: ["$project", 0] }
        }
    },
    {
        $project: {
            avgEstimatedHours: 1,
            projectName: "$project.name"
        }
    }
]).toArray();
print("Average estimated hours by project:");
printjson(avgEstimatedHoursByProject);
avgEstimatedHoursByProject

// aggregation: how many tasks in each status for each user
var taskCountByStatusAndUser = db.tasks.aggregate([
    {
        $group: {
            _id: {
                assigneeId: "$assigneeId",
                status: "$status"
            },
            count: { $sum: 1 }
        }
    },
    {
        $lookup: {
            from: "users",
            localField: "_id.assigneeId",
            foreignField: "_id",
            as: "assignee"
        }
    },
    {   
        $addFields: {
            assignee: { $arrayElemAt: ["$assignee", 0] }
        }
    },
    {
        $project: {
            count: 1,
            status: "$_id.status",
            assigneeName: "$assignee.name"
        }
    }
]).toArray();
print("Task count by status and user:");
printjson(taskCountByStatusAndUser);
taskCountByStatusAndUser

// dashboard: $facet to get multiple aggregations in one query
var dashboardData = db.tasks.aggregate([
    {
        $facet: {
            countByStatus: [
                {
                    $group: {
                        _id: "$status",
                        count: { $sum: 1 }
                    }
                }
            ],
            avgEstimatedHoursByProject: [
                {
                    $group: {
                        _id: "$projectId",
                        avgEstimatedHours: { $avg: "$estimatedHours" }
                    }
                },
                {
                    $lookup: {
                        from: "projects",
                        localField: "_id",
                        foreignField: "_id",
                        as: "project"
                    }
                },  
                {
                    $addFields: {
                        project: { $arrayElemAt: ["$project", 0] }
                    }
                },
                {
                    $project: {
                        avgEstimatedHours: 1,
                        projectName: "$project.name"
                    }
                }
            ],
            countByStatusAndUser: [
                {
                    $group: {
                        _id: {
                            assigneeId: "$assigneeId",
                            status: "$status"
                        },
                        count: { $sum: 1 }
                    }
                },
                {
                    $lookup: {
                        from: "users",
                        localField: "_id.assigneeId",
                        foreignField: "_id",
                        as: "assignee"
                    }
                },
                {
                    $addFields: {
                        assignee: { $arrayElemAt: ["$assignee", 0] }
                    }
                },
                {
                    $project: {
                        count: 1,
                        status: "$_id.status",
                        assigneeName: "$assignee.name"
                    }
                }
            ]
        }
    }
]).toArray();
print("Dashboard data:");
printjson(dashboardData);
dashboardData