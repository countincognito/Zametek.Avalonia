﻿namespace Zametek.Avalonia.PropertyPersistence
{
    public class Persistence<TState, TElement, TProperty>
        : IPersistence<TState, TElement, TProperty>
        where TState : IPersistenceState<TElement>, new()
        where TElement : IPersistenceElement<TProperty>, new()
        where TProperty : IPersistenceProperty, new()
    {
        #region Fields

        private TState m_State;
        private readonly Dictionary<string, TElement> m_StateElementsLookup;

        #endregion


        public Persistence()
        {
            m_State = new TState();
            m_StateElementsLookup = new Dictionary<string, TElement>();
        }


        public void Load(IStateResourceAccess<TState> stateResourceAccess)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException(nameof(stateResourceAccess));
            }
            try
            {
                m_State = stateResourceAccess.Load() ?? new TState();
            }
            finally
            {
                InitializeLookupTables();
            }
        }

        public void Save(IStateResourceAccess<TState> stateResourceAccess)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException(nameof(stateResourceAccess));
            }
            stateResourceAccess.Save(m_State);
        }

        public void Persist(
            string uid,
            string propertyName,
            string value)
        {
            if (m_StateElementsLookup.ContainsKey(uid))
            {
                Update(uid, propertyName, value);
            }
            else
            {
                Add(uid, propertyName, value);
            }
        }

        public bool Contains(
            string uid,
            string propertyName)
        {
            return m_StateElementsLookup.ContainsKey(uid) && Contains(m_StateElementsLookup[uid], propertyName);
        }

        public string GetValue(
            string uid,
            string propertyName)
        {
            return GetValue(m_StateElementsLookup[uid], propertyName);
        }


        public void InitializeLookupTables()
        {
            foreach (TElement element in m_State.Elements)
            {
                m_StateElementsLookup.Add(element.Uid, element);
            }
        }

        public void Add(
            string uid,
            string propertyName,
            string value)
        {
            if (m_StateElementsLookup.ContainsKey(uid))
            {
                throw new InvalidOperationException($@"Property name ""{propertyName}"" is already in persisted state");
            }
            var element = new TElement
            {
                Uid = uid
            };
            var property = new TProperty
            {
                Name = propertyName,
                Value = value
            };
            element.Properties.Add(property);
            m_State.Elements.Add(element);
            m_StateElementsLookup.Add(element.Uid, element);
        }

        public void Update(
            string uid,
            string propertyName,
            string value)
        {
            if (!m_StateElementsLookup.TryGetValue(uid, out TElement? element))
            {
                throw new InvalidOperationException($@"Property name ""{propertyName}"" is not in persisted state");
            }
            if (Contains(element, propertyName))
            {
                GetProperty(element, propertyName).Value = value;
                return;
            }
            var property = new TProperty
            {
                Name = propertyName,
                Value = value
            };
            element.Properties.Add(property);
        }



        #region Private Static Methods

        private static bool Contains(
            TElement element,
            string propertyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return element.Properties.Any(x => x.Name == propertyName);
        }

        private static string GetValue(
            TElement element,
            string propertyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            TProperty property = element.Properties.FirstOrDefault(x => x.Name == propertyName);
            if (property == null)
            {
                return string.Empty;
            }
            return property.Value;
        }

        private static TProperty GetProperty(
            TElement element,
            string propertyName)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return element.Properties.FirstOrDefault(x => x.Name == propertyName);
        }

        #endregion
    }
}
