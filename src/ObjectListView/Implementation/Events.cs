/*
 * Events - All the events that can be triggered within an ObjectListView.
 *
 * Author: Phillip Piper
 * Date: 17/10/2008 9:15 PM
 *
 * Change log:
 * v2.8.0
 * 2014-05-20   JPP  - Added IsHyperlinkEventArgs.IsHyperlink 
 * v2.6
 * 2012-04-17   JPP  - Added group state change and group expansion events
 * v2.5
 * 2010-08-08   JPP  - CellEdit validation and finish events now have NewValue property.
 * v2.4
 * 2010-03-04   JPP  - Added filtering events
 * v2.3
 * 2009-08-16   JPP  - Added group events
 * 2009-08-08   JPP  - Added HotItem event
 * 2009-07-24   JPP  - Added Hyperlink events
 *                   - Added Formatting events
 * v2.2.1
 * 2009-06-13   JPP  - Added Cell events
 *                   - Moved all event parameter blocks to this file.
 *                   - Added Handled property to AfterSearchEventArgs
 * v2.2
 * 2009-06-01   JPP  - Added ColumnToGroupBy and GroupByOrder to sorting events
                     - Gave all event descriptions
 * 2009-04-23   JPP  - Added drag drop events
 * v2.1
 * 2009-01-18   JPP  - Moved SelectionChanged event to this file
 * v2.0
 * 2008-12-06   JPP  - Added searching events
 * 2008-12-01   JPP  - Added secondary sort information to Before/AfterSorting events
 * 2008-10-17   JPP  - Separated from ObjectListView.cs
 * 
 * Copyright (C) 2006-2014 Phillip Piper
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * If you wish to use this code in a closed source application, please contact phillip.piper@gmail.com.
 */

using System.Collections;
using System.ComponentModel;

namespace BrightIdeasSoftware;

/// <summary>
/// The callbacks for CellEditing events
/// </summary>
/// <remarks> this 
/// We could replace this with EventHandler&lt;CellEditEventArgs&gt; but that would break all
/// cell editing event code from v1.x.
/// </remarks>
public delegate void CellEditEventHandler(object sender, CellEditEventArgs e);

public partial class ObjectListView
{
	//-----------------------------------------------------------------------------------
	#region Events

	/// <summary>
	/// Triggered after a ObjectListView has been searched by the user typing into the list
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered after the control has done a search-by-typing action.")]
	public event EventHandler<AfterSearchingEventArgs> AfterSearching;

	/// <summary>
	/// Triggered after a ObjectListView has been sorted
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered after the items in the list have been sorted.")]
	public event EventHandler<AfterSortingEventArgs> AfterSorting;

	/// <summary>
	/// Triggered before a ObjectListView is searched by the user typing into the list
	/// </summary>
	/// <remarks>
	/// Set Cancelled to true to prevent the searching from taking place.
	/// Changing StringToFind or StartSearchFrom will change the subsequent search.
	/// </remarks>
	[Category("ObjectListView"),
	Description("This event is triggered before the control does a search-by-typing action.")]
	public event EventHandler<BeforeSearchingEventArgs> BeforeSearching;

	/// <summary>
	/// Triggered before a ObjectListView is sorted
	/// </summary>
	/// <remarks>
	/// Set Cancelled to true to prevent the sort from taking place.
	/// Changing ColumnToSort or SortOrder will change the subsequent sort.
	/// </remarks>
	[Category("ObjectListView"),
	Description("This event is triggered before the items in the list are sorted.")]
	public event EventHandler<BeforeSortingEventArgs> BeforeSorting;

	/// <summary>
	/// Triggered after a ObjectListView has created groups
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered after the groups are created.")]
	public event EventHandler<CreateGroupsEventArgs> AfterCreatingGroups;

	/// <summary>
	/// Triggered before a ObjectListView begins to create groups
	/// </summary>
	/// <remarks>
	/// Set Groups to prevent the default group creation process
	/// </remarks>
	[Category("ObjectListView"),
	Description("This event is triggered before the groups are created.")]
	public event EventHandler<CreateGroupsEventArgs> BeforeCreatingGroups;

	/// <summary>
	/// Triggered just before a ObjectListView creates groups
	/// </summary>
	/// <remarks>
	/// You can make changes to the groups, which have been created, before those
	/// groups are created within the listview.
	/// </remarks>
	[Category("ObjectListView"),
	Description("This event is triggered when the groups are just about to be created.")]
	public event EventHandler<CreateGroupsEventArgs> AboutToCreateGroups;

	/// <summary>
	/// Triggered when a button in a cell is left clicked.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the user left clicks a button.")]
	public event EventHandler<CellClickEventArgs> ButtonClick;

	/// <summary>
	/// This event is triggered when the user moves a drag over an ObjectListView that
	/// has a SimpleDropSink installed as the drop handler.
	/// </summary>
	/// <remarks>
	/// Handlers for this event should set the Effect argument and optionally the
	/// InfoMsg property. They can also change any of the DropTarget* setttings to change
	/// the target of the drop.
	/// </remarks>
	[Category("ObjectListView"),
	Description("Can the user drop the currently dragged items at the current mouse location?")]
	public event EventHandler<OlvDropEventArgs> CanDrop;

	/// <summary>
	/// Triggered when a cell has finished being edited.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered cell edit operation has completely finished")]
	public event CellEditEventHandler CellEditFinished;

	/// <summary>
	/// Triggered when a cell is about to finish being edited.
	/// </summary>
	/// <remarks>If Cancel is already true, the user is cancelling the edit operation.
	/// Set Cancel to true to prevent the value from the cell being written into the model.
	/// You cannot prevent the editing from finishing within this event -- you need
	/// the CellEditValidating event for that.</remarks>
	[Category("ObjectListView"),
	Description("This event is triggered cell edit operation is finishing.")]
	public event CellEditEventHandler CellEditFinishing;

	/// <summary>
	/// Triggered when a cell is about to be edited.
	/// </summary>
	/// <remarks>Set Cancel to true to prevent the cell being edited.
	/// You can change the the Control to be something completely different.</remarks>
	[Category("ObjectListView"),
	Description("This event is triggered when cell edit is about to begin.")]
	public event CellEditEventHandler CellEditStarting;

	/// <summary>
	/// Triggered when a cell editor needs to be validated
	/// </summary>
	/// <remarks>
	/// If this event is cancelled, focus will remain on the cell editor.
	/// </remarks>
	[Category("ObjectListView"),
	Description("This event is triggered when a cell editor is about to lose focus and its new contents need to be validated.")]
	public event CellEditEventHandler CellEditValidating;

	/// <summary>
	/// Triggered when a cell is left clicked.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the user left clicks a cell.")]
	public event EventHandler<CellClickEventArgs> CellClick;

	/// <summary>
	/// Triggered when the mouse is above a cell.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the mouse is over a cell.")]
	public event EventHandler<CellOverEventArgs> CellOver;

	/// <summary>
	/// Triggered when a cell is right clicked.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the user right clicks a cell.")]
	public event EventHandler<CellRightClickEventArgs> CellRightClick;

