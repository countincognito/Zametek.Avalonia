namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IPersistenceProperty
    {
        string? Name { get; set; }

        string? Value { get; set; }
    }
}
