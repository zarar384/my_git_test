function isValidSudoku(board) {
    if (!board.every(function (s) { return s.length === 9; }))
        return false;
    var flattenedArray = board.flatMap(function (row) { return row; });
    if (flattenedArray.length !== 81)
        return false;
    var combinedArray = Array.from({ length: 9 }, function (_, rowIndex) {
        return Array.from({ length: 9 }, function (_, colIndex) {
            return board[Math.floor(rowIndex / 3) * 3 + Math.floor(colIndex / 3)][(rowIndex % 3) * 3 + (colIndex % 3)];
        });
    });
    var counter = 0;
    var subBox = [];
    combinedArray.flat().forEach(function (e) {
        if (counter === 9) {
            counter = 0;
            subBox = [];
        }
        var val = e.replace("'", "");
        var num = Number(val);
        if (isFinite(num)) {
            if (subBox.includes(e))
                return false;
            subBox.push(e);
        }
        counter++;
    });
    return true;
}
// var board = [
//   ["5", "3", ".", ".", "7", ".", ".", ".", "."],
//   ["6", ".", ".", "1", "9", "5", ".", ".", "."],
//   [".", "9", "8", ".", ".", ".", ".", "6", "."],
//   ["8", ".", ".", ".", "6", ".", ".", ".", "3"],
//   ["4", ".", ".", "8", ".", "3", ".", ".", "1"],
//   ["7", ".", ".", ".", "2", ".", ".", ".", "6"],
//   [".", "6", ".", ".", ".", ".", "2", "8", "."],
//   [".", ".", ".", "4", "1", "9", ".", ".", "5"],
//   [".", ".", ".", ".", "8", ".", ".", "7", "9"],
// ];
// console.log(isValidSudoku(board));
var board2 = [
    ["8", "3", ".", ".", "7", ".", ".", ".", "."],
    ["6", ".", ".", "1", "9", "5", ".", ".", "."],
    [".", "9", "8", ".", ".", ".", ".", "6", "."],
    ["8", ".", ".", ".", "6", ".", ".", ".", "3"],
    ["4", ".", ".", "8", ".", "3", ".", ".", "1"],
    ["7", ".", ".", ".", "2", ".", ".", ".", "6"],
    [".", "6", ".", ".", ".", ".", "2", "8", "."],
    [".", ".", ".", "4", "1", "9", ".", ".", "5"],
    [".", ".", ".", ".", "8", ".", ".", "7", "9"],
];
console.log(isValidSudoku(board2));
