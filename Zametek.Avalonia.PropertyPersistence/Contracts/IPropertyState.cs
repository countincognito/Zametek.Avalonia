using Avalonia;

namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IPropertyState<TState, TElement, TProperty>
        where TState : IPersistenceState<TElement>, new()
        where TElement : IPersistenceElement<TProperty>, new()
        where TProperty : IPersistenceProperty, new()
    {
        string ElementUid { get; }

        PropertyStateMode ElementMode { get; }

        Type ElementType { get; }

        bool HasPropertyValue(AvaloniaProperty property);

        object GetPropertyValue(AvaloniaProperty property);

        object AddPropertyValue(AvaloniaProperty property, object value);

        void UpdatePropertyValue(AvaloniaProperty property, object value);

        string Serialize(AvaloniaProperty prop, object value);

        object Deserialize(AvaloniaProperty prop, string stringValue);
    }
}
