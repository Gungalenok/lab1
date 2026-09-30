namespace lab1;

public static class Processor
{
    private const string Separator =
        "--------------------------------------------------------------------------";

    public static void ExecuteCommands(
        string commandsPath,
        string dataPath,
        string resultPath)
    {
        List<GeneticData> data = GeneticCodec.ReadFile(dataPath);
        using StreamWriter writer = new StreamWriter(resultPath);

        writer.WriteLine("Pavel Bulava");
        writer.WriteLine("Genetic Searching");
        writer.WriteLine(Separator);

        int commandNumber = 1;

        foreach (string line in File.ReadLines(commandsPath))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            ExecuteCommand(line, commandNumber, data, writer);
            commandNumber++;
        }
        Console.WriteLine($"Результат сохранён в файл: {resultPath}");
    }

    public static List<GeneticData> Search(
        List<GeneticData> data,
        string sequence)
    {
        return data
            .Where(item => item.AminoAcids.Contains(sequence))
            .ToList();
    }

    public static int Diff(GeneticData first, GeneticData second)
    {
        int commonLength = Math.Min(
            first.AminoAcids.Length,
            second.AminoAcids.Length
        );
        int difference = Math.Abs(
            first.AminoAcids.Length - second.AminoAcids.Length
        );

        for (int i = 0; i < commonLength; i++)
        {
            if (first.AminoAcids[i] != second.AminoAcids[i])
                difference++;
        }

        return difference;
    }

    public static GeneticData? FindProtein(
        List<GeneticData> data,
        string proteinName)
    {
        int index = data.FindIndex(p => p.ProteinName == proteinName);
        return index >= 0 ? data[index] : null;
    }

    public static char Mode(GeneticData data)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();

        foreach (char aminoAcid in data.AminoAcids)
        {
            if (counts.ContainsKey(aminoAcid))
                counts[aminoAcid]++;
            else
                counts[aminoAcid] = 1;
        }

        return counts
            .OrderByDescending(item => item.Value)
            .ThenBy(item => item.Key)
            .First()
            .Key;
    }

    private static void ExecuteCommand(
        string line,
        int commandNumber,
        List<GeneticData> data,
        StreamWriter writer)
    {
        string[] parts = line.Split('\t');
        switch (parts[0])
        {
            case "search":
                ExecuteSearch(parts[1], commandNumber, data, writer);
                break;
            case "diff":
                ExecuteDiff(parts[1], parts[2], commandNumber, data, writer);
                break;
            case "mode":
                ExecuteMode(parts[1], commandNumber, data, writer);
                break;
            default:
                throw new FormatException(
                    $"Unknown command '{parts[0]}' at line {commandNumber}."
                );
        }
    }

    private static void ExecuteSearch(
        string encodedSequence,
        int number,
        List<GeneticData> data,
        StreamWriter writer)
    {
        string sequence = GeneticCodec.Decode(encodedSequence);
        List<GeneticData> found = Search(data, sequence);

        writer.WriteLine($"{number:D3}\tsearch\t{sequence}");
        writer.WriteLine("organism\t\t\t\tprotein");

        if (found.Count == 0)
        {
            writer.WriteLine("NOT FOUND");
        }
        else
        {
            foreach (GeneticData protein in found)
            {
                writer.WriteLine(
                    $"{protein.OrganismName}\t\t{protein.ProteinName}"
                );
            }
        }

        writer.WriteLine(Separator);
    }

    private static void ExecuteDiff(
        string firstName,
        string secondName,
        int number,
        List<GeneticData> data,
        StreamWriter writer)
    {
        GeneticData? first = FindProtein(data, firstName);
        GeneticData? second = FindProtein(data, secondName);

        writer.WriteLine($"{number:D3}\tdiff\t{firstName}\t{secondName}");
        writer.WriteLine("amino-acids difference:");

        if (first == null || second == null)
        {
            List<string> missing = new List<string>();

            if (first == null)
                missing.Add(firstName);
            if (second == null)
                missing.Add(secondName);

            writer.WriteLine($"MISSING: {string.Join(' ', missing)}");
        }
        else
        {
            writer.WriteLine(Diff(first.Value, second.Value));
        }

        writer.WriteLine(Separator);
    }

    private static void ExecuteMode(
        string proteinName,
        int number,
        List<GeneticData> data,
        StreamWriter writer)
    {
        GeneticData? protein = FindProtein(data, proteinName);

        writer.WriteLine($"{number:D3}\tmode\t{proteinName}");
        writer.WriteLine("amino-acid occurs:");

        if (protein == null)
        {
            writer.WriteLine($"MISSING: {proteinName}");
        }
        else
        {
            char aminoAcid = Mode(protein.Value);
            int count = protein.Value.AminoAcids.Count(c => c == aminoAcid);
            writer.WriteLine($"{aminoAcid}\t\t{count}");
        }

        writer.WriteLine(Separator);
    }
}
