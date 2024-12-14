using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Utilities;
using Newtonsoft.Json;
using System.Collections;
using System.Globalization;
using System.Text;

namespace Zametek.Avalonia.PropertyPersistence
{
    public class PropertyState
        : AvaloniaObject,
        IPropertyState<PersistenceState, PersistenceElement, PersistenceProperty>
    {

        private readonly Dictionary<AvaloniaProperty, object> m_PropertyValues;

        #region Ctors

        public PropertyState(AvaloniaObject element)
            : base()
        {
            m_PropertyValues = [];
            ElementMode = GetMode(element);
            ElementUid = PropertyStateExtension.GetUidWithNamespace(element);
            ElementType = element.GetType();
        }

        static PropertyState()
        {
            Persistence = new Persistence<PersistenceState, PersistenceElement, PersistenceProperty>();
        }

        #endregion

        #region Dependency Properties

        public static readonly AvaloniaProperty<string?> UidProperty =
            AvaloniaProperty.RegisterAttached<PropertyState, Interactive, string?>("Uid", string.Empty);

        public static readonly AvaloniaProperty<PropertyStateMode> ModeProperty =
            AvaloniaProperty.RegisterAttached<PropertyState, Interactive, PropertyStateMode>("Mode", PropertyStateMode.Persisted);

        public static readonly AvaloniaProperty<Control?> VisualAnchorProperty =
            AvaloniaProperty.RegisterAttached<PropertyState, Interactive, Control?>("VisualAnchor", null);

        public static readonly AvaloniaProperty<bool> IsNamespacingEnabledProperty =
           AvaloniaProperty.RegisterAttached<PropertyState, Interactive, bool>("IsNamespacingEnabled", false);

        public static void SetUid(
            AvaloniaObject element,
            string value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(UidProperty, value);
        }

        public static string? GetUid(AvaloniaObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (string?)element.GetValue(UidProperty);
        }

        public static void SetMode(
            AvaloniaObject element,
            PropertyStateMode value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(ModeProperty, value);
        }

        public static PropertyStateMode GetMode(AvaloniaObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (PropertyStateMode)element.GetValue(ModeProperty);
        }

        public static void SetVisualAnchor(
            AvaloniaObject element,
            Control value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(VisualAnchorProperty, value);
        }

        public static Control? GetVisualAnchor(AvaloniaObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (Control?)element.GetValue(VisualAnchorProperty);
        }

        public static bool GetIsNamespacingEnabled(AvaloniaObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            return (bool)item.GetValue(IsNamespacingEnabledProperty);
        }

        public static void SetIsNamespacingEnabled(
            AvaloniaObject item,
            bool value)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            item.SetValue(IsNamespacingEnabledProperty, value);
        }

        #endregion

        #region Public Properties

        public string ElementUid
        {
            get;
            protected set;
        }

        public PropertyStateMode ElementMode
        {
            get;
            protected set;
        }

        public Type ElementType
        {
            get;
            protected set;
        }

        #endregion

        #region Static Properties

        internal static Persistence<PersistenceState, PersistenceElement, PersistenceProperty> Persistence
        {
            get;
            private set;
        }

        #endregion








        public bool HasPropertyValue(AvaloniaProperty property)
        {
            return m_PropertyValues.ContainsKey(property);
        }

        public object? GetPropertyValue(AvaloniaProperty property)
        {
            if (!m_PropertyValues.TryGetValue(property, out object? value))
            {
                throw new InvalidOperationException($@"There is no value for property name ""{property.Name}""");
            }
            return value;
        }

