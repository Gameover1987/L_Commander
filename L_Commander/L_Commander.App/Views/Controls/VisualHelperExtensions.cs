using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace L_Commander.App.Views.Controls
{
    internal static class visualhelperextensions
    {
        public static T GetVisualChild<T>(this Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        public static bool IsMouseInBounds(this FrameworkElement fe, MouseEventArgs e)
        {
            Rect bounds = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight);
            return bounds.Contains(e.GetPosition(fe));
        }
        public static void ShiftSelections(this DataGrid grid, bool down)
        {
            int sel;
            int idx;

            List<int> selections = grid.SelectedRowIndexes();
            IList data = grid.DataList();
            if (data != null && selections.Count > 0)
                if (down)
                {
                    sel = selections.Last();
                    idx = sel + 1;
                    if (data != null && idx >= 0 && idx < data.Count)
                    {
                        while (idx >= 0)
                        {
                            idx = data.ShiftUnselectedRowUp(selections, idx) - 1;
                            while (idx > 0 && !selections.Contains(idx - 1))
                                idx--;
                        }
                    }
                }
                else
                {
                    sel = selections.First();
                    if (data != null && sel > 0)
                    {
                        while (sel < data.Count)
                        {
                            sel = data.ShiftUnselectedRowDown(selections, sel - 1) + 1;
                            while (sel < data.Count && !selections.Contains(sel))
                                sel++;
                        }
                    }
                }
        }
        static int ShiftUnselectedRowUp(this IList data, List<int> selections, int idx)
        {
            int prev = selections.PrevInsertionIndex(idx);
            if (prev >= 0 && prev != idx)
            {

                object row = data[idx];
                data.RemoveAt(idx);
                data.Insert(prev, row);
            }
            return prev;
        }
        static int ShiftUnselectedRowDown(this IList data, List<int> selections, int idx)
        {
            int next = selections.NextInsertionIndex(idx);
            if (next >= 0 && next < data.Count && next != idx && idx >= 0 && idx < data.Count)
            {
                object row = data[idx];
                data.RemoveAt(idx);
                data.Insert(next, row);
            }
            return next;
        }
        static int PrevInsertionIndex(this List<int> selections, int idx)
        {
            int ret = -1;
            if (selections != null && idx > 0)
            {
                idx--;
                ret = idx;
                while (idx >= 0 && selections.Contains(idx))
                {
                    ret = idx;
                    idx--;
                }
            }
            return ret;
        }
        static int NextInsertionIndex(this List<int> selections, int idx)
        {
            if (selections == null)
                return -1;

            if (selections != null)
                while (selections.Contains(idx + 1))
                    idx++;

            return idx;
        }
        static List<int> SelectedRowIndexes(this DataGrid grid)
        {
            List<int> ret = new List<int>();
            IList data = grid.DataList();
            if (grid != null)
                foreach (object o in grid.SelectedItems)
                {
                    int idx = grid.GetIndexOf(o);
                    if (idx >= 0)
                        ret.Add(idx);
                }
            ret.Sort();
            return ret;
        }
        public static IList DataList(this DataGrid grid)
        {
            return grid == null ? null : grid.ItemsSource as IList;
        }
        public static int GetIndexOf(this DataGrid grid, object data)
        {
            IList list = grid.DataList();
            return list == null || data == null ? -1 : list.IndexOf(data);
        }
        public static int GetIndexAt(this DataGrid grid, MouseEventArgs e)
        {
            object over = grid.FindOverItem(e);
            return grid.GetIndexOf(over);
        }
        public static object FindOverItem(this DataGrid grid, MouseEventArgs e)
        {
            DataGridRow row = grid.FindOverRow(e);
            int idx = row == null ? -1 : row.GetIndex();
            return row == null || row.IsEditing ? null : row.Item;
        }
        public static DataGridRow FindOverRow(this DataGrid grid, MouseEventArgs e)
        {
            return grid.GetAtPoint<DataGridRow>(e);
        }
        public static bool IsOver<T>(this DataGrid grid, MouseEventArgs e) where T : DependencyObject
        {
            return grid.GetAtPoint<T>(e) != null;
        }
        public static T GetAtPoint<T>(this UIElement reference, MouseEventArgs e) where T : DependencyObject
        {
            return reference.GetAtPoint<T>(e.GetPosition(reference));
        }
        public static T GetAtPoint<T>(this UIElement reference, Point point) where T : DependencyObject
        {
            DependencyObject element = reference == null ? null : reference.InputHitTest(point) as DependencyObject;
            if (element == null)
                return null;
            else if (element is T)
                return (T)element;
            else
                return element.TryFindParent<T>();
        }
        public static T TryFindParent<T>(this DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = child.GetParentObject();

            //we've reached the end of the tree
            if (parentObject == null)
                return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                //use recursion to proceed with next level
                return parentObject.TryFindParent<T>();
        }
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null) return null;

            //handle content elements separately
            ContentElement contentElement = child as ContentElement;
            if (contentElement != null)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                    return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            //also try searching for parent in framework elements (such as DockPanel, etc)
            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null)
                    return parent;
            }

            //if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }
        public static List<T> FindLogicalChildren<T>(this DependencyObject obj) where T : DependencyObject
        {
            List<T> children = new List<T>();
            foreach (var child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child != null)
                {
                    if (child is T)
                        children.Add((T)child);

                    if (child is DependencyObject)
                        children.AddRange((child as DependencyObject).FindLogicalChildren<T>()); // recursive
                }
            }
            return children;
        }
        /// <summary>
        /// Gets the list of routed event handlers subscribed to the specified routed event.
        /// </summary>
        /// <param name="element">The UI element on which the event is defined.</param>
        /// <param name="routedEvent">The routed event for which to retrieve the event handlers.</param>
        /// <returns>The list of subscribed routed event handlers.</returns>
        public static RoutedEventHandlerInfo[] GetRoutedEventHandlers(this UIElement element, RoutedEvent routedEvent)
        {
            var routedEventHandlers = default(RoutedEventHandlerInfo[]);
            // Get the EventHandlersStore instance which holds event handlers for the specified element.
            // The EventHandlersStore class is declared as internal.
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty("EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);

            if (eventHandlersStore != null)
            {
                // Invoke the GetRoutedEventHandlers method on the EventHandlersStore instance 
                // for getting an array of the subscribed event handlers.
                var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod("GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(eventHandlersStore, new object[] { routedEvent });
            }
            return routedEventHandlers;
        }
    }
}
