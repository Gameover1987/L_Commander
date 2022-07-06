using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace L_Commander.UI.Helpers
{
    public static class TextBlockToolTipHelper
    {
        public static readonly DependencyProperty ShowTooltipWhenTrimmingProperty = DependencyProperty.RegisterAttached(
                "ShowTooltipWhenTrimming", typeof(bool), typeof(TextBlockToolTipHelper), new PropertyMetadata(ShowTooltipWhenTrimmingPropertyChangedCallBack));

        public static void SetShowTooltipWhenTrimming(TextBlock textBlock, bool value)
        {
            textBlock.SetValue(ShowTooltipWhenTrimmingProperty, value);
        }

        public static bool GetShowTooltipWhenTrimming(TextBlock textBlock)
        {
            return (bool)textBlock.GetValue(ShowTooltipWhenTrimmingProperty);
        }

        public static readonly DependencyProperty ShowTooltipWhenWrappingProperty = DependencyProperty.RegisterAttached(
            "ShowTooltipWhenWrapping", typeof(bool), typeof(TextBlockToolTipHelper), new PropertyMetadata(default(bool)));

        public static void SetShowTooltipWhenWrapping(TextBlock textBlock, bool value)
        {
            textBlock.SetValue(ShowTooltipWhenWrappingProperty, value);
        }

        public static bool GetShowTooltipWhenWrapping(TextBlock textBlock)
        {
            return (bool)textBlock.GetValue(ShowTooltipWhenWrappingProperty);
        }

        public static readonly DependencyProperty ToolTipTextProperty = DependencyProperty.RegisterAttached(
            "ToolTipText", typeof(string), typeof(TextBlockToolTipHelper), new FrameworkPropertyMetadata
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        public static void SetToolTipText(TextBlock textBlock, string value)
        {
            textBlock.SetValue(ToolTipTextProperty, value);
        }

        public static string GetToolTipText(TextBlock textBlock)
        {
            return (string)textBlock.GetValue(ToolTipTextProperty);
        }

        private static void ShowTooltipWhenTrimmingPropertyChangedCallBack(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            var textBlock = target as TextBlock;
            if (textBlock == null)
                return;

            if (textBlock.TextTrimming == TextTrimming.None)
                return;

            var showTooltipWhenTrimming = (bool)e.NewValue;
            if (showTooltipWhenTrimming)
            {
                textBlock.SizeChanged += TextBlockOnSizeChanged;
                var textProperty = TypeDescriptor.GetProperties(typeof(TextBlock))["Text"];
                textProperty.AddValueChanged(textBlock, TextBlockOnTextChanged);
                textBlock.Unloaded += TextBlockOnUnloaded;
            }
        }

        private static void TextBlockOnTextChanged(object sender, EventArgs eventArgs)
        {
            EnableOrDisableToolTip((TextBlock)sender);
        }

        private static void TextBlockOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            EnableOrDisableToolTip((TextBlock)sender);
        }

        private static void TextBlockOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var textBlock = (TextBlock)sender;
            textBlock.SizeChanged -= TextBlockOnSizeChanged;

            var textProperty = TypeDescriptor.GetProperties(typeof(TextBlock))["Text"];
            textProperty.RemoveValueChanged(textBlock, TextBlockOnTextChanged);

            textBlock.Unloaded -= TextBlockOnUnloaded;
        }

        private static void EnableOrDisableToolTip(TextBlock textBlock)
        {
            var toolTipText = (string)textBlock.GetValue(ToolTipTextProperty);
            if (string.IsNullOrEmpty(toolTipText))
                toolTipText = textBlock.Text;

            var textWidth = GetTextWidth(textBlock);
            if (textWidth > textBlock.ActualWidth + 1 || GetShowTooltipWhenWrapping(textBlock))
            {
                textBlock.ToolTip = toolTipText;
            }
            else
            {
                textBlock.ToolTip = null;
            }
        }

        private static double GetTextWidth(TextBlock textBlock)
        {
            var formattedText = new FormattedText(
                    textBlock.Text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                    textBlock.FontSize,
                    textBlock.Foreground);
            return formattedText.Width;
        }
    }
}
