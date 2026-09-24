using lab1;

string outputDirectory = AppContext.BaseDirectory;
string commandsPath = Path.Combine(outputDirectory, "commands.txt");
string sequencesPath = Path.Combine(outputDirectory, "sequences.txt");
string resultPath = Path.Combine(outputDirectory, "genedata.txt");

Processor.ExecuteCommands(commandsPath, sequencesPath, resultPath);