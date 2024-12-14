namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IPersistence<TState, TElement, TProperty>
        where TState : IPersistenceState<TElement>, new()
        where TElement : IPersistenceElement<TProperty>, new()
        where TProperty : IPersistenceProperty, new()
    {
        void Load(IStateResourceAccess<TState> stateResourceAccess);

        void Save(IStateResourceAccess<TState> stateResourceAccess);

        void Persist(string uid, string propertyName, string value);

        bool Contains(string uid, string propertyName);

        string GetValue(string uid, string propertyName);

        void InitializeLookupTables();

        void Add(string uid, string propertyName, string value);

        void Update(string uid, string propertyName, string value);
    }
}
