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

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="TChildType">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static TChildType FindChild<TChildType>(DependencyObject parent, string childName = null)
            where TChildType : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            TChildType foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child

                if (!(child is TChildType))
                {
                    // recursively drill down the tree
                    foundChild = FindChild<TChildType>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (TChildType)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (TChildType)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
