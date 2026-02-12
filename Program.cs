﻿using NLog;

string path = Directory.GetCurrentDirectory() + "//nlog.xml";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

string file = "mario.csv";
// make sure movie file exists
if (!File.Exists(file))
{
    logger.Error("File does not exist: {File}", file);
}
else
{
    // TODO: create user menu
    string? choice;
    do
    {
        // display choices to user
        Console.WriteLine("1) Add Character");
        Console.WriteLine("2) Display All Characters");
        Console.WriteLine("Enter to quit");
        Console.Write("Choice: "); // FIX: prompt so user knows to type here

        // create parallel lists of character details
        // lists are used since we do not know number of lines of data. 
        List<UInt64> Ids = [];
        List<string> Names = [];
        List<string> Descriptions = [];
        List<string> Species = [];
        List<string> FirstAppearance = [];
        List<UInt64> YearCreated = [];

        // to populate the lists with data, read from the data file
        try
        {
            StreamReader sr = new(file);
            // first line contains column headers
            sr.ReadLine();
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line is not null)
                {
                    // character details are separated with comma(,)
                    string[] characterDetails = line.Split(',');
                    // 1st array element contains id
                    Ids.Add(UInt64.Parse(characterDetails[0]));
                    // 2nd array element contains character name
                    Names.Add(characterDetails[1]);
                    // 3rd array element contains character description
                    Descriptions.Add(characterDetails[2]);
                    Species.Add(characterDetails[3]);
                    FirstAppearance.Add(characterDetails[4]);
                    YearCreated.Add(UInt64.Parse(characterDetails[5]));
                }
            }
            sr.Close();
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }

        // input selection
        choice = Console.ReadLine();
        logger.Info("User choice: {Choice}", choice);

        if (choice == "1")
        {
            // Add Character
            Console.WriteLine("Enter new character name: ");
            string? Name = Console.ReadLine();

            if (!string.IsNullOrEmpty(Name))
            {
                // check for duplicate name
                List<string> LowerCaseNames = Names.ConvertAll(n => n.ToLower());
                if (LowerCaseNames.Contains(Name.ToLower()))
                {
                    logger.Info($"Duplicate name {Name}");
                    Console.WriteLine("That name already exists."); // FIX: give user feedback
                }
                else
                {
                    // generate id - use max value in Ids + 1
                    UInt64 Id = Ids.Max() + 1;

                    // input character description
                    Console.WriteLine("Enter description:");
                    string? Description = Console.ReadLine() ?? ""; // FIX: avoid null

                    // FIX: these must be SINGLE values, not your List variables
                    Console.WriteLine("Enter species:");
                    string characterSpecies = Console.ReadLine() ?? "";

                    Console.WriteLine("Enter first appearance:");
                    string characterFirstAppearance = Console.ReadLine() ?? "";

                    Console.WriteLine("Enter year created:");
                    string yearInput = Console.ReadLine() ?? "";

                    // FIX: validate numeric year
                    if (!UInt64.TryParse(yearInput, out UInt64 characterYearCreated))
                    {
                        logger.Error("Invalid year created: {YearInput}", yearInput);
                        Console.WriteLine("Year created must be a number.");
                    }
                    else
                    {
                        // create file from data
                        StreamWriter sw = new(file, true);

                        // FIX: write the SINGLE values, not the Lists
                        sw.WriteLine($"{Id},{Name},{Description},{characterSpecies},{characterFirstAppearance},{characterYearCreated}");
                        sw.Close();

                        // add new character details to Lists
                        Ids.Add(Id);
                        Names.Add(Name);
                        Descriptions.Add(Description);

                        // FIX: add the new values (not the list into itself)
                        Species.Add(characterSpecies);
                        FirstAppearance.Add(characterFirstAppearance);
                        YearCreated.Add(characterYearCreated);

                        // log transaction
                        logger.Info($"Character id {Id} added");
                        Console.WriteLine("Character added!");
                    }
                }
            }
            else
            {
                logger.Error("You must enter a name");
                Console.WriteLine("You must enter a name."); // FIX: user feedback
            }
        }
        else if (choice == "2")
        {
            // Display All Characters
            // loop thru Lists
            for (int i = 0; i < Ids.Count; i++)
            {
                // display character details
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
}

logger.Info("Program ended");
