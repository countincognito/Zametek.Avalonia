namespace Zametek.Avalonia.PropertyPersistence
{
    public static class PropertyStateHelper
    {
        public static void Load(IStateResourceAccess<PersistenceState> stateResourceAccess)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException(nameof(stateResourceAccess));
            }
            PropertyStateExtension.Load(
                stateResourceAccess,
                x => new PropertyState(x));
        }

        public static void Save(IStateResourceAccess<PersistenceState> stateResourceAccess)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException(nameof(stateResourceAccess));
            }
            PropertyStateExtension.Save(stateResourceAccess);
        }
    }
}