	/// <summary>
	/// This event is triggered when a cell needs a tool tip.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a cell needs a tool tip.")]
	public event EventHandler<ToolTipShowingEventArgs> CellToolTipShowing;

	/// <summary>
	/// This event is triggered when a checkbox is checked/unchecked on a subitem
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a checkbox is checked/unchecked on a subitem.")]
	public event EventHandler<SubItemCheckingEventArgs> SubItemChecking;

	/// <summary>
	/// Triggered when a column header is right clicked.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the user right clicks a column header.")]
	public event ColumnRightClickEventHandler ColumnRightClick;

	/// <summary>
	/// This event is triggered when the user releases a drag over an ObjectListView that
	/// has a SimpleDropSink installed as the drop handler.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the user dropped items onto the control.")]
	public event EventHandler<OlvDropEventArgs> Dropped;

	/// <summary>
	/// This event is triggered when the control needs to filter its collection of objects.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the control needs to filter its collection of objects.")]
	public event EventHandler<FilterEventArgs> Filter;

	/// <summary>
	/// This event is triggered when a cell needs to be formatted.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a cell needs to be formatted.")]
	public event EventHandler<FormatCellEventArgs> FormatCell;

	/// <summary>
	/// This event is triggered when the frozeness of the control changes.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when frozeness of the control changes.")]
	public event EventHandler<FreezeEventArgs> Freezing;

	/// <summary>
	/// This event is triggered when a row needs to be formatted.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a row needs to be formatted.")]
	public event EventHandler<FormatRowEventArgs> FormatRow;

	/// <summary>
	/// This event is triggered when a group is about to collapse or expand.
	/// This can be cancelled to prevent the expansion.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a group is about to collapse or expand.")]
	public event EventHandler<GroupExpandingCollapsingEventArgs> GroupExpandingCollapsing;

	/// <summary>
	/// This event is triggered when a group changes state.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a group changes state.")]
	public event EventHandler<GroupStateChangedEventArgs> GroupStateChanged;

	/// <summary>
	/// This event is triggered when a header checkbox is changing value
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a header checkbox changes value.")]
	public event EventHandler<HeaderCheckBoxChangingEventArgs> HeaderCheckBoxChanging;

	/// <summary>
	/// This event is triggered when a header needs a tool tip.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a header needs a tool tip.")]
	public event EventHandler<ToolTipShowingEventArgs> HeaderToolTipShowing;

	/// <summary>
	/// Triggered when the "hot" item changes
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the hot item changed.")]
	public event EventHandler<HotItemChangedEventArgs> HotItemChanged;

	/// <summary>
	/// Triggered when a hyperlink cell is clicked.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a hyperlink cell is clicked.")]
	public event EventHandler<HyperlinkClickedEventArgs> HyperlinkClicked;

	/// <summary>
	/// Triggered when the task text of a group is clicked.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the task text of a group is clicked.")]
	public event EventHandler<GroupTaskClickedEventArgs> GroupTaskClicked;

	/// <summary>
	/// Is the value in the given cell a hyperlink.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the control needs to know if a given cell contains a hyperlink.")]
	public event EventHandler<IsHyperlinkEventArgs> IsHyperlink;

	/// <summary>
	/// Some new objects are about to be added to an ObjectListView.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when objects are about to be added to the control")]
	public event EventHandler<ItemsAddingEventArgs> ItemsAdding;

	/// <summary>
	/// The contents of the ObjectListView has changed.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the contents of the control have changed.")]
	public event EventHandler<ItemsChangedEventArgs> ItemsChanged;

	/// <summary>
	/// The contents of the ObjectListView is about to change via a SetObjects call
	/// </summary>
	/// <remarks>
	/// <para>Set Cancelled to true to prevent the contents of the list changing. This does not work with virtual lists.</para>
	/// </remarks>
	[Category("ObjectListView"),
	Description("This event is triggered when the contents of the control changes.")]
	public event EventHandler<ItemsChangingEventArgs> ItemsChanging;

	/// <summary>
	/// Some objects are about to be removed from an ObjectListView.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when objects are removed from the control.")]
	public event EventHandler<ItemsRemovingEventArgs> ItemsRemoving;

	/// <summary>
	/// This event is triggered when the user moves a drag over an ObjectListView that
	/// has a SimpleDropSink installed as the drop handler, and when the source control
	/// for the drag was an ObjectListView.
	/// </summary>
	/// <remarks>
	/// Handlers for this event should set the Effect argument and optionally the
	/// InfoMsg property. They can also change any of the DropTarget* setttings to change
	/// the target of the drop.
	/// </remarks>
	[Category("ObjectListView"),
	Description("Can the dragged collection of model objects be dropped at the current mouse location")]
	public event EventHandler<ModelDropEventArgs> ModelCanDrop;

	/// <summary>
	/// This event is triggered when the user releases a drag over an ObjectListView that
	/// has a SimpleDropSink installed as the drop handler and when the source control
	/// for the drag was an ObjectListView.
	/// </summary>
	[Category("ObjectListView"),
	Description("A collection of model objects from a ObjectListView has been dropped on this control")]
	public event EventHandler<ModelDropEventArgs> ModelDropped;

	/// <summary>
	/// This event is triggered once per user action that changes the selection state
	/// of one or more rows.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered once per user action that changes the selection state of one or more rows.")]
	public event EventHandler SelectionChanged;

	/// <summary>
	/// This event is triggered when the contents of the ObjectListView has scrolled.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when the contents of the ObjectListView has scrolled.")]
	public event EventHandler<ScrollEventArgs> Scroll;

	#endregion

