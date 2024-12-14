using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Globalization;

namespace Zametek.Avalonia.PropertyPersistence
{
    public class PropertyStateExtension
        : MarkupExtension
    {
        private static readonly Dictionary<string, PropertyState> s_PropertyStates =
            new Dictionary<string, PropertyState>();

        private static readonly AvaloniaProperty<Dictionary<string, EventHandler<RoutedEventArgs>>> s_LoadedHandlersProperty =
        AvaloniaProperty.RegisterAttached<PropertyState, Interactive, Dictionary<string, EventHandler<RoutedEventArgs>>>("LoadedHandlers", null);

        private static Func<AvaloniaObject, PropertyState> s_AddPropertyState;

        public object? Default
        {
            get;
            set;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget provideValueTarget)
            {
                return this;
            }

            if (serviceProvider.GetService(typeof(IRootObjectProvider)) is not IRootObjectProvider rootObjectProvider)
            {
                return this;
            }

            AvaloniaObject? targetObject = provideValueTarget.TargetObject as AvaloniaObject
                ?? rootObjectProvider.IntermediateRootObject as AvaloniaObject;

            return ProvideValue(
               targetObject,
               provideValueTarget.TargetProperty as AvaloniaProperty,
               Default) ?? this;
        }

        internal static object ProvideValue(
            AvaloniaObject target,
            AvaloniaProperty property,
            object defaultValue)
        {
            if (target == null
                || property == null)
            {
                return null;
            }
            object outputValue = defaultValue;
            if (outputValue == null
                || string.IsNullOrEmpty(outputValue.ToString()))
            {
                throw new InvalidOperationException($@"No default value provided for property ""{target}.{property.Name}""");
            }
            if (!outputValue.GetType().IsSerializable)
            {
                throw new InvalidOperationException($@"Default value provided for property ""{target}.{property.Name}"" is not serializable");
            }

            Control visualAnchor = PropertyState.GetVisualAnchor(target);

            if (!(target is Control element) || visualAnchor != null)
            {
                element = visualAnchor;
            }
            var defaultString = outputValue as string;
            if (!string.IsNullOrEmpty(defaultString))
            {
                outputValue = PropertyState.ConvertFromString(target.GetType(), property, defaultString);
            }

            void handler(object s, RoutedEventArgs e)
            {
                if (element != null)
                {
                    element.Loaded -= handler;
                }

                if (!HasPropertyValue(target, property))
                {
                    object value = AddPropertyValue(target, property, outputValue);
                    if (value == null)
                    {
                        throw new InvalidOperationException($@"The element ""{target}"" has no unique identifier for property persistence");
                    }
                }

                var binding = CreateStateBinding(target, property);
                target.Bind(property, binding);
            }

            // Windows are already loaded when the application starts
            // so need to invoke their loaded handlers manually.
            if (target is Window)
            {
                handler(null, null);
            }
            // Keep track of the loaded handlers in case they need to
            // activated manually later.
            AddPropertyLoadedHandler(target, property, handler);
            if (element != null)
            {
                element.Loaded += handler;
            }
            if (HasPropertyValue(target, property))
            {
                return GetPropertyValue(target, property);
            }
            return outputValue;
        }
        public static void Load(
            IStateResourceAccess<PersistenceState> stateResourceAccess,
            Func<AvaloniaObject, PropertyState> addPropertyState)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException(nameof(stateResourceAccess));
            }
            PropertyState.Persistence.Load(stateResourceAccess);
            s_AddPropertyState = addPropertyState ?? throw new ArgumentNullException(nameof(addPropertyState));
        }

        public static void Save(IStateResourceAccess<PersistenceState> stateResourceAccess)
        {
            if (stateResourceAccess == null)
            {
                throw new ArgumentNullException(nameof(stateResourceAccess));
            }
            PropertyState.Persistence.Save(stateResourceAccess);
        }

        internal static string GetUidWithNamespace(AvaloniaObject element)
        {
            return $@"{PropertyState.GetNamespace(element)}{PropertyState.GetUid(element)}";
        }

        private static bool HasPropertyValue(
            AvaloniaObject element,
            AvaloniaProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            if (string.IsNullOrEmpty(uidWithNamespace)
               || !s_PropertyStates.TryGetValue(uidWithNamespace, out PropertyState state))
            {
                return false;
            }
            return state.HasPropertyValue(property);
        }

        private static object GetPropertyValue(
            AvaloniaObject element,
            AvaloniaProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            if (string.IsNullOrEmpty(uidWithNamespace)
               || !s_PropertyStates.TryGetValue(uidWithNamespace, out PropertyState state))
            {
                throw new InvalidOperationException($@"The property ""{element}.{property.Name}"" is not in state");
            }
            else if (!state.HasPropertyValue(property))
            {
                throw new InvalidOperationException($@"The property ""{element}.{property.Name}"" has no value");
            }
            return state.GetPropertyValue(property);
        }

        private static object AddPropertyValue(
            AvaloniaObject element,
            AvaloniaProperty property,
            object value)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            if (string.IsNullOrEmpty(uidWithNamespace))
            {
                return null;
            }
            if (!s_PropertyStates.TryGetValue(uidWithNamespace, out PropertyState state))
            {
                state = s_AddPropertyState(element);
                s_PropertyStates.Add(uidWithNamespace, state);
            }
            return state.AddPropertyValue(property, value);
        }

        private static void UpdatePropertyValue(
            AvaloniaObject element,
            AvaloniaProperty property,
            object value)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            if (string.IsNullOrEmpty(uidWithNamespace)
               || !s_PropertyStates.TryGetValue(uidWithNamespace, out PropertyState state))
            {
                return;
            }
            state.UpdatePropertyValue(property, value);
        }

        private static void AddPropertyLoadedHandler(
            AvaloniaObject element,
            AvaloniaProperty property,
            EventHandler<RoutedEventArgs> value)
        {
            Dictionary<string, EventHandler<RoutedEventArgs>> dictionary = GetPrivateLoadedHandlers(element);
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, EventHandler<RoutedEventArgs>>();
                SetPrivateLoadedHandlers(element, dictionary);
            }
            dictionary.Add(property.Name, value);
        }

        private static bool IsPropertyPersisted(
            AvaloniaObject element,
            AvaloniaProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            return PropertyState.Persistence.Contains(uidWithNamespace, property.Name);
        }

        private static void SetPrivateLoadedHandlers(
            AvaloniaObject element,
            Dictionary<string, EventHandler<RoutedEventArgs>> value)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            element.SetValue(s_LoadedHandlersProperty, value);
        }

        private static Dictionary<string, EventHandler<RoutedEventArgs>> GetPrivateLoadedHandlers(AvaloniaObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            return (Dictionary<string, EventHandler<RoutedEventArgs>>)element.GetValue(s_LoadedHandlersProperty);
        }

        private static BindingBase CreateStateBinding(
            AvaloniaObject element,
            AvaloniaProperty property)
        {
            string uidWithNamespace = GetUidWithNamespace(element);
            var output = new Binding()
            {
                Mode = BindingMode.TwoWay,
                Converter = new PropertyValueConverter()
                {
                    Target = element,
                    Property = property,
                },
                Source = s_PropertyStates,
                // TODO
                Path = $@"[{uidWithNamespace}]",
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            return output;
        }

        private class PropertyValueConverter
           : IValueConverter
        {
            public AvaloniaObject Target
            {
                get;
                set;
            }

            public AvaloniaProperty Property
            {
                get;
                set;
            }

            public object Convert(
                object value,
                Type targetType,
                object parameter,
                CultureInfo culture)
            {
                if (HasPropertyValue(Target, Property))
                {
                    return GetPropertyValue(Target, Property);
                }

                // TODO
                Target.ClearValue(Property);
                //BindingOperations.ClearBinding(Target, Property);
                return BindingOperations.DoNothing;
            }

            public object ConvertBack(
                object value,
                Type targetType,
                object parameter,
                CultureInfo culture)
            {
                UpdatePropertyValue(Target, Property, value);
                return BindingOperations.DoNothing;
            }
        }
    }
}
