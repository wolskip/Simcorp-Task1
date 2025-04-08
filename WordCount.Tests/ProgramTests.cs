using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

[TestFixture]
public class ProgramTests
{
    [Test]
    public void GetWordsFromFile_ShouldReturnCorrectWords()
    {
        // Arrange
        var testContent = "Hello world! Hello again.";
        var expectedWords = new[] { "hello", "world", "hello", "again" };
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, testContent);

        try
        {
            // Act
            var words = Program.GetWordsFromFile(tempFile);

            // Assert
            Assert.AreEqual(expectedWords, words);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void GetWordsFromFile_ShouldHandleEmptyFile()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, string.Empty);

        try
        {
            // Act
            var words = Program.GetWordsFromFile(tempFile);

            // Assert
            Assert.IsEmpty(words);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Main_ShouldHandleNonExistentFile()
    {
        // Arrange
        var nonExistentFile = "nonexistent.txt";
        var args = new[] { nonExistentFile };

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        Program.Main(args);

        // Assert
        var output = consoleOutput.ToString();
        StringAssert.Contains($"Error - file not found: {nonExistentFile}", output);
    }

    [Test]
    public void Main_ShouldCountWordsCorrectly()
    {
        // Arrange
        var tempFile1 = Path.GetTempFileName();
        var tempFile2 = Path.GetTempFileName();
        File.WriteAllText(tempFile1, "Hello world");
        File.WriteAllText(tempFile2, "Hello again");

        var args = new[] { tempFile1, tempFile2 };

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // Act
            Program.Main(args);

            // Assert
            var output = consoleOutput.ToString();
            StringAssert.Contains("hello: 2", output);
            StringAssert.Contains("world: 1", output);
            StringAssert.Contains("again: 1", output);
        }
        finally
        {
            File.Delete(tempFile1);
            File.Delete(tempFile2);
        }
    }

    [Test]
    public void Main_ShouldRespectMaxThreadsParameter()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "Hello world Hello again");

        var args = new[] { tempFile, "--max-threads=1" };

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // Act
            Program.Main(args);

            // Assert
            var output = consoleOutput.ToString();
            StringAssert.Contains("hello: 2", output);
            StringAssert.Contains("world: 1", output);
            StringAssert.Contains("again: 1", output);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Main_ShouldRespectBatchSizeParameter()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "Hello world Hello again");

        var args = new[] { tempFile, "--batch-size=1" };

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // Act
            Program.Main(args);

            // Assert
            var output = consoleOutput.ToString();
            StringAssert.Contains("hello: 2", output);
            StringAssert.Contains("world: 1", output);
            StringAssert.Contains("again: 1", output);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Main_ShouldHandleHighDiverseLoad()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var diverseContent = string.Join(" ", Enumerable.Range(1, 100000).Select(i => $"word{i}"));
        File.WriteAllText(tempFile, diverseContent);

        var args = new[] { tempFile };

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // Act
            var stopwatch = Stopwatch.StartNew();
            Program.Main(args);
            stopwatch.Stop();

            // Assert
            Assert.Less(stopwatch.ElapsedMilliseconds, 5000, "High diverse load took too long.");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Main_ShouldHandleHighLoadWithHighRepetition()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var repetitiveContent = string.Join(" ", Enumerable.Repeat("hello", 1000000));
        File.WriteAllText(tempFile, repetitiveContent);

        var args = new[] { tempFile };

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        try
        {
            // Act
            var stopwatch = Stopwatch.StartNew();
            Program.Main(args);
            stopwatch.Stop();

            // Assert
            Assert.Less(stopwatch.ElapsedMilliseconds, 5000, "High repetitive load took too long.");
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Main_ShouldShowPerformanceImpactOfMaxThreads()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var content = string.Join(" ", Enumerable.Repeat("hello world", 100000));
        File.WriteAllText(tempFile, content);

        var argsWithOneThread = new[] { tempFile, "--max-threads=1" };
        var argsWithMultipleThreads = new[] { tempFile, "--max-threads=16" };

        var originalConsoleOut = Console.Out;

        try
        {
            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var stopwatchOneThread = Stopwatch.StartNew();
            Program.Main(argsWithOneThread);
            stopwatchOneThread.Stop();

            consoleOutput.GetStringBuilder().Clear();

            var stopwatchMultipleThreads = Stopwatch.StartNew();
            Program.Main(argsWithMultipleThreads);
            stopwatchMultipleThreads.Stop();

            // Assert
            Assert.Greater(stopwatchOneThread.ElapsedMilliseconds, stopwatchMultipleThreads.ElapsedMilliseconds, "Multiple threads did not improve performance.");
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            File.Delete(tempFile);
        }
    }

    [Test]
    public void Main_ShouldShowPerformanceImpactOfBatchSize()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();
        var content = string.Join(" ", Enumerable.Repeat("hello world", 100000));
        File.WriteAllText(tempFile, content);

        var argsWithSmallBatch = new[] { tempFile, "--batch-size=10" };
        var argsWithLargeBatch = new[] { tempFile, "--batch-size=1000" };

        var originalConsoleOut = Console.Out;

        try
        {
            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var stopwatchSmallBatch = Stopwatch.StartNew();
            Program.Main(argsWithSmallBatch);
            stopwatchSmallBatch.Stop();

            consoleOutput.GetStringBuilder().Clear();

            var stopwatchLargeBatch = Stopwatch.StartNew();
            Program.Main(argsWithLargeBatch);
            stopwatchLargeBatch.Stop();

            // Assert
            Assert.Greater(stopwatchSmallBatch.ElapsedMilliseconds, stopwatchLargeBatch.ElapsedMilliseconds, "Larger batch size did not improve performance.");
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
            File.Delete(tempFile);
        }
    }
}