	//-----------------------------------------------------------------------------------
	#region OnEvents

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnAboutToCreateGroups(CreateGroupsEventArgs e)
	{
		if (AboutToCreateGroups != null)
		{
			AboutToCreateGroups(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnBeforeCreatingGroups(CreateGroupsEventArgs e)
	{
		if (BeforeCreatingGroups != null)
		{
			BeforeCreatingGroups(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnAfterCreatingGroups(CreateGroupsEventArgs e)
	{
		if (AfterCreatingGroups != null)
		{
			AfterCreatingGroups(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnAfterSearching(AfterSearchingEventArgs e)
	{
		if (AfterSearching != null)
		{
			AfterSearching(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnAfterSorting(AfterSortingEventArgs e)
	{
		if (AfterSorting != null)
		{
			AfterSorting(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnBeforeSearching(BeforeSearchingEventArgs e)
	{
		if (BeforeSearching != null)
		{
			BeforeSearching(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnBeforeSorting(BeforeSortingEventArgs e)
	{
		if (BeforeSorting != null)
		{
			BeforeSorting(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnButtonClick(CellClickEventArgs args)
	{
		if (ButtonClick != null)
		{
			ButtonClick(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnCanDrop(OlvDropEventArgs args)
	{
		if (CanDrop != null)
		{
			CanDrop(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnCellClick(CellClickEventArgs args)
	{
		if (CellClick != null)
		{
			CellClick(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnCellOver(CellOverEventArgs args)
	{
		if (CellOver != null)
		{
			CellOver(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnCellRightClick(CellRightClickEventArgs args)
	{
		if (CellRightClick != null)
		{
			CellRightClick(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnCellToolTip(ToolTipShowingEventArgs args)
	{
		if (CellToolTipShowing != null)
		{
			CellToolTipShowing(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnSubItemChecking(SubItemCheckingEventArgs args)
	{
		if (SubItemChecking != null)
		{
			SubItemChecking(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnColumnRightClick(ColumnClickEventArgs e)
	{
		if (ColumnRightClick != null)
		{
			ColumnRightClick(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnDropped(OlvDropEventArgs args)
	{
		if (Dropped != null)
		{
			Dropped(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	internal protected virtual void OnFilter(FilterEventArgs e)
	{
		if (Filter != null)
		{
			Filter(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnFormatCell(FormatCellEventArgs args)
	{
		if (FormatCell != null)
		{
			FormatCell(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnFormatRow(FormatRowEventArgs args)
	{
		if (FormatRow != null)
		{
			FormatRow(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnFreezing(FreezeEventArgs args)
	{
		if (Freezing != null)
		{
			Freezing(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnGroupExpandingCollapsing(GroupExpandingCollapsingEventArgs args)
	{
		if (GroupExpandingCollapsing != null)
		{
			GroupExpandingCollapsing(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnGroupStateChanged(GroupStateChangedEventArgs args)
	{
		if (GroupStateChanged != null)
		{
			GroupStateChanged(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnHeaderCheckBoxChanging(HeaderCheckBoxChangingEventArgs args)
	{
		if (HeaderCheckBoxChanging != null)
		{
			HeaderCheckBoxChanging(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnHeaderToolTip(ToolTipShowingEventArgs args)
	{
		if (HeaderToolTipShowing != null)
		{
			HeaderToolTipShowing(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnHotItemChanged(HotItemChangedEventArgs e)
	{
		if (HotItemChanged != null)
		{
			HotItemChanged(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnHyperlinkClicked(HyperlinkClickedEventArgs e)
	{
		if (HyperlinkClicked != null)
		{
			HyperlinkClicked(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnGroupTaskClicked(GroupTaskClickedEventArgs e)
	{
		if (GroupTaskClicked != null)
		{
			GroupTaskClicked(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnIsHyperlink(IsHyperlinkEventArgs e)
	{
		if (IsHyperlink != null)
		{
			IsHyperlink(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnItemsAdding(ItemsAddingEventArgs e)
	{
		if (ItemsAdding != null)
		{
			ItemsAdding(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnItemsChanged(ItemsChangedEventArgs e)
	{
		if (ItemsChanged != null)
		{
			ItemsChanged(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnItemsChanging(ItemsChangingEventArgs e)
	{
		if (ItemsChanging != null)
		{
			ItemsChanging(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnItemsRemoving(ItemsRemovingEventArgs e)
	{
		if (ItemsRemoving != null)
		{
			ItemsRemoving(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnModelCanDrop(ModelDropEventArgs args)
	{
		if (ModelCanDrop != null)
		{
			ModelCanDrop(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="args"></param>
	protected virtual void OnModelDropped(ModelDropEventArgs args)
	{
		if (ModelDropped != null)
		{
			ModelDropped(this, args);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnSelectionChanged(EventArgs e)
	{
		if (SelectionChanged != null)
		{
			SelectionChanged(this, e);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnScroll(ScrollEventArgs e)
	{
		if (Scroll != null)
		{
			Scroll(this, e);
		}
	}


	/// <summary>
	/// Tell the world when a cell is about to be edited.
	/// </summary>
	protected virtual void OnCellEditStarting(CellEditEventArgs e)
	{
		if (CellEditStarting != null)
		{
			CellEditStarting(this, e);
		}
	}

	/// <summary>
	/// Tell the world when a cell is about to finish being edited.
	/// </summary>
	protected virtual void OnCellEditorValidating(CellEditEventArgs e)
	{
		// Hack. ListView is an imperfect control container. It does not manage validation
		// perfectly. If the ListView is part of a TabControl, and the cell editor loses
		// focus by the user clicking on another tab, the TabControl processes the click
		// and switches tabs, even if this Validating event cancels. This results in the
		// strange situation where the cell editor is active, but isn't visible. When the
		// user switches back to the tab with the ListView, composite controls like spin
		// controls, DateTimePicker and ComboBoxes do not work properly. Specifically,
		// keyboard input still works fine, but the controls do not respond to mouse
		// input. SO, if the validation fails, we have to specifically give focus back to
		// the cell editor. (this is the Select() call in the code below). 
		// But (there is always a 'but'), doing that changes the focus so the cell editor
		// triggers another Validating event -- which fails again. From the user's point
		// of view, they click away from the cell editor, and the validating code
		// complains twice. So we only trigger a Validating event if more than 0.1 seconds
		// has elapsed since the last validate event.
		// I know it's a hack. I'm very open to hear a neater solution.

		// Also, this timed response stops us from sending a series of validation events
		// if the user clicks and holds on the OLV scroll bar.
		//System.Diagnostics.Debug.WriteLine(Environment.TickCount - lastValidatingEvent);
		if ((Environment.TickCount - lastValidatingEvent) < 100)
		{
			e.Cancel = true;
		}
		else
		{
			lastValidatingEvent = Environment.TickCount;
			if (CellEditValidating != null)
			{
				CellEditValidating(this, e);
			}
		}
		lastValidatingEvent = Environment.TickCount;
	}
	private int lastValidatingEvent = 0;

	/// <summary>
	/// Tell the world when a cell is about to finish being edited.
	/// </summary>
	protected virtual void OnCellEditFinishing(CellEditEventArgs e)
	{
		if (CellEditFinishing != null)
		{
			CellEditFinishing(this, e);
		}
	}

	/// <summary>
	/// Tell the world when a cell has finished being edited.
	/// </summary>
	protected virtual void OnCellEditFinished(CellEditEventArgs e)
	{
		if (CellEditFinished != null)
		{
			CellEditFinished(this, e);
		}
	}

	#endregion
}

public partial class TreeListView
{

	#region Events

	/// <summary>
	/// This event is triggered when user input requests the expansion of a list item.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a branch is about to expand.")]
	public event EventHandler<TreeBranchExpandingEventArgs> Expanding;

	/// <summary>
	/// This event is triggered when user input requests the collapse of a list item.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a branch is about to collapsed.")]
	public event EventHandler<TreeBranchCollapsingEventArgs> Collapsing;

	/// <summary>
	/// This event is triggered after the expansion of a list item due to user input.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a branch has been expanded.")]
	public event EventHandler<TreeBranchExpandedEventArgs> Expanded;

	/// <summary>
	/// This event is triggered after the collapse of a list item due to user input.
	/// </summary>
	[Category("ObjectListView"),
	Description("This event is triggered when a branch has been collapsed.")]
	public event EventHandler<TreeBranchCollapsedEventArgs> Collapsed;

	#endregion

	#region OnEvents

	/// <summary>
	/// Trigger the expanding event
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnExpanding(TreeBranchExpandingEventArgs e)
	{
		if (Expanding != null)
		{
			Expanding(this, e);
		}
	}

	/// <summary>
	/// Trigger the collapsing event
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnCollapsing(TreeBranchCollapsingEventArgs e)
	{
		if (Collapsing != null)
		{
			Collapsing(this, e);
		}
	}

	/// <summary>
	/// Trigger the expanded event
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnExpanded(TreeBranchExpandedEventArgs e)
	{
		if (Expanded != null)
		{
			Expanded(this, e);
		}
	}

	/// <summary>
	/// Trigger the collapsed event
	/// </summary>
	/// <param name="e"></param>
	protected virtual void OnCollapsed(TreeBranchCollapsedEventArgs e)
	{
		if (Collapsed != null)
		{
			Collapsed(this, e);
		}
	}

	#endregion
}

//-----------------------------------------------------------------------------------
#region Event Parameter Blocks

/// <summary>
/// Let the world know that a cell edit operation is beginning or ending
/// </summary>
public class CellEditEventArgs : EventArgs
{
	/// <summary>
	/// Create an event args
	/// </summary>
	/// <param name="column"></param>
	/// <param name="control"></param>
	/// <param name="cellBounds"></param>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	public CellEditEventArgs(OLVColumn column, Control control, Rectangle cellBounds, OLVListItem item, int subItemIndex)
	{
		Control = control;
		Column = column;
		this.cellBounds = cellBounds;
		ListViewItem = item;
		RowObject = item.RowObject;
		SubItemIndex = subItemIndex;
		Value = column.GetValue(item.RowObject);
	}

	/// <summary>
	/// Change this to true to cancel the cell editing operation.
	/// </summary>
	/// <remarks>
	/// <para>During the CellEditStarting event, setting this to true will prevent the cell from being edited.</para>
	/// <para>During the CellEditFinishing event, if this value is already true, this indicates that the user has
	/// cancelled the edit operation and that the handler should perform cleanup only. Setting this to true,
	/// will prevent the ObjectListView from trying to write the new value into the model object.</para>
	/// </remarks>
	public bool Cancel;

	/// <summary>
	/// During the CellEditStarting event, this can be modified to be the control that you want
	/// to edit the value. You must fully configure the control before returning from the event,
	/// including its bounds and the value it is showing.
	/// During the CellEditFinishing event, you can use this to get the value that the user
	/// entered and commit that value to the model. Changing the control during the finishing
	/// event has no effect.
	/// </summary>
	public Control Control;

	/// <summary>
	/// The column of the cell that is going to be or has been edited.
	/// </summary>
	public OLVColumn Column { get; }

	/// <summary>
	/// The model object of the row of the cell that is going to be or has been edited.
	/// </summary>
	public object RowObject { get; }

	/// <summary>
	/// The listview item of the cell that is going to be or has been edited.
	/// </summary>
	public OLVListItem ListViewItem { get; }

	/// <summary>
	/// The data value of the cell as it stands in the control.
	/// </summary>
	/// <remarks>Only validate during Validating and Finishing events.</remarks>
	public object NewValue { get; set; }

	/// <summary>
	/// The index of the cell that is going to be or has been edited.
	/// </summary>
	public int SubItemIndex { get; }

	/// <summary>
	/// The data value of the cell before the edit operation began.
	/// </summary>
	public object Value { get; }

	/// <summary>
	/// The bounds of the cell that is going to be or has been edited.
	/// </summary>
	public Rectangle CellBounds => cellBounds;
	private Rectangle cellBounds;

	/// <summary>
	/// Gets or sets whether the control used for editing should be auto matically disposed
	/// when the cell edit operation finishes. Defaults to true
	/// </summary>
	/// <remarks>If the control is expensive to create, you might want to cache it and reuse for
	/// for various cells. If so, you don't want ObjectListView to dispose of the control automatically</remarks>
	public bool AutoDispose { get; set; } = true;
}

/// <summary>
/// Event blocks for events that can be cancelled
/// </summary>
public class CancellableEventArgs : EventArgs
{
	/// <summary>
	/// Has this event been cancelled by the event handler?
	/// </summary>
	public bool Canceled;
}

/// <summary>
/// BeforeSorting
/// </summary>
public class BeforeSortingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create BeforeSortingEventArgs
	/// </summary>
	/// <param name="column"></param>
	/// <param name="order"></param>
	/// <param name="column2"></param>
	/// <param name="order2"></param>
	public BeforeSortingEventArgs(OLVColumn column, SortOrder order, OLVColumn column2, SortOrder order2)
	{
		ColumnToGroupBy = column;
		GroupByOrder = order;
		ColumnToSort = column;
		SortOrder = order;
		SecondaryColumnToSort = column2;
		SecondarySortOrder = order2;
	}

	/// <summary>
	/// Create BeforeSortingEventArgs
	/// </summary>
	/// <param name="groupColumn"></param>
	/// <param name="groupOrder"></param>
	/// <param name="column"></param>
	/// <param name="order"></param>
	/// <param name="column2"></param>
	/// <param name="order2"></param>
	public BeforeSortingEventArgs(OLVColumn groupColumn, SortOrder groupOrder, OLVColumn column, SortOrder order, OLVColumn column2, SortOrder order2)
	{
		ColumnToGroupBy = groupColumn;
		GroupByOrder = groupOrder;
		ColumnToSort = column;
		SortOrder = order;
		SecondaryColumnToSort = column2;
		SecondarySortOrder = order2;
	}

	/// <summary>
	/// Did the event handler already do the sorting for us?
	/// </summary>
	public bool Handled;

	/// <summary>
	/// What column will be used for grouping
	/// </summary>
	public OLVColumn ColumnToGroupBy;

	/// <summary>
	/// How will groups be ordered
	/// </summary>
	public SortOrder GroupByOrder;

	/// <summary>
	/// What column will be used for sorting
	/// </summary>
	public OLVColumn ColumnToSort;

	/// <summary>
	/// What order will be used for sorting. None means no sorting.
	/// </summary>
	public SortOrder SortOrder;

	/// <summary>
	/// What column will be used for secondary sorting?
	/// </summary>
	public OLVColumn SecondaryColumnToSort;

	/// <summary>
	/// What order will be used for secondary sorting?
	/// </summary>
	public SortOrder SecondarySortOrder;
}

/// <summary>
/// Sorting has just occurred.
/// </summary>
public class AfterSortingEventArgs : EventArgs
{
	/// <summary>
	/// Create a AfterSortingEventArgs
	/// </summary>
	/// <param name="groupColumn"></param>
	/// <param name="groupOrder"></param>
	/// <param name="column"></param>
	/// <param name="order"></param>
	/// <param name="column2"></param>
	/// <param name="order2"></param>
	public AfterSortingEventArgs(OLVColumn groupColumn, SortOrder groupOrder, OLVColumn column, SortOrder order, OLVColumn column2, SortOrder order2)
	{
		ColumnToGroupBy = groupColumn;
		GroupByOrder = groupOrder;
		ColumnToSort = column;
		SortOrder = order;
		SecondaryColumnToSort = column2;
		SecondarySortOrder = order2;
	}

	/// <summary>
	/// Create a AfterSortingEventArgs
	/// </summary>
	/// <param name="args"></param>
	public AfterSortingEventArgs(BeforeSortingEventArgs args)
	{
		ColumnToGroupBy = args.ColumnToGroupBy;
		GroupByOrder = args.GroupByOrder;
		ColumnToSort = args.ColumnToSort;
		SortOrder = args.SortOrder;
		SecondaryColumnToSort = args.SecondaryColumnToSort;
		SecondarySortOrder = args.SecondarySortOrder;
	}

	/// <summary>
	/// What column was used for grouping?
	/// </summary>
	public OLVColumn ColumnToGroupBy { get; }

	/// <summary>
	/// What ordering was used for grouping?
	/// </summary>
	public SortOrder GroupByOrder { get; }

	/// <summary>
	/// What column was used for sorting?
	/// </summary>
	public OLVColumn ColumnToSort { get; }

	/// <summary>
	/// What ordering was used for sorting?
	/// </summary>
	public SortOrder SortOrder { get; }

	/// <summary>
	/// What column was used for secondary sorting?
	/// </summary>
	public OLVColumn SecondaryColumnToSort { get; }

	/// <summary>
	/// What order was used for secondary sorting?
	/// </summary>
	public SortOrder SecondarySortOrder { get; }
}

/// <summary>
/// This event is triggered when the contents of a list have changed
/// and we want the world to have a chance to filter the list.
/// </summary>
public class FilterEventArgs : EventArgs
{
	/// <summary>
	/// Create a FilterEventArgs
	/// </summary>
	/// <param name="objects"></param>
	public FilterEventArgs(IEnumerable objects)
	{
		Objects = objects;
	}

	/// <summary>
	/// Gets or sets what objects are being filtered
	/// </summary>
	public IEnumerable Objects;

	/// <summary>
	/// Gets or sets what objects survived the filtering
	/// </summary>
	public IEnumerable FilteredObjects;
}

/// <summary>
/// This event is triggered after the items in the list have been changed,
/// either through SetObjects, AddObjects or RemoveObjects.
/// </summary>
public class ItemsChangedEventArgs : EventArgs
{
	/// <summary>
	/// Create a ItemsChangedEventArgs
	/// </summary>
	public ItemsChangedEventArgs()
	{
	}

	/// <summary>
	/// Constructor for this event when used by a virtual list
	/// </summary>
	/// <param name="oldObjectCount"></param>
	/// <param name="newObjectCount"></param>
	public ItemsChangedEventArgs(int oldObjectCount, int newObjectCount)
	{
		OldObjectCount = oldObjectCount;
		NewObjectCount = newObjectCount;
	}

	/// <summary>
	/// Gets how many items were in the list before it changed
	/// </summary>
	public int OldObjectCount { get; }

	/// <summary>
	/// Gets how many objects are in the list after the change.
	/// </summary>
	public int NewObjectCount { get; }
}

/// <summary>
/// This event is triggered by AddObjects before any change has been made to the list.
/// </summary>
public class ItemsAddingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create an ItemsAddingEventArgs
	/// </summary>
	/// <param name="objectsToAdd"></param>
	public ItemsAddingEventArgs(ICollection objectsToAdd)
	{
		ObjectsToAdd = objectsToAdd;
	}

	/// <summary>
	/// Create an ItemsAddingEventArgs
	/// </summary>
	/// <param name="index"></param>
	/// <param name="objectsToAdd"></param>
	public ItemsAddingEventArgs(int index, ICollection objectsToAdd)
	{
		Index = index;
		ObjectsToAdd = objectsToAdd;
	}

	/// <summary>
	/// Gets or sets where the collection is going to be inserted.
	/// </summary>
	public int Index;

	/// <summary>
	/// Gets or sets the objects to be added to the list
	/// </summary>
	public ICollection ObjectsToAdd;
}

/// <summary>
/// This event is triggered by SetObjects before any change has been made to the list.
/// </summary>
/// <remarks>
/// When used with a virtual list, OldObjects will always be null.
/// </remarks>
public class ItemsChangingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create ItemsChangingEventArgs
	/// </summary>
	/// <param name="oldObjects"></param>
	/// <param name="newObjects"></param>
	public ItemsChangingEventArgs(IEnumerable oldObjects, IEnumerable newObjects)
	{
		OldObjects = oldObjects;
		NewObjects = newObjects;
	}

	/// <summary>
	/// Gets the objects that were in the list before it change.
	/// For virtual lists, this will always be null.
	/// </summary>
	public IEnumerable OldObjects { get; }

	/// <summary>
	/// Gets or sets the objects that will be in the list after it changes.
	/// </summary>
	public IEnumerable NewObjects;
}

/// <summary>
/// This event is triggered by RemoveObjects before any change has been made to the list.
/// </summary>
public class ItemsRemovingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create an ItemsRemovingEventArgs
	/// </summary>
	/// <param name="objectsToRemove"></param>
	public ItemsRemovingEventArgs(ICollection objectsToRemove)
	{
		ObjectsToRemove = objectsToRemove;
	}

	/// <summary>
	/// Gets or sets the objects that will be removed
	/// </summary>
	public ICollection ObjectsToRemove;
}

/// <summary>
/// Triggered after the user types into a list
/// </summary>
public class AfterSearchingEventArgs : EventArgs
{
	/// <summary>
	/// Create an AfterSearchingEventArgs
	/// </summary>
	/// <param name="stringToFind"></param>
	/// <param name="indexSelected"></param>
	public AfterSearchingEventArgs(string stringToFind, int indexSelected)
	{
		StringToFind = stringToFind;
		IndexSelected = indexSelected;
	}

	/// <summary>
	/// Gets the string that was actually searched for
	/// </summary>
	public string StringToFind { get; }

	/// <summary>
	/// Gets or sets whether an the event handler already handled this event
	/// </summary>
	public bool Handled;

	/// <summary>
	/// Gets the index of the row that was selected by the search.
	/// -1 means that no row was matched
	/// </summary>
	public int IndexSelected { get; }
}

/// <summary>
/// Triggered when the user types into a list
/// </summary>
public class BeforeSearchingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create BeforeSearchingEventArgs
	/// </summary>
	/// <param name="stringToFind"></param>
	/// <param name="startSearchFrom"></param>
	public BeforeSearchingEventArgs(string stringToFind, int startSearchFrom)
	{
		StringToFind = stringToFind;
		StartSearchFrom = startSearchFrom;
	}

	/// <summary>
	/// Gets or sets the string that will be found by the search routine
	/// </summary>
	/// <remarks>Modifying this value does not modify the memory of what the user has typed. 
	/// When the user next presses a character, the search string will revert to what 
	/// the user has actually typed.</remarks>
	public string StringToFind;

	/// <summary>
	/// Gets or sets the index of the first row that will be considered to matching.
	/// </summary>
	public int StartSearchFrom;
}

/// <summary>
/// The parameter block when telling the world about a cell based event
/// </summary>
public class CellEventArgs : EventArgs
{
	/// <summary>
	/// Gets the ObjectListView that is the source of the event
	/// </summary>
	public ObjectListView ListView { get; internal set; }

	/// <summary>
	/// Gets the model object under the cell
	/// </summary>
	/// <remarks>This is null for events triggered by the header.</remarks>
	public object Model { get; internal set; }

	/// <summary>
	/// Gets the row index of the cell
	/// </summary>
	/// <remarks>This is -1 for events triggered by the header.</remarks>
	public int RowIndex { get; internal set; } = -1;

	/// <summary>
	/// Gets the column index of the cell
	/// </summary>
	/// <remarks>This is -1 when the view is not in details view.</remarks>
	public int ColumnIndex { get; internal set; } = -1;

	/// <summary>
	/// Gets the column of the cell 
	/// </summary>
	/// <remarks>This is null when the view is not in details view.</remarks>
	public OLVColumn Column { get; internal set; }

	/// <summary>
	/// Gets the location of the mouse at the time of the event
	/// </summary>
	public Point Location
	{
		get { return location; }
		internal set { location = value; }
	}
	private Point location;

	/// <summary>
	/// Gets the state of the modifier keys at the time of the event
	/// </summary>
	public Keys ModifierKeys { get; internal set; }

	/// <summary>
	/// Gets the item of the cell
	/// </summary>
	public OLVListItem Item { get; internal set; }

	/// <summary>
	/// Gets the subitem of the cell
	/// </summary>
	/// <remarks>This is null when the view is not in details view and 
	/// for event triggered by the header</remarks>
	public OLVListSubItem SubItem { get; internal set; }

	/// <summary>
	/// Gets the HitTest object that determined which cell was hit
	/// </summary>
	public OlvListViewHitTestInfo HitTest { get; internal set; }

	/// <summary>
	/// Gets or set if this event completelely handled. If it was, no further processing
	/// will be done for it.
	/// </summary>
	public bool Handled;
}

/// <summary>
/// Tells the world that a cell was clicked
/// </summary>
public class CellClickEventArgs : CellEventArgs
{
	/// <summary>
	/// Gets or sets the number of clicks associated with this event
	/// </summary>
	public int ClickCount { get; set; }
}

/// <summary>
/// Tells the world that a cell was right clicked
/// </summary>
public class CellRightClickEventArgs : CellEventArgs
{
	/// <summary>
	/// Gets or sets the menu that should be displayed as a result of this event.
	/// </summary>
	/// <remarks>The menu will be positioned at Location, so changing that property changes
	/// where the menu will be displayed.</remarks>
	public ContextMenuStrip MenuStrip;
}

/// <summary>
/// Tell the world that the mouse is over a given cell
/// </summary>
public class CellOverEventArgs : CellEventArgs
{
}

/// <summary>
/// Tells the world that the frozen-ness of the ObjectListView has changed.
/// </summary>
public class FreezeEventArgs : EventArgs
{
	/// <summary>
	/// Make a FreezeEventArgs
	/// </summary>
	/// <param name="freeze"></param>
	public FreezeEventArgs(int freeze)
	{
		FreezeLevel = freeze;
	}

	/// <summary>
	/// How frozen is the control? 0 means that the control is unfrozen, 
	/// more than 0 indicates froze.
	/// </summary>
	public int FreezeLevel { get; set; }
}

/// <summary>
/// The parameter block when telling the world that a tool tip is about to be shown.
/// </summary>
public class ToolTipShowingEventArgs : CellEventArgs
{
	/// <summary>
	/// Gets the tooltip control that is triggering the tooltip event
	/// </summary>
	public ToolTipControl ToolTipControl { get; internal set; }

	/// <summary>
	/// Gets or sets the text should be shown on the tooltip for this event
	/// </summary>
	/// <remarks>Setting this to empty or null prevents any tooltip from showing</remarks>
	public string Text;

	/// <summary>
	/// In what direction should the text for this tooltip be drawn?
	/// </summary>
	public RightToLeft RightToLeft;

	/// <summary>
	/// Should the tooltip for this event been shown in bubble style?
	/// </summary>
	/// <remarks>This doesn't work reliable under Vista</remarks>
	public bool? IsBalloon;

	/// <summary>
	/// What color should be used for the background of the tooltip
	/// </summary>
	/// <remarks>Setting this does nothing under Vista</remarks>
	public Color? BackColor;

	/// <summary>
	/// What color should be used for the foreground of the tooltip
	/// </summary>
	/// <remarks>Setting this does nothing under Vista</remarks>
	public Color? ForeColor;

	/// <summary>
	/// What string should be used as the title for the tooltip for this event?
	/// </summary>
	public string Title;

	/// <summary>
	/// Which standard icon should be used for the tooltip for this event
	/// </summary>
	public ToolTipControl.StandardIcons? StandardIcon;

	/// <summary>
	/// How many milliseconds should the tooltip remain before it automatically
	/// disappears.
	/// </summary>
	public int? AutoPopDelay;

	/// <summary>
	/// What font should be used to draw the text of the tooltip?
	/// </summary>
	public Font Font;
}

/// <summary>
/// Common information to all hyperlink events
/// </summary>
public class HyperlinkEventArgs : EventArgs
{
	//TODO: Unified with CellEventArgs

	/// <summary>
	/// Gets the ObjectListView that is the source of the event
	/// </summary>
	public ObjectListView ListView { get; internal set; }

	/// <summary>
	/// Gets the model object under the cell
	/// </summary>
	public object Model { get; internal set; }

	/// <summary>
	/// Gets the row index of the cell
	/// </summary>
	public int RowIndex { get; internal set; } = -1;

	/// <summary>
	/// Gets the column index of the cell
	/// </summary>
	/// <remarks>This is -1 when the view is not in details view.</remarks>
	public int ColumnIndex { get; internal set; } = -1;

	/// <summary>
	/// Gets the column of the cell 
	/// </summary>
	/// <remarks>This is null when the view is not in details view.</remarks>
	public OLVColumn Column { get; internal set; }

	/// <summary>
	/// Gets the item of the cell
	/// </summary>
	public OLVListItem Item { get; internal set; }

	/// <summary>
	/// Gets the subitem of the cell
	/// </summary>
	/// <remarks>This is null when the view is not in details view</remarks>
	public OLVListSubItem SubItem { get; internal set; }

	/// <summary>
	/// Gets the ObjectListView that is the source of the event
	/// </summary>
	public string Url { get; internal set; }

	/// <summary>
	/// Gets or set if this event completelely handled. If it was, no further processing
	/// will be done for it.
	/// </summary>
	public bool Handled { get; set; }
}

/// <summary>
/// 
/// </summary>
public class IsHyperlinkEventArgs : EventArgs
{
	/// <summary>
	/// Gets the ObjectListView that is the source of the event
	/// </summary>
	public ObjectListView ListView { get; internal set; }

	/// <summary>
	/// Gets the model object under the cell
	/// </summary>
	public object Model { get; internal set; }

	/// <summary>
	/// Gets the column of the cell 
	/// </summary>
	/// <remarks>This is null when the view is not in details view.</remarks>
	public OLVColumn Column { get; internal set; }

	/// <summary>
	/// Gets the text of the cell 
	/// </summary>
	public string Text { get; internal set; }

	/// <summary>
	/// Gets or sets whether or not this cell is a hyperlink.
	/// Defaults to true for enabled rows and false for disabled rows. 
	/// </summary>
	public bool IsHyperlink { get; set; }

	/// <summary>
	/// Gets or sets the url that should be invoked when this cell is clicked.
	/// </summary>
	/// <remarks>Setting this to None or String.Empty means that this cell is not a hyperlink</remarks>
	public string Url;
}

/// <summary>
/// </summary>
public class FormatRowEventArgs : EventArgs
{
	//TODO: Unified with CellEventArgs

	/// <summary>
	/// Gets the ObjectListView that is the source of the event
	/// </summary>
	public ObjectListView ListView { get; internal set; }

	/// <summary>
	/// Gets the item of the cell
	/// </summary>
	public OLVListItem Item { get; internal set; }

	/// <summary>
	/// Gets the model object under the cell
	/// </summary>
	public object Model => Item.RowObject;

	/// <summary>
	/// Gets the row index of the cell
	/// </summary>
	public int RowIndex { get; internal set; } = -1;

	/// <summary>
	/// Gets the display index of the row
	/// </summary>
	public int DisplayIndex { get; internal set; } = -1;

	/// <summary>
	/// Should events be triggered for each cell in this row?
	/// </summary>
	public bool UseCellFormatEvents { get; set; }
}

/// <summary>
/// Parameter block for FormatCellEvent
/// </summary>
public class FormatCellEventArgs : FormatRowEventArgs
{
	/// <summary>
	/// Gets the column index of the cell
	/// </summary>
	/// <remarks>This is -1 when the view is not in details view.</remarks>
	public int ColumnIndex { get; internal set; } = -1;

	/// <summary>
	/// Gets the column of the cell 
	/// </summary>
	/// <remarks>This is null when the view is not in details view.</remarks>
	public OLVColumn Column { get; internal set; }

	/// <summary>
	/// Gets the subitem of the cell
	/// </summary>
	/// <remarks>This is null when the view is not in details view</remarks>
	public OLVListSubItem SubItem { get; internal set; }

	/// <summary>
	/// Gets the model value that is being displayed by the cell.
	/// </summary>
	/// <remarks>This is null when the view is not in details view</remarks>
	public object CellValue => SubItem == null ? null : SubItem.ModelValue;
}

/// <summary>
/// The event args when a hyperlink is clicked
/// </summary>
public class HyperlinkClickedEventArgs : CellEventArgs
{
	/// <summary>
	/// Gets the url that was associated with this cell.
	/// </summary>
	public string Url { get; set; }
}

/// <summary>
/// The event args when the check box in a column header is changing
/// </summary>
public class HeaderCheckBoxChangingEventArgs : CancelEventArgs
{

	/// <summary>
	/// Get the column whose checkbox is changing
	/// </summary>
	public OLVColumn Column { get; internal set; }

	/// <summary>
	/// Get or set the new state that should be used by the column
	/// </summary>
	public CheckState NewCheckState { get; set; }
}

/// <summary>
/// The event args when the hot item changed
/// </summary>
public class HotItemChangedEventArgs : EventArgs
{
	/// <summary>
	/// Gets or set if this event completelely handled. If it was, no further processing
	/// will be done for it.
	/// </summary>
	public bool Handled { get; set; }

	/// <summary>
	/// Gets the part of the cell that the mouse is over
	/// </summary>
	public HitTestLocation HotCellHitLocation { get; internal set; }

	/// <summary>
	/// Gets an extended indication of the part of item/subitem/group that the mouse is currently over
	/// </summary>
	public virtual HitTestLocationEx HotCellHitLocationEx
	{
		get { return hotCellHitLocationEx; }
		internal set { hotCellHitLocationEx = value; }
	}
	private HitTestLocationEx hotCellHitLocationEx;

	/// <summary>
	/// Gets the index of the column that the mouse is over
	/// </summary>
	/// <remarks>In non-details view, this will always be 0.</remarks>
	public int HotColumnIndex { get; internal set; }

	/// <summary>
	/// Gets the index of the row that the mouse is over
	/// </summary>
	public int HotRowIndex { get; internal set; }

	/// <summary>
	/// Gets the group that the mouse is over
	/// </summary>
	public OLVGroup HotGroup { get; internal set; }

	/// <summary>
	/// Gets the part of the cell that the mouse used to be over
	/// </summary>
	public HitTestLocation OldHotCellHitLocation { get; internal set; }

	/// <summary>
	/// Gets an extended indication of the part of item/subitem/group that the mouse used to be over
	/// </summary>
	public virtual HitTestLocationEx OldHotCellHitLocationEx
	{
		get { return oldHotCellHitLocationEx; }
		internal set { oldHotCellHitLocationEx = value; }
	}
	private HitTestLocationEx oldHotCellHitLocationEx;

	/// <summary>
	/// Gets the index of the column that the mouse used to be over
	/// </summary>
	public int OldHotColumnIndex { get; internal set; }

	/// <summary>
	/// Gets the index of the row that the mouse used to be over
	/// </summary>
	public int OldHotRowIndex { get; internal set; }

	/// <summary>
	/// Gets the group that the mouse used to be over
	/// </summary>
	public OLVGroup OldHotGroup { get; internal set; }

	/// <summary>
	/// Returns a string that represents the current object.
	/// </summary>
	/// <returns>
	/// A string that represents the current object.
	/// </returns>
	/// <filterpriority>2</filterpriority>
	public override string ToString() => string.Format("NewHotCellHitLocation: {0}, HotCellHitLocationEx: {1}, NewHotColumnIndex: {2}, NewHotRowIndex: {3}, HotGroup: {4}", HotCellHitLocation, hotCellHitLocationEx, HotColumnIndex, HotRowIndex, HotGroup);
}

/// <summary>
/// Let the world know that a checkbox on a subitem is changing
/// </summary>
public class SubItemCheckingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create a new event block
	/// </summary>
	/// <param name="column"></param>
	/// <param name="item"></param>
	/// <param name="subItemIndex"></param>
	/// <param name="currentValue"></param>
	/// <param name="newValue"></param>
	public SubItemCheckingEventArgs(OLVColumn column, OLVListItem item, int subItemIndex, CheckState currentValue, CheckState newValue)
	{
		Column = column;
		ListViewItem = item;
		SubItemIndex = subItemIndex;
		CurrentValue = currentValue;
		NewValue = newValue;
	}

	/// <summary>
	/// The column of the cell that is having its checkbox changed.
	/// </summary>
	public OLVColumn Column { get; }

	/// <summary>
	/// The model object of the row of the cell that is having its checkbox changed.
	/// </summary>
	public object RowObject => ListViewItem.RowObject;

	/// <summary>
	/// The listview item of the cell that is having its checkbox changed.
	/// </summary>
	public OLVListItem ListViewItem { get; }

	/// <summary>
	/// The current check state of the cell.
	/// </summary>
	public CheckState CurrentValue { get; }

	/// <summary>
	/// The proposed new check state of the cell.
	/// </summary>
	public CheckState NewValue { get; set; }

	/// <summary>
	/// The index of the cell that is going to be or has been edited.
	/// </summary>
	public int SubItemIndex { get; }
}

/// <summary>
/// This event argument block is used when groups are created for a list.
/// </summary>
public class CreateGroupsEventArgs : EventArgs
{
	/// <summary>
	/// Create a CreateGroupsEventArgs
	/// </summary>
	/// <param name="parms"></param>
	public CreateGroupsEventArgs(GroupingParameters parms)
	{
		Parameters = parms;
	}

	/// <summary>
	/// Gets the settings that control the creation of groups
	/// </summary>
	public GroupingParameters Parameters { get; }

	/// <summary>
	/// Gets or sets the groups that should be used
	/// </summary>
	public IList<OLVGroup> Groups { get; set; }

	/// <summary>
	/// Has this event been cancelled by the event handler?
	/// </summary>
	public bool Canceled { get; set; }
}

/// <summary>
/// This event argument block is used when the text of a group task is clicked
/// </summary>
public class GroupTaskClickedEventArgs : EventArgs
{
	/// <summary>
	/// Create a GroupTaskClickedEventArgs
	/// </summary>
	/// <param name="group"></param>
	public GroupTaskClickedEventArgs(OLVGroup group)
	{
		Group = group;
	}

	/// <summary>
	/// Gets which group was clicked
	/// </summary>
	public OLVGroup Group { get; }
}

/// <summary>
/// This event argument block is used when a group is about to expand or collapse
/// </summary>
public class GroupExpandingCollapsingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create a GroupExpandingCollapsingEventArgs
	/// </summary>
	/// <param name="group"> </param>
	public GroupExpandingCollapsingEventArgs(OLVGroup group)
	{
		Group = group ?? throw new ArgumentNullException(nameof(group));
	}

	/// <summary>
	/// Gets which group is expanding/collapsing
	/// </summary>
	public OLVGroup Group { get; }

	/// <summary>
	/// Gets whether this event is going to expand the group.
	/// If this is false, the group must be collapsing.
	/// </summary>
	public bool IsExpanding => Group.Collapsed;
}

/// <summary>
/// This event argument block is used when the state of group has changed (collapsed, selected)
/// </summary>
public class GroupStateChangedEventArgs : EventArgs
{
	/// <summary>
	/// Create a GroupStateChangedEventArgs
	/// </summary>
	/// <param name="group"></param>
	/// <param name="oldState"> </param>
	/// <param name="newState"> </param>
	public GroupStateChangedEventArgs(OLVGroup group, GroupState oldState, GroupState newState)
	{
		Group = group;
		OldState = oldState;
		NewState = newState;
	}

	/// <summary>
	/// Gets whether the group was collapsed by this event
	/// </summary>
	public bool Collapsed => ((OldState & GroupState.LVGS_COLLAPSED) != GroupState.LVGS_COLLAPSED) &&
				   ((NewState & GroupState.LVGS_COLLAPSED) == GroupState.LVGS_COLLAPSED);

	/// <summary>
	/// Gets whether the group was focused by this event
	/// </summary>
	public bool Focused => ((OldState & GroupState.LVGS_FOCUSED) != GroupState.LVGS_FOCUSED) &&
				   ((NewState & GroupState.LVGS_FOCUSED) == GroupState.LVGS_FOCUSED);

	/// <summary>
	/// Gets whether the group was selected by this event
	/// </summary>
	public bool Selected => ((OldState & GroupState.LVGS_SELECTED) != GroupState.LVGS_SELECTED) &&
				   ((NewState & GroupState.LVGS_SELECTED) == GroupState.LVGS_SELECTED);

	/// <summary>
	/// Gets whether the group was uncollapsed by this event
	/// </summary>
	public bool Uncollapsed => ((OldState & GroupState.LVGS_COLLAPSED) == GroupState.LVGS_COLLAPSED) &&
				   ((NewState & GroupState.LVGS_COLLAPSED) != GroupState.LVGS_COLLAPSED);

	/// <summary>
	/// Gets whether the group was unfocused by this event
	/// </summary>
	public bool Unfocused => ((OldState & GroupState.LVGS_FOCUSED) == GroupState.LVGS_FOCUSED) &&
				   ((NewState & GroupState.LVGS_FOCUSED) != GroupState.LVGS_FOCUSED);

	/// <summary>
	/// Gets whether the group was unselected by this event
	/// </summary>
	public bool Unselected => ((OldState & GroupState.LVGS_SELECTED) == GroupState.LVGS_SELECTED) &&
				   ((NewState & GroupState.LVGS_SELECTED) != GroupState.LVGS_SELECTED);

	/// <summary>
	/// Gets which group had its state changed
	/// </summary>
	public OLVGroup Group { get; }

	/// <summary>
	/// Gets the previous state of the group
	/// </summary>
	public GroupState OldState { get; }


	/// <summary>
	/// Gets the new state of the group
	/// </summary>
	public GroupState NewState { get; }
}

/// <summary>
/// This event argument block is used when a branch of a tree is about to be expanded
/// </summary>
public class TreeBranchExpandingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create a new event args
	/// </summary>
	/// <param name="model"></param>
	/// <param name="item"></param>
	public TreeBranchExpandingEventArgs(object model, OLVListItem item)
	{
		Model = model;
		Item = item;
	}

	/// <summary>
	/// Gets the model that is about to expand. If null, all branches are going to be expanded.
	/// </summary>
	public object Model { get; private set; }

	/// <summary>
	/// Gets the OLVListItem that is about to be expanded
	/// </summary>
	public OLVListItem Item { get; private set; }
}

/// <summary>
/// This event argument block is used when a branch of a tree has just been expanded
/// </summary>
public class TreeBranchExpandedEventArgs : EventArgs
{
	/// <summary>
	/// Create a new event args
	/// </summary>
	/// <param name="model"></param>
	/// <param name="item"></param>
	public TreeBranchExpandedEventArgs(object model, OLVListItem item)
	{
		Model = model;
		Item = item;
	}

	/// <summary>
	/// Gets the model that is was expanded. If null, all branches were expanded.
	/// </summary>
	public object Model { get; private set; }

	/// <summary>
	/// Gets the OLVListItem that was expanded
	/// </summary>
	public OLVListItem Item { get; private set; }
}

/// <summary>
/// This event argument block is used when a branch of a tree is about to be collapsed
/// </summary>
public class TreeBranchCollapsingEventArgs : CancellableEventArgs
{
	/// <summary>
	/// Create a new event args
	/// </summary>
	/// <param name="model"></param>
	/// <param name="item"></param>
	public TreeBranchCollapsingEventArgs(object model, OLVListItem item)
	{
		Model = model;
		Item = item;
	}

	/// <summary>
	/// Gets the model that is about to collapse. If this is null, all models are going to collapse.
	/// </summary>
	public object Model { get; private set; }

	/// <summary>
	/// Gets the OLVListItem that is about to be collapsed. Can be null
	/// </summary>
	public OLVListItem Item { get; private set; }
}


/// <summary>
/// This event argument block is used when a branch of a tree has just been collapsed
/// </summary>
public class TreeBranchCollapsedEventArgs : EventArgs
{
	/// <summary>
	/// Create a new event args
	/// </summary>
	/// <param name="model"></param>
	/// <param name="item"></param>
	public TreeBranchCollapsedEventArgs(object model, OLVListItem item)
	{
		Model = model;
		Item = item;
	}

	/// <summary>
	/// Gets the model that is was collapsed. If null, all branches were collapsed
	/// </summary>
	public object Model { get; private set; }

	/// <summary>
	/// Gets the OLVListItem that was collapsed
	/// </summary>
	public OLVListItem Item { get; private set; }
}

#endregion
