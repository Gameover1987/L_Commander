using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace L_Commander.App.Views.Controls;

/// <summary>
/// a new class is needed since protected functions can't be accessed from static extensions
/// </summary>
public class CustomDataGrid : DataGrid
{
    object downItem;
    int downIdx;
    bool leftRow;

    static CustomDataGrid()
    {
        ItemsSourceProperty.OverrideMetadata(typeof(CustomDataGrid), new FrameworkPropertyMetadata((PropertyChangedCallback)null));
    }

    public CustomDataGrid()
    {
        this.PreviewMouseDown += DataGrid_PreviewMouseDown;
        this.PreviewMouseMove += DataGrid_PreviewMouseMove;
        this.PreviewMouseUp += DataGrid_PreviewMouseUp;
        this.MouseLeave += DataGrid_MouseLeave;
    }

    /// <summary>
    /// if clicking on a checkbox, all selected rows checkboxes are updated.
    /// also notes which row is potentially being clicked, then ignores the mousedown so the selections aren't automatically updated
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void DataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        DataGrid grid = sender as DataGrid;//reference to self for extensions
        DataGridRow row = grid.FindOverRow(e);
        leftRow = false;
        downItem = null; // always clear selecteditem 
        downIdx = this.GetIndexAt(e);
        
        if (row != null && overCheckbox && SelectionMode == DataGridSelectionMode.Extended)
        {
            DataGridCell cell = grid.GetAtPoint<DataGridCell>(e);
            DataGridColumn col = cell == null ? null : cell.Column;
            object overdata = row.Item;
            CheckBox checkbox = col == null ? null : col.GetCellContent(overdata) as CheckBox;
            if (checkbox != null)
            {
                bool oldstate = checkbox.IsChecked.Value;
                bool newstate = !oldstate;
                // loop thru all the selected columns and update the ischecked state
                foreach (object selecteditem in grid.SelectedItems)
                {
                    row = grid.ItemContainerGenerator.ContainerFromItem(selecteditem) as DataGridRow;
                    if (row != null)
                    {
                        checkbox = col.GetCellContent(row) as CheckBox;
                        if (checkbox != null)
                            checkbox.IsChecked = newstate;
                    }
                }
                e.Handled = true;
            }
        }
        else if (SelectionMode == DataGridSelectionMode.Extended && row != null && SelectedItems.Contains(row.Item) && Keyboard.Modifiers == ModifierKeys.None)  //if the pointed item is already selected do not reselect it, so the previous multi-selection will remain
        {
            e.Handled = true;  // this prevents the multiselection from disappearing, BUT datagridcell still gets the event and sets DataGrid's private member _selectionAnchor
            downItem = row.Item; // store our item to select on MouseLeftButtonUp
        }
    }
    /// <summary>
    /// checks if you are reordering the current set of selections within the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void DataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        try
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (Mouse.Captured == this)
                {
                    if (!this.IsMouseInBounds(e))
                        DataGrid_MouseLeave(sender, e);
                }

                // enable selection repositioning within grid
                DataGridRow row = this.FindOverRow(e);
                this.GetAtPoint<DataGridRow>(e);
                int idx = row == null ? -1 : row.GetIndex();
                object o = this.InputHitTest(e.GetPosition(this));
                if (row != null && !row.IsEditing)
                {
                    int idxover = this.GetIndexAt(e);
                    if (idxover >= 0 && downIdx >= 0 && idxover != downIdx)
                    {
                        leftRow = true;
                        this.ShiftSelections(idxover > downIdx);
                        downIdx = idxover;
                    }
                }
            }
        }
        catch (Exception x)
        {
            //Globals.Instance.Log(x);
        }

    }
    /// <summary>
    /// starts drag/drop if you are dragging selections out of the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void DataGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
            leftRow = true;
            if (SelectedItems.Count > 0 && SelectionMode == DataGridSelectionMode.Extended)
            {
                DataObject data = new DataObject();
                object todrag = SelectedItems.Cast<object>().ToArray();
                data.SetData(todrag.GetType(), todrag);
                System.Windows.DragDrop.DoDragDrop(this, data, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }
    }
    /// <summary>
    /// updates the selection to a single row if you simply clicked on a row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void DataGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        DataGridRow row = this.FindOverRow(e);
        if (SelectionMode == DataGridSelectionMode.Extended && row != null && row.Item == downItem)  // check if it's set and concerning the same row
        {
            if (SelectedItem == downItem)
                SelectedItem = null;  // if the item is already selected whe need to trigger a change 
            SelectedItem = downItem;  // this will clear the multi selection, and only select the item we pressed down on
            typeof(DataGrid).GetField("_selectionAnchor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, new DataGridCellInfo(row.Item, ColumnFromDisplayIndex(0)));  // we need to set this anchor for when we select by pressing shift key
            downItem = null;  // handled
        }
    }
}