using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace testWPF.Helpers
{
    public static class Placeholder
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached(
                "Text",
                typeof(string),
                typeof(Placeholder),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetText(DependencyObject obj)
        {
            return (string)obj.GetValue(TextProperty);
        }

        public static void SetText(DependencyObject obj, string value)
        {
            obj.SetValue(TextProperty, value);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.Loaded += (s, _) =>
                {
                    AddAdorner(textBox);
                };
                textBox.TextChanged += (s, _) =>
                {
                    UpdateAdornerVisibility(textBox);
                };
            }
        }

        private static void AddAdorner(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            var adorners = layer.GetAdorners(textBox);
            if (adorners != null)
            {
                foreach (var adorner in adorners)
                {
                    if (adorner is PlaceholderAdorner) return;
                }
            }

            var placeholderAdorner = new PlaceholderAdorner(textBox, GetText(textBox));
            layer.Add(placeholderAdorner);
            UpdateAdornerVisibility(textBox);
        }

        private static void UpdateAdornerVisibility(TextBox textBox)
        {
            var layer = AdornerLayer.GetAdornerLayer(textBox);
            if (layer == null) return;

            var adorners = layer.GetAdorners(textBox);
            if (adorners == null) return;

            foreach (var adorner in adorners)
            {
                if (adorner is PlaceholderAdorner placeholderAdorner)
                {
                    placeholderAdorner.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }

    public class PlaceholderAdorner : Adorner
    {
        private readonly VisualCollection _visuals;
        private readonly TextBlock _textBlock;

        public PlaceholderAdorner(UIElement adornedElement, string placeholderText)
            : base(adornedElement)
        {
            _textBlock = new TextBlock
            {
                Text = placeholderText,
                Foreground = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)), // полупрозрачный белый
                FontStyle = FontStyles.Italic,
                FontSize = 16,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Montserrat"),
                Margin = new Thickness(10, 0, 0, 0), // отступ слева
                VerticalAlignment = VerticalAlignment.Center,
                IsHitTestVisible = false
            };

            var container = new Grid();
            container.Children.Add(_textBlock);
            container.VerticalAlignment = VerticalAlignment.Stretch;
            container.HorizontalAlignment = HorizontalAlignment.Stretch;

            _visuals = new VisualCollection(this)
            {
                container
            };
        }

        protected override int VisualChildrenCount => _visuals.Count;

        protected override Visual GetVisualChild(int index) => _visuals[index];

        protected override Size MeasureOverride(Size constraint)
        {
            foreach (UIElement child in _visuals)
            {
                child.Measure(constraint);
            }
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in _visuals)
            {
                child.Arrange(new Rect(finalSize));
            }
            return finalSize;
        }
    }
}

