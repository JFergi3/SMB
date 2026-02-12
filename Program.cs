﻿using NLog;

string path = Path.Combine(Directory.GetCurrentDirectory(), "nlog.xml");
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string file = "mario.csv";

if (!File.Exists(file))
{
    logger.Error("File does not exist: {File}", file);
    Console.WriteLine($"ERROR: File does not exist: {file}");
    logger.Info("Program ended");
    return;
}

// Create parallel lists ONCE (outside the loop)
List<UInt64> Ids = new();
List<string> Names = new();
List<string> Descriptions = new();
List<string> Species = new();
List<string> FirstAppearance = new();
List<UInt64> YearCreated = new();

// Load file ONCE at start (optional but cleaner)
LoadCharacters(file, Ids, Names, Descriptions, Species, FirstAppearance, YearCreated, logger);

string? choice;
do
{
    Console.WriteLine("1) Add Character");
    Console.WriteLine("2) Display All Characters");
    Console.WriteLine("Enter to quit");
    Console.Write("Choice: ");

    choice = Console.ReadLine();
    logger.Info("User choice: {Choice}", choice);

    if (choice == "1")
    {
        Console.Write("Enter new character name: ");
        string? name = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            logger.Error("You must enter a name");
            Console.WriteLine("You must enter a name.");
            continue;
        }

        // check for duplicate name (case-insensitive)
        bool duplicate = Names.Any(n => n.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (duplicate)
        {
            logger.Info("Duplicate name {Name}", name);
            Console.WriteLine("That name already exists.");
            continue;
        }

        Console.Write("Enter description: ");
        string? description = Console.ReadLine() ?? "";

        Console.Write("Enter species: ");
        string? species = Console.ReadLine() ?? "";

        Console.Write("Enter first appearance: ");
        string? firstAppearance = Console.ReadLine() ?? "";

        Console.Write("Enter year created: ");
        string? yearInput = Console.ReadLine(); 

        if (!UInt64.TryParse(yearInput, out UInt64 yearCreated))
        {
            Console.WriteLine("Year must be a number.");
            logger.Error("Invalid year input: {YearInput}", yearInput);
            continue;
        }

        UInt64 id = Ids.Count == 0 ? 1 : Ids.Max() + 1;

        // Append to file (write SINGLE values, not lists)
        try
        {
            using StreamWriter sw = new(file, append: true);
            sw.WriteLine($"{id},{name},{description},{species},{firstAppearance},{yearCreated}");
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to write to file.");
            Console.WriteLine("Failed to save character.");
            continue;
        }

        // Add to lists
        Ids.Add(id);
        Names.Add(name);
        Descriptions.Add(description);
        Species.Add(species);
        FirstAppearance.Add(firstAppearance);
        YearCreated.Add(yearCreated);

        logger.Info("Character id {Id} added", id);
        Console.WriteLine("Character added!");
    }
    else if (choice == "2")
    {
        for (int i = 0; i < Ids.Count; i++)
        {
            Console.WriteLine($"Id: {Ids[i]}");
            Console.WriteLine($"Name: {Names[i]}");
            Console.WriteLine($"Description: {Descriptions[i]}");
            Console.WriteLine($"Species: {Species[i]}");
            Console.WriteLine($"First appearance: {FirstAppearance[i]}");
            Console.WriteLine($"Year Created: {YearCreated[i]}");
            Console.WriteLine();
        }
    }

} while (choice == "1" || choice == "2");

logger.Info("Program ended");

static void LoadCharacters(
    string file,
    List<UInt64> ids,
    List<string> names,
    List<string> descriptions,
    List<string> species,
    List<string> firstAppearance,
    List<UInt64> yearCreated,
    Logger logger)
{
    try
    {
        using StreamReader sr = new(file);

        // header
        sr.ReadLine();

        while (!sr.EndOfStream)
        {
            string? line = sr.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');

            // basic safety check
            if (parts.Length < 6)
            {
                logger.Warn("Skipping malformed line: {Line}", line);
                continue;
            }

            ids.Add(UInt64.Parse(parts[0]));
            names.Add(parts[1]);
            descriptions.Add(parts[2]);
            species.Add(parts[3]);
            firstAppearance.Add(parts[4]);
            yearCreated.Add(UInt64.Parse(parts[5]));
        }
    }
    catch (Exception ex)
    {
        logger.Error(ex, "Failed to load characters");
    }
}
