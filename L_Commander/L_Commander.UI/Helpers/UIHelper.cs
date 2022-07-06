using System.Windows;
using System.Windows.Media;

namespace L_Commander.UI.Helpers
{
    public static class UIHelper
    {
        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject
        {
            var current = dependencyObject;

            while (current != null)
            {
                var tabItem = current as T;
                if (tabItem != null)
                {
                    return tabItem;
                }

                current = VisualTreeHelper.GetParent(current);
            }

            return null;
        }
    }
}
