using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Shared
{
    public static class Globalization
    {
        private static Data _data;
        private static int _selectedLanguage;

        public struct Data
        {
            public bool hasData;
            public string[] languages;
            public string[] cultures;
            public Dictionary<string, string[]> translations;
        }
        
        public static void SetText(Data data)
        {
            _data = data;
        }

        public static void SetSelectedLanguage(int index)
        {
            if (index >= 0 && index < _data.languages.Length)
            {
                _selectedLanguage = index;
            }

            if (index >= 0 && index < _data.cultures.Length)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(_data.cultures[index]);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(_data.cultures[index]);
            }
        }

        public static string[] GetAvailableLanguages()
        {
            return _data.languages;
        }

        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).Loaded -= OnElementLoaded;
            ApplyTranslations(sender as DependencyObject);
        }

        public static void ApplyTranslations(DependencyObject target)
        {
            var element = target as UIElement;

            if (element == null)
                return;

            if (!string.IsNullOrEmpty(element.Uid))
            {
                if (element is TextBlock)
                {
                    ((TextBlock)element).Text = Translate(element.Uid);
                }
                else if (element is TextBox)
                {
                    ((TextBox)element).Text = Translate(element.Uid);
                }
                else if (element is GroupBox)
                {
                    ((GroupBox)element).Header = Translate(element.Uid);
                }
                else if (element is Window)
                {
                    ((Window)element).Title = Translate(element.Uid);
                }
                else if (element is ContentControl)
                {
                    ((ContentControl)element).Content = Translate(element.Uid);
                }
                else if (element is MenuItem)
                {
                    ((MenuItem)element).Header = Translate(element.Uid);
                }
            }

            if (element is FrameworkElement && ((FrameworkElement)element).ContextMenu != null)
            {
                ApplyTranslations(((FrameworkElement)element).ContextMenu);
            }

            if (element is ComboBox)
            {
                foreach (var child in ((ComboBox) element).Items)
                {
                    ApplyTranslations(child as UIElement);
                }
            }

            int childCount = VisualTreeHelper.GetChildrenCount(target);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(target, i) as UIElement;
                ApplyTranslations(child);
            }
        }

        private static void E_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as FrameworkElement).IsVisibleChanged -= E_IsVisibleChanged;
            ApplyTranslations(sender as DependencyObject);
        }

        public static void ApplyTranslations(ContextMenu menu)
        {
            foreach (var item in menu.Items)
            {
                if (item is DependencyObject)
                {
                    ApplyTranslations(item as DependencyObject);
                }
            }
        }

        public static string Translate(string key)
        {
            if (_data.hasData && _data.translations.TryGetValue(key, out string[] translations))
            {
                string result = translations[_selectedLanguage];

                // Default to English if translation is missing
                if (string.IsNullOrEmpty(result))
                {
                    return translations[0];
                }

                return result;
            }

            return key;
        }

        public static string TranslateFormat(string key, params object[] args)
        {
            return string.Format(Translate(key), args);
        }
    }
}
