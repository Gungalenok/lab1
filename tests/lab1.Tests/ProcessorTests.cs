using lab1;
using Xunit;

namespace lab1.Tests;

public class ProcessorTests
{
    [Fact]
    public void Search_returns_each_matching_record()
    {
        List<GeneticData> data = [Protein("one", "ACDEF"), Protein("two", "TTAC"), Protein("three", "GG")];

        List<GeneticData> result = Processor.Search(data, "AC");

        Assert.Equal(["one", "two"], result.Select(p => p.ProteinName));
    }

    [Theory]
    [InlineData("", "", 0)]
    [InlineData("ACD", "ACD", 0)]
    [InlineData("ACD", "ADD", 1)]
    [InlineData("AC", "ACDEF", 3)]
    [InlineData("ACDEF", "TC", 4)]
    [InlineData("", "AAA", 3)]
    public void Diff_counts_mismatches_and_unmatched_tail(string first, string second, int expected)
    {
        Assert.Equal(expected, Processor.Diff(Protein("first", first), Protein("second", second)));
    }

    [Fact]
    public void FindProtein_returns_matching_record_or_null()
    {
        List<GeneticData> data = [Protein("known", "ABC")];

        Assert.Equal("ABC", Processor.FindProtein(data, "known")?.AminoAcids);
        Assert.Null(Processor.FindProtein(data, "missing"));
    }

    [Theory]
    [InlineData("CCAA", 'A')]
    [InlineData("V", 'V')]
    [InlineData("ACCCCCA", 'C')]
    public void Mode_returns_most_frequent_amino_acid(string sequence, char expected)
    {
        Assert.Equal(expected, Processor.Mode(Protein("protein", sequence)));
    }

    [Fact]
    public void ExecuteCommands_writes_search_diff_mode_and_missing_results()
    {
        WithTempDirectory(directory =>
        {
            string dataPath = Path.Combine(directory, "sequences.txt");
            string commandsPath = Path.Combine(directory, "commands.txt");
            string resultPath = Path.Combine(directory, "result.txt");
            File.WriteAllText(dataPath, "one\torganism-one\tACD\ntwo\torganism-two\tADD\n");
            File.WriteAllText(commandsPath, "search\tAC\ndiff\tone\ttwo\nmode\tone\ndiff\tone\tmissing\nmode\tmissing\nsearch\tZZ\n");

            Processor.ExecuteCommands(commandsPath, dataPath, resultPath);

            string output = File.ReadAllText(resultPath);
            Assert.Contains("001\tsearch\tAC", output);
            Assert.Contains("organism-one\t\tone", output);
            Assert.Contains("002\tdiff\tone\ttwo", output);
            Assert.Contains($"amino-acids difference:{Environment.NewLine}1", output);
            Assert.Contains("003\tmode\tone", output);
            Assert.Contains("A\t\t1", output);
            Assert.Contains("MISSING: missing", output);
            Assert.Contains("NOT FOUND", output);
        });
    }

    private static GeneticData Protein(string name, string sequence) => new()
    {
        ProteinName = name,
        OrganismName = $"organism-{name}",
        AminoAcids = sequence
    };

    private static void WithTempDirectory(Action<string> action)
    {
        string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            action(directory);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
