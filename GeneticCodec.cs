using System.Text;

namespace lab1;

public struct GeneticData
{
    public string ProteinName;
    public string OrganismName;
    public string AminoAcids;

    public static GeneticData FromString(string line)
    {
        string[] parts = line.Split('\t', 3);

        return new GeneticData
        {
            ProteinName = parts[0],
            OrganismName = parts[1],
            AminoAcids = GeneticCodec.Decode(parts[2])
        };
    }
}

public static class GeneticCodec
{

    public static List<GeneticData> ReadFile(string path)
    {
        List<GeneticData> data = new List<GeneticData>();

        foreach (string line in File.ReadLines(path))
        {
            if (!string.IsNullOrWhiteSpace(line))
                data.Add(GeneticData.FromString(line));
        }

        return data;
    }


    public static string Decode(string value)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < value.Length; i++)
        {
            if (!char.IsDigit(value[i]))
            {
                result.Append(value[i]);
                continue;
            }

            int count = value[i] - '0';
            result.Append(value[i + 1], count);
            i++;
        }

        return result.ToString();
    }

    public static string Encode(string value)
    {
        StringBuilder result = new StringBuilder();

        for (int i = 0; i < value.Length;)
        {
            int count = 1;
            while (i + count < value.Length && value[i] == value[i + count])
                count++;

            while (count > 9)
            {
                result.Append(9).Append(value[i]);
                count -= 9;
            }

            if (count >= 3)
            result.Append(count);
            result.Append(value[i]);

            i += count;
        }

        return result.ToString();
    }
}