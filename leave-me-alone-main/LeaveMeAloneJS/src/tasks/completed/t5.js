const value = 0.2;
console.log(minMnozstvi(value));

function minMnozstvi(e) {
  if (e === 1 || e >= 1 || e <= 0) return 1;
  const decSep = e.toString().includes('.') ? '.' : (e.toString().includes(',') ? ',' : null);
  const fractionalPart = e.toString().split(decSep)[1];
  const valueModified = "0" + decSep + "0".repeat(fractionalPart.length - 1) + "1";
  return parseFloat(valueModified);
}
