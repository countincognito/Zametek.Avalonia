namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IPropertyStateExtension<TState, TElement, TProperty>
        where TState : IPersistenceState<TElement>, new()
        where TElement : IPersistenceElement<TProperty>, new()
        where TProperty : IPersistenceProperty, new()
    {
        object Default { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider);
    }
}
