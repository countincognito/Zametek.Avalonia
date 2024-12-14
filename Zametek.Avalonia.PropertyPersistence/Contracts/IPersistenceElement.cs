namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IPersistenceElement<TProperty>
    {
        List<TProperty> Properties { get; }

        string? Uid { get; set; }
    }
}
