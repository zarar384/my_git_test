export default function executionTimeAsync(
  fn: () => void,
  isLog: boolean = false
): number {
  const start = performance.now();
  try {
    const result = fn();
    const end = performance.now();
    if (isLog) console.log(result);
    return end - start;
  } catch (error) {
    console.error(`Error during function execution: ${error}`);
  }
}
