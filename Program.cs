﻿using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.xml";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string file = "mario.csv";
if (!File.Exists(file))
{
    logger.Error("File does not exist: {File}", file);
}
else
{
    string? choice;

    do
    {
        List<Character> characters = new();

        try
        {
            using StreamReader sr = new(file);
            sr.ReadLine();

            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] characterDetails = line.Split(',');
                if (characterDetails.Length < 6)
                {
                    logger.Warn("Skipping invalid row: {Row}", line);
                    continue;
                }

                if (!UInt64.TryParse(characterDetails[0], out UInt64 id))
                {
                    logger.Warn("Skipping row with invalid id: {Row}", line);
                    continue;
                }

                if (!UInt64.TryParse(characterDetails[5], out UInt64 yearCreated))
                {
                    logger.Warn("Skipping row with invalid year: {Row}", line);
                    continue;
                }

                Character character = new()
                {
                    Id = id,
                    Name = characterDetails[1],
                    Description = characterDetails[2],
                    Species = characterDetails[3],
                    FirstAppearance = characterDetails[4],
                    YearCreated = yearCreated
                };

                characters.Add(character);
            }
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }

        // display choices to user
        Console.WriteLine("1) Add Character");
        Console.WriteLine("2) Display All Characters");
        Console.WriteLine("Enter to quit");
        Console.Write("Choice: ");

        // input selection
        choice = Console.ReadLine();
        logger.Info("User choice: {Choice}", choice);

        if (choice == "1")
        {
            // Add Character
            Console.WriteLine("Enter new character name: ");
            string? name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
            {
                // check for duplicate name
                bool duplicate = characters.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (duplicate)
                {
                    logger.Info("Duplicate name {Name}", name);
                    Console.WriteLine("That name already exists.");
                }
                else
                {
                    // generate id - use max value in existing items + 1
                    UInt64 id = characters.Count > 0 ? characters.Max(c => c.Id) + 1 : 1;

                    // input character details
                    Console.WriteLine("Enter description:");
                    string description = Console.ReadLine() ?? string.Empty;

                    Console.WriteLine("Enter species:");
                    string species = Console.ReadLine() ?? string.Empty;

                    Console.WriteLine("Enter first appearance:");
                    string firstAppearance = Console.ReadLine() ?? string.Empty;

                    Console.WriteLine("Enter year created:");
                    string yearInput = Console.ReadLine() ?? string.Empty;

                    if (!UInt64.TryParse(yearInput, out UInt64 yearCreated))
                    {
                        logger.Error("Invalid year created: {YearInput}", yearInput);
                        Console.WriteLine("Year created must be a number.");
                    }
                    else
                    {
                        using StreamWriter sw = new(file, true);
                        sw.WriteLine($"{id},{name},{description},{species},{firstAppearance},{yearCreated}");

                        logger.Info("Character id {Id} added", id);
                        Console.WriteLine("Character added!");
                    }
                }
            }
            else
            {
                logger.Error("You must enter a name");
                Console.WriteLine("You must enter a name.");
            }
        }
        else if (choice == "2")
        {
            // Display All Characters
            foreach (Character character in characters)
            {
                Console.WriteLine(character.Display());
            }
        }

    } while (choice == "1" || choice == "2");
}

logger.Info("Program ended");
