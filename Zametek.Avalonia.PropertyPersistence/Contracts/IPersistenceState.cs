namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IPersistenceState<TElement>
    {
        List<TElement> Elements { get; }
    }
}
