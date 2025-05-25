namespace LeaveMeAloneFuncSkillForge.Utils
{
    public static class FrameworkExtensions
    {
        public static void PrintTable<T>(this IEnumerable<T> items)
        {
            if (items == null || !items.Any())
            {
                Console.WriteLine("No data.");
                return;
            }

            var properties = typeof(T).GetProperties();

            // Max columns width
            var headers = properties.Select(p => p.Name).ToArray();
            var columns = properties.Select(p => new
            {
                Property = p,
                Header = p.Name,
                Width = Math.Max(
                    p.Name.Length,
                    items.Select(x => p.GetValue(x)?.ToString()?.Length ?? 0).Max()
                )
            }).ToArray();

            // Columns Name
            foreach (var col in columns)
            {
                Console.Write($"{col.Header.PadRight(col.Width + 2)}");
            }
            Console.WriteLine();

            // -
            foreach (var col in columns)
            {
                Console.Write(new string('-', col.Width + 2));
            }
            Console.WriteLine();

            // Rows
            foreach (var item in items)
            {
                foreach (var col in columns)
                {
                    var value = col.Property.GetValue(item)?.ToString() ?? "";
                    Console.Write($"{value.PadRight(col.Width + 2)}");
                }
                Console.WriteLine();
            }
        }
    }
}
