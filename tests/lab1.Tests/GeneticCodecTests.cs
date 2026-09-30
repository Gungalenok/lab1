using lab1;
using Xunit;

namespace lab1.Tests;

public class GeneticCodecTests
{
    [Theory]
    [InlineData("", "")]
    [InlineData("AA", "AA")]
    [InlineData("FK3I", "FKIII")]
    [InlineData("3A4C", "AAACCCC")]
    [InlineData("9A", "AAAAAAAAA")]
    [InlineData("8ATA3TCGC4TC5ATTGTCAGATGAGAG6AT4A", "AAAAAAAATATTTCGCTTTTCAAAAATTGTCAGATGAGAGAAAAAATAAAA")]
    public void Decode_expands_digit_character_pairs(string encoded, string expected)
    {
        Assert.Equal(expected, GeneticCodec.Decode(encoded));
    }

    [Fact]
    public void FromString_parses_names_and_decodes_sequence()
    {
        GeneticData protein = GeneticData.FromString("protein-1\torganism-1\t3A2C");

        Assert.Equal("protein-1", protein.ProteinName);
        Assert.Equal("organism-1", protein.OrganismName);
        Assert.Equal("AAACC", protein.AminoAcids);
    }

    [Fact]
    public void ReadFile_skips_blank_lines_and_decodes_records()
    {
        WithTempFile("one\torganism\t2A\n\n two\tother\tC\n", path =>
        {
            List<GeneticData> data = GeneticCodec.ReadFile(path);

            Assert.Equal(2, data.Count);
            Assert.Equal("AA", data[0].AminoAcids);
            Assert.Equal(" two", data[1].ProteinName);
        });
    }

    private static void WithTempFile(string contents, Action<string> action)
    {
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, contents);
            action(path);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
