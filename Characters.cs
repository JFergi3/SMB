class Character
{
    public UInt64 Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string FirstAppearance { get; set; } = string.Empty;
    public UInt64 YearCreated { get; set; }

    public string Display()
    {
        return $"Id: {Id}\nName: {Name}\nDescription: {Description}\nSpecies: {Species}\nFirst appearance: {FirstAppearance}\nYear Created: {YearCreated}\n";
    }
}