        /// <summary>
        /// Adds a property value to the memory state if it does not already exist and
        /// adds it to the persisted state if necessary. Or, if the property value already
        /// exists in the persisted state then it is retrieved, added to the memory state
        /// and returned.
        /// </summary>
        public object AddPropertyValue(AvaloniaProperty property, object value)
        {
            if (m_PropertyValues.ContainsKey(property))
            {
                return value;
            }
            if (ElementMode == PropertyStateMode.Persisted)
            {
                if (Persistence.Contains(ElementUid, property.Name))
                {
                    string stringValue = Persistence.GetValue(ElementUid, property.Name);
                    value = Deserialize(property, stringValue);
                }
                else
                {
                    Persistence.Persist(
                        ElementUid,
                        property.Name,
                        Serialize(property, value));
                }
            }
            m_PropertyValues.Add(property, value);
            return value;
        }

        public void UpdatePropertyValue(AvaloniaProperty property, object value)
        {
            if (!m_PropertyValues.ContainsKey(property))
            {
                throw new InvalidOperationException($@"Property name ""{property.Name}"" is not in memory state");
            }
            if (ElementMode == PropertyStateMode.Persisted)
            {
                if (!Persistence.Contains(ElementUid, property.Name))
                {
                    throw new InvalidOperationException($@"Property name ""{property.Name}"" is not in persisted state");
                }
                Persistence.Persist(
                    ElementUid,
                    property.Name,
                    Serialize(property, value));
            }
            m_PropertyValues[property] = value;
        }






        internal static object ConvertFromString(Type targetType, AvaloniaProperty property, string stringValue)
        {


            // TODO


            if (TypeUtilities.TryConvert(property.PropertyType, stringValue, CultureInfo.InvariantCulture, out object? result))
            {
                return result;
            }

            return result;



            //return DependencyPropertyDescriptor
            //    .FromProperty(property, targetType)
            //    .Converter
            //    .ConvertFromString(stringValue);
        }

        //internal static string ConvertToString(Type targetType, AvaloniaProperty property, object value)
        //{
        //    return DependencyPropertyDescriptor
        //        .FromProperty(property, targetType)
        //        .Converter
        //        .ConvertToString(value);
        //}

        internal static string GetNamespace(AvaloniaObject element)
        {


            Control visualAnchor = GetVisualAnchor(element);
            if (!(element is Control frameworkElement)
                || visualAnchor != null)
            {
                frameworkElement = visualAnchor;
                if (frameworkElement != null
                    && GetIsNamespacingEnabled(element))
                {
                    return GetNamespace(frameworkElement) + GetNamespaceName(frameworkElement) + '.';
                }
            }
            if (frameworkElement != null
                && !GetIsNamespacingEnabled(element))
            {
                frameworkElement = null;
            }
            return GetNamespace(frameworkElement);
        }





        private static string GetNamespace(Control element)
        {
            if (element == null)
            {
                return string.Empty;
            }
            var stringBuilder = new StringBuilder();
            Control logicalAncestor = element.FindLogicalAncestorOfType<Control>();
            while (logicalAncestor != null)
            {
                stringBuilder.Insert(0, '.');
                stringBuilder.Insert(0, GetNamespaceName(logicalAncestor));
                logicalAncestor = logicalAncestor.FindLogicalAncestorOfType<Control>();
            }
            return stringBuilder.ToString();
        }

        private static string GetNamespaceName(Control element)
        {
             string name = GetUid(element);
            if (string.IsNullOrEmpty(name))
            {
                name = element.GetType().Name;
            }
            return name;
        }









        public string Serialize(AvaloniaProperty property, object value)
        {



            //Type valueType = DependencyPropertyDescriptor.FromProperty(property, Type).PropertyType;
            Type valueType = property.PropertyType;
            if (valueType.IsAssignableFrom(typeof(IEnumerable)))
            {
                valueType = typeof(List<object>);
            }
            return JsonConvert.SerializeObject(value, valueType, null);
        }

        public object Deserialize(AvaloniaProperty property, string stringValue)
        {
            //Type valueType = DependencyPropertyDescriptor.FromProperty(property, Type).PropertyType;

            Type valueType = property.PropertyType;
            if (valueType.IsAssignableFrom(typeof(IEnumerable)))
            {
                valueType = typeof(List<object>);
            }
            return JsonConvert.DeserializeObject(stringValue, valueType);
        }

    }
}
