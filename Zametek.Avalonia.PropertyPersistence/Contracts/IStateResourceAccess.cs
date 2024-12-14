namespace Zametek.Avalonia.PropertyPersistence
{
    public interface IStateResourceAccess<TState>
    {
        TState? Load();

        void Save(TState state);
    }
}
