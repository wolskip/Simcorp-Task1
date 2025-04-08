# Word Count Aggregator

This repository contains a solution to a coding task provided by a recruiter.

## Problem Description

### Task: Word Count Aggregator
Write and test a C# program that:
- Counts the occurrences of each unique word across multiple text files.
- Aggregates the results efficiently, minimizing runtime regardless of the number of files or their size.

#### Example:
Given two files:
- File 1: "Go do that thing that you do so well"
- File 2: "I play football well"

The program should output:
```
1: Go
2: do
2: that
1: thing
1: you
1: so
2: well
1: I
1: play
1: football
```

---

## Build Instructions

1. Ensure you have the [.NET SDK](https://dotnet.microsoft.com/download) installed (version 7.x or later).
2. Clone the repository:
   ```bash
   git clone <repository-url>
   cd <repository-folder>
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build --configuration Release
   ```

---

## Run Instructions

1. Navigate to the project directory.
2. Run the program:
   ```bash
   dotnet run --project WordCount --configuration Release
   ```

---

## Tests

The project includes a comprehensive set of tests to ensure correctness and performance. These tests are implemented using the NUnit framework and can be run using the following command:
```bash
dotnet test
```

### Functional Tests
1. **Basic Word Count**: Verifies that the program correctly counts words in a file.
2. **Empty File Handling**: Ensures the program handles empty files gracefully.
3. **Non-Existent File Handling**: Confirms that the program reports an error for non-existent files.
4. **Multiple File Aggregation**: Tests the program's ability to aggregate word counts across multiple files.

### Performance Tests
1. **High Diverse Load**:
   - Tests the program's performance with a large number of unique words (e.g., 100,000 unique words).
   - Ensures the program completes within a reasonable time frame (e.g., less than 5 seconds).

2. **High Load with High Repetition**:
   - Evaluates the program's performance with a large number of repeated words (e.g., 1,000,000 repetitions of the same word).
   - Confirms that the program handles repetitive input efficiently.

3. **Impact of `--max-threads` Parameter**:
   - Compares execution time with a single thread (`--max-threads=1`) versus multiple threads (e.g., `--max-threads=16`).
   - Verifies that increasing the number of threads improves performance for large inputs.

4. **Impact of `--batch-size` Parameter**:
   - Tests the effect of different batch sizes on performance (e.g., `--batch-size=10` vs. `--batch-size=1000`).
   - Confirms that larger batch sizes improve performance by reducing overhead.

### Running Specific Tests
To run specific tests, use the `--filter` option. For example:
```bash
dotnet test --filter "Main_ShouldHandleHighDiverseLoad"
```

---

## Adding to Command Prompt with Environment Variables

To make the program accessible globally via the command prompt:
1. Build the project to generate the executable:
   ```bash
   dotnet publish --configuration Release --output <output-folder>
   ```
2. Add the `<output-folder>` path to your system's `PATH` environment variable:
   - On Windows:
     1. Open "Environment Variables" settings.
     2. Add the `<output-folder>` path to the `Path` variable under "System Variables."
   - On Linux/Mac:
     ```bash
     export PATH=$PATH:<output-folder>
     ```
3. You can now run the program from anywhere using:
   ```bash
   WordCount
   ```

---

## Notes
- Feel free to modify the code to suit your specific requirements.
- This software is provided "as is," without warranty of any kind, express or implied.

---