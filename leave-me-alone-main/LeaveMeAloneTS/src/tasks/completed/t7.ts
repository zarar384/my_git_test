function isValidSudoku(board: string[][]): boolean {
  if (!board.every((s) => s.length === 9)) return false;
  const flattenedArray = board.flatMap((row) => row);
  if (flattenedArray.length !== 81) return false;

  function isValidSet(elements: string[]): boolean {
    const seen = new Set<string>();
    for (const element of elements) {
      if (element !== ".") {
        if (seen.has(element)) return false;
        seen.add(element);
      }
    }
    return true;
  }

  for (let i = 0; i < 9; i++) {
    if (!isValidSet(board[i])) return false;
  }

  // Проверка столбцов
  for (let j = 0; j < 9; j++) {
    const column = board.map((row) => row[j]);
    if (!isValidSet(column)) return false;
  }

  const combinedArray = Array.from({ length: 9 }, (_, rowIndex) =>
    Array.from(
      { length: 9 },
      (_, colIndex) =>
        board[Math.floor(rowIndex / 3) * 3 + Math.floor(colIndex / 3)][
          (rowIndex % 3) * 3 + (colIndex % 3)
        ]
    )
  );
  let counter = 0;
  var subBox = [];
  var isEmpty = true;
  var arr = combinedArray.flat();
  var valid = arr.some((e) => {
    if (counter === 9) {
      counter = 0;
      subBox = [];
    }

    var val = e.replace("'", "");
    var num = Number(val);
    if (isFinite(num)) {
      if (num > 9 || num <= 0) return true;
      if (subBox.includes(e)) return true;
      subBox.push(e);
      isEmpty = false;
    }

    counter++;
    return false;
  });

  return isEmpty || !valid;
}

var board = [
  ["5", "3", ".", ".", "7", ".", ".", ".", "."],
  ["6", ".", ".", "1", "9", "5", ".", ".", "."],
  [".", "9", "8", ".", ".", ".", ".", "6", "."],
  ["8", ".", ".", ".", "6", ".", ".", ".", "3"],
  ["4", ".", ".", "8", ".", "3", ".", ".", "1"],
  ["7", ".", ".", ".", "2", ".", ".", ".", "6"],
  [".", "6", ".", ".", ".", ".", "2", "8", "."],
  [".", ".", ".", "4", "1", "9", ".", ".", "5"],
  [".", ".", ".", ".", "8", ".", ".", "7", "9"],
];

console.log(isValidSudoku(board));

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

var board3 = [
  [".", ".", "4", ".", ".", ".", "6", "3", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  ["5", ".", ".", ".", ".", ".", ".", "9", "."],
  [".", ".", ".", "5", "6", ".", ".", ".", "."],
  ["4", ".", "3", ".", ".", ".", ".", ".", "1"],
  [".", ".", ".", "7", ".", ".", ".", ".", "."],
  [".", ".", ".", "5", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
];

console.log(isValidSudoku(board3)); //false

var board4 = [
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
  [".", ".", ".", ".", ".", ".", ".", ".", "."],
];

console.log(isValidSudoku(board4)); //true

var board5 = [
  [".", "8", "7", "6", "5", "4", "3", "2", "1"],
  ["2", ".", ".", ".", ".", ".", ".", ".", "."],
  ["3", ".", ".", ".", ".", ".", ".", ".", "."],
  ["4", ".", ".", ".", ".", ".", ".", ".", "."],
  ["5", ".", ".", ".", ".", ".", ".", ".", "."],
  ["6", ".", ".", ".", ".", ".", ".", ".", "."],
  ["7", ".", ".", ".", ".", ".", ".", ".", "."],
  ["8", ".", ".", ".", ".", ".", ".", ".", "."],
  ["9", ".", ".", ".", ".", ".", ".", ".", "."],
];

console.log(isValidSudoku(board5)); //true
