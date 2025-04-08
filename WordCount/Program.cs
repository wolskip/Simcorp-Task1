using System.Collections.Concurrent;
using System.Text.RegularExpressions;

/// <summary>
/// The main program class for counting words in files.
/// </summary>
public class Program
{
    /// <summary>
    /// The entry point of the program.
    /// </summary>
    /// <param name="args">
    /// Command-line arguments specifying file paths and optional parameters:
    /// <list type="bullet">
    /// <item><description><c>--max-threads=N</c>: Sets the maximum number of threads.</description></item>
    /// <item><description><c>--batch-size=N</c>: Sets the batch size for processing words. Set to 1000 bt default </description></item>
    /// <item><description>File paths to process.</description></item>
    /// </list>
    /// </param>
    static public void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Error - no files provided. Try: dotnet run <file1> <file2> ... [--max-threads=N] [--batch-size=N(1000)]");
            return;
        }

        bool listNewFiles = args.Contains("--list-new-files");
        int maxThreads = args.FirstOrDefault(arg => arg.StartsWith("--max-threads=")) is string threadArg && int.TryParse(threadArg.Split('=')[1], out var threads) ? threads : Environment.ProcessorCount;
        int batchSize = args.FirstOrDefault(arg => arg.StartsWith("--batch-size=")) is string batchArg && int.TryParse(batchArg.Split('=')[1], out var batch) ? batch : 1000;

        var files = args.Where(arg => !arg.StartsWith("--")).ToArray();

        var wordCounts = new ConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var options = new ParallelOptions { MaxDegreeOfParallelism = maxThreads };

        Parallel.ForEach(files, options, file =>
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Error - file not found: {file}");
            }

            try
            {
                var words = GetWordsFromFile(file).ToList();
                for (int i = 0; i < words.Count; i += batchSize)
                {
                    var batch = words.Skip(i).Take(batchSize);
                    Parallel.ForEach(batch, options, word =>
                    {
                        wordCounts.AddOrUpdate(word, 1, (_, count) => count + 1);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {file}: {ex.Message}");
            }
        });

        foreach (var kvp in wordCounts.OrderBy(kvp => kvp.Key))
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }

    /// <summary>
    /// Reads words from a file using a regular expression.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>An enumerable collection of words found in the file.</returns>
    public static IEnumerable<string> GetWordsFromFile(string filePath)
    {
        var words = new List<string>();
        var regex = CompiledRegex;

        using (var reader = new StreamReader(filePath))
        {
            IEnumerable<string> lines = File.ReadLines(filePath);
            foreach (var line in lines)
            {
                words.AddRange(regex.Matches(line).Select(m => m.Value.ToLower()));
            }
        }

        return words;
    }

    /// <summary>
    /// A compiled regular expression for matching words.
    /// This regex matches word characters (letters, digits, and underscores).
    /// it's compiled for better performance.
    /// </summary>
    private static readonly Regex CompiledRegex = new Regex(@"\b\w+\b", RegexOptions.Compiled);
}