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
    private object _downItem;
    private int _downIdx;
    private bool _leftRow;

    static CustomDataGrid()
    {
        ItemsSourceProperty.OverrideMetadata(typeof(CustomDataGrid), new FrameworkPropertyMetadata((PropertyChangedCallback)null));
    }

    public CustomDataGrid()
    {
        PreviewMouseDown += DataGrid_PreviewMouseDown;
        PreviewMouseMove += DataGrid_PreviewMouseMove;
        PreviewMouseUp += DataGrid_PreviewMouseUp;
        MouseLeave += DataGrid_MouseLeave;
    }

    /// <summary>
    /// if clicking on a checkbox, all selected rows checkboxes are updated.
    /// also notes which row is potentially being clicked, then ignores the mousedown so the selections aren't automatically updated
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var grid = sender as DataGrid; //reference to self for extensions
        var row = grid.FindOverRow(e);

        _downItem = null; // always clear selecteditem 

        if (SelectionMode == DataGridSelectionMode.Extended && 
            row != null && SelectedItems.Contains(row.Item) && 
            Keyboard.Modifiers == ModifierKeys.None &&
            e.ClickCount != 2)  //if the pointed item is already selected do not reselect it, so the previous multi-selection will remain
        {
            e.Handled = true;  // this prevents the multiselection from disappearing, BUT datagridcell still gets the event and sets DataGrid's private member _selectionAnchor
            _downItem = row.Item; // store our item to select on MouseLeftButtonUp
        }
    }
    /// <summary>
    /// checks if you are reordering the current set of selections within the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.LeftButton != MouseButtonState.Pressed)
            return;

        if (Mouse.Captured == this)
        {
            if (!this.IsMouseInBounds(e))
                DataGrid_MouseLeave(sender, e);
        }

        // enable selection repositioning within grid
        var row = this.FindOverRow(e);
        this.GetAtPoint<DataGridRow>(e);
        int idx = row == null ? -1 : row.GetIndex();
        object o = InputHitTest(e.GetPosition(this));
        if (row != null && !row.IsEditing)
        {
            int idxover = this.GetIndexAt(e);
            if (idxover >= 0 && _downIdx >= 0 && idxover != _downIdx)
            {
                _leftRow = true;
                this.ShiftSelections(idxover > _downIdx);
                _downIdx = idxover;
            }
        }

    }
    /// <summary>
    /// starts drag/drop if you are dragging selections out of the grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_MouseLeave(object sender, MouseEventArgs e)
    {
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
            _leftRow = true;
            if (SelectedItems.Count > 0 && SelectionMode == DataGridSelectionMode.Extended)
            {
                DataObject data = new DataObject();
                object todrag = SelectedItems.Cast<object>().ToArray();
                data.SetData(todrag.GetType(), todrag);
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }
    }
    /// <summary>
    /// updates the selection to a single row if you simply clicked on a row
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        DataGridRow row = this.FindOverRow(e);
        if (SelectionMode == DataGridSelectionMode.Extended && row != null && row.Item == _downItem)  // check if it's set and concerning the same row
        {
            if (SelectedItem == _downItem)
                SelectedItem = null;  // if the item is already selected whe need to trigger a change 
            SelectedItem = _downItem;  // this will clear the multi selection, and only select the item we pressed down on
            typeof(DataGrid).GetField("_selectionAnchor", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, new DataGridCellInfo(row.Item, ColumnFromDisplayIndex(0)));  // we need to set this anchor for when we select by pressing shift key
            _downItem = null;  // handled
        }
    }
}