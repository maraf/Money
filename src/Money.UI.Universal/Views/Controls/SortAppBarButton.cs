using Money.Models.Sorting;
using Money.ViewModels;
using Neptuo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Money.Views.Controls
{
    public class SortAppBarButton : AppBarButton
    {
        private readonly Dictionary<object, SortDirection> defaultSortDirection = new Dictionary<object, SortDirection>();

        private Type parameterType;
        private MethodInfo updateMethod;
        private PropertyInfo typeProperty;
        private PropertyInfo directionProperty;

        private bool isSortDescriptorUpdate = false;

        public object SortDescriptor
        {
            get { return (object)GetValue(SortDescriptorProperty); }
            set { SetValue(SortDescriptorProperty, value); }
        }

        public static readonly DependencyProperty SortDescriptorProperty = DependencyProperty.Register(
            "SortDescriptor",
            typeof(object),
            typeof(SortAppBarButton),
            new PropertyMetadata(null, OnSortDescriptorChanged)
        );

        private static void OnSortDescriptorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SortAppBarButton control = (SortAppBarButton)d;
            control.OnSortDescriptorChanged(e.NewValue);
        }

        private void OnSortDescriptorChanged(object newValue)
        {
            if (isSortDescriptorUpdate)
                return;

            if (newValue == null)
            {
                // TODO: Something...
                return;
            }

            TypeInfo type = newValue.GetType().GetTypeInfo();
            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(SortDescriptor<>))
                throw Ensure.Exception.ArgumentOutOfRange("SortDescriptor", $"A value passed to the '{nameof(SortDescriptor)}' must be of type 'SortDescriptor<>'.");

            Type newParameterType = type.GenericTypeArguments.First();
            if (parameterType != newParameterType)
                BuildParameterType(newParameterType);

            UpdateIcon();
        }

        private void BuildParameterType(Type newParameterType)
        {
            parameterType = newParameterType;
            if (!parameterType.GetTypeInfo().IsEnum)
                throw Ensure.Exception.ArgumentOutOfRange("SortDescriptor", $"A generic parameter type must an enum.");

            updateMethod = typeof(SortDescriptorExtensions)
                .GetMethod(nameof(SortDescriptorExtensions.Update), BindingFlags.Static | BindingFlags.Public)
                .MakeGenericMethod(parameterType);

            typeProperty = typeof(SortDescriptor<>)
                .MakeGenericType(newParameterType)
                .GetProperty(nameof(SortDescriptor<SortDirection>.Type));

            directionProperty = typeof(SortDescriptor<>)
                .MakeGenericType(newParameterType)
                .GetProperty(nameof(SortDescriptor<SortDirection>.Direction));

            defaultSortDirection.Clear();

            MenuFlyout flyout = new MenuFlyout();
            foreach (object value in Enum.GetValues(parameterType))
            {
                string text = Enum.GetName(parameterType, value);

                MemberInfo itemInfo = parameterType
                    .GetMember(text)
                    .First();

                DescriptionAttribute attribute = itemInfo.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                    text = attribute.Description;

                DefaultValueAttribute defaultValue = itemInfo.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultValue != null)
                    defaultSortDirection[value] = (SortDirection)defaultValue.Value;

                MenuFlyoutItem item = new MenuFlyoutItem()
                {
                    Text = text,
                    Tag = value
                };

                item.Click += OnItemClick;
                flyout.Items.Add(item);
            }

            Flyout = flyout;
        }

        private void UpdateIcon()
        {
            object value = typeProperty.GetValue(SortDescriptor);
            SortDirection direction = (SortDirection)directionProperty.GetValue(SortDescriptor);

            foreach (MenuFlyoutItem item in ((MenuFlyout)Flyout).Items)
            {
                if (item.Tag.Equals(value))
                    item.Icon = new FontIcon() { Glyph = direction == SortDirection.Ascending ? "\uE74A" : "\uE74B" };
                else
                    item.Icon = null;
            }
        }

        private SortDirection GetSortDirection(object value)
        {
            if (!defaultSortDirection.TryGetValue(value, out SortDirection direction))
                direction = SortDirection.Ascending;

            return direction;
        }

        private void OnItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                isSortDescriptorUpdate = true;

                MenuFlyoutItem item = (MenuFlyoutItem)sender;
                object value = item.Tag;

                SortDirection direction = GetSortDirection(value);
                SortDescriptor = updateMethod.Invoke(null, new[] { SortDescriptor, value, direction });
                UpdateIcon();
            }
            finally
            {
                isSortDescriptorUpdate = false;
            }
        }

        public SortAppBarButton()
        {
            Icon = new SymbolIcon(Symbol.Sort);
            Label = "Sort";
        }
    }
}
