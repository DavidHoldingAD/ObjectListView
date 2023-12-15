using BrightIdeasSoftware;

namespace ObjectListViewDemo;

/// <summary>
/// This form is an example of how an application could allows the user to select which columns 
/// an ObjectListView will display, as well as select which order the columns are displayed in.
/// </summary>
/// <remarks>
/// <para>In Tile view, ColumnHeader.DisplayIndex does nothing. To reorder the columns you have
/// to change the order of objects in the Columns property.</para>
/// <para>Remember that the first column is special!
/// It has to remain the first column.</para>
/// </remarks>
public partial class ColumnSelectionForm : Form
{
	/// <summary>
	/// Make a new ColumnSelectionForm
	/// </summary>
	public ColumnSelectionForm()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Open this form so it will edit the columns that are available in the listview's current view
	/// </summary>
	/// <param name="olv">The ObjectListView whose columns are to be altered</param>
	public void OpenOn(ObjectListView olv) => OpenOn(olv, olv.View);

	/// <summary>
	/// Open this form so it will edit the columns that are available in the given listview
	/// when the listview is showing the given type of view.
	/// </summary>
	/// <remarks>RearrangableColumns are only visible in Details and Tile views, so view must be one
	/// of those values.</remarks>
	/// <param name="olv">The ObjectListView whose columns are to be altered</param>
	/// <param name="view">The view that is to be altered. Must be View.Details or View.Tile</param>
	public void OpenOn(ObjectListView olv, View view)
	{
		if (view != View.Details && view != View.Tile)
		{
			return;
		}

		InitializeForm(olv, view);
		if (ShowDialog() == DialogResult.OK)
		{
			ProcessOK(olv, view);
		}
	}

	/// <summary>
	/// Initialize the form to show the columns of the given view
	/// </summary>
	/// <param name="olv"></param>
	/// <param name="view"></param>
	protected void InitializeForm(ObjectListView olv, View view)
	{
		AllColumns = olv.AllColumns;
		RearrangableColumns = new List<OLVColumn>(AllColumns);
		foreach (OLVColumn col in RearrangableColumns)
		{
			if (view == View.Details)
			{
				MapColumnToVisible[col] = col.IsVisible;
			}
			else
			{
				MapColumnToVisible[col] = col.IsTileViewColumn;
			}
		}
		RearrangableColumns.Sort(new SortByDisplayOrder(this));

		objectListView1.BooleanCheckStateGetter = delegate (object rowObject)
		{
			return MapColumnToVisible[(OLVColumn)rowObject];
		};

		objectListView1.BooleanCheckStatePutter = delegate (object rowObject, bool newValue)
		{
			// primary column should always be checked so ignore attempts to change it
			if (!IsPrimaryColumn((OLVColumn)rowObject))
			{
				MapColumnToVisible[(OLVColumn)rowObject] = newValue;
				EnableControls();
			}
			return newValue;
		};

		objectListView1.SetObjects(RearrangableColumns);
	}
	private List<OLVColumn> AllColumns = null;
	private List<OLVColumn> RearrangableColumns = new();
	private Dictionary<OLVColumn, bool> MapColumnToVisible = new();

	/// <summary>
	/// The user has pressed OK. Do what's requied.
	/// </summary>
	/// <param name="olv"></param>
	/// <param name="view"></param>
	protected void ProcessOK(ObjectListView olv, View view)
	{
		olv.Freeze();

		// Update the column definitions to reflect whether they have been hidden
		foreach (OLVColumn col in olv.AllColumns)
		{
			if (view == View.Details)
			{
				col.IsVisible = MapColumnToVisible[col];
			}
			else
			{
				col.IsTileViewColumn = MapColumnToVisible[col];
			}
		}

		// Collect the columns are still visible
		List<OLVColumn> visibleColumns = RearrangableColumns.FindAll(
			delegate (OLVColumn x)
			{ return MapColumnToVisible[x]; });

		// Detail view and Tile view have to be handled in different ways.
		if (view == View.Details)
		{
			// Of the still visible columns, change DisplayIndex to reflect their position in the rearranged list
			olv.ChangeToFilteredColumns(view);
			foreach (ColumnHeader col in olv.Columns)
			{
				col.DisplayIndex = visibleColumns.IndexOf((OLVColumn)col);
			}
		}
		else
		{
			// In Tile view, DisplayOrder does nothing. So to change the display order, we have to change the 
			// order of the columns in the Columns property.
			// Remember, the primary column is special and has to remain first!
			OLVColumn primaryColumn = AllColumns[0];
			visibleColumns.Remove(primaryColumn);

			olv.Columns.Clear();
			olv.Columns.Add(primaryColumn);
			olv.Columns.AddRange(visibleColumns.ToArray());
			olv.CalculateReasonableTileSize();
		}

		olv.Unfreeze();
	}

	#region Event handlers

	private void buttonMoveUp_Click(object sender, EventArgs e)
	{
		int selectedIndex = objectListView1.SelectedIndices[0];
		OLVColumn col = RearrangableColumns[selectedIndex];
		RearrangableColumns.RemoveAt(selectedIndex);
		RearrangableColumns.Insert(selectedIndex - 1, col);

		objectListView1.BuildList();

		EnableControls();
	}

	private void buttonMoveDown_Click(object sender, EventArgs e)
	{
		int selectedIndex = objectListView1.SelectedIndices[0];
		OLVColumn col = RearrangableColumns[selectedIndex];
		RearrangableColumns.RemoveAt(selectedIndex);
		RearrangableColumns.Insert(selectedIndex + 1, col);

		objectListView1.BuildList();

		EnableControls();
	}

	private void buttonShow_Click(object sender, EventArgs e) => objectListView1.SelectedItem.Checked = true;

	private void buttonHide_Click(object sender, EventArgs e) => objectListView1.SelectedItem.Checked = false;

	private void buttonOK_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.OK;
		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void objectListView1_SelectionChanged(object sender, EventArgs e) => EnableControls();

	#endregion

	#region Control enabling

	private bool IsPrimaryColumn(OLVColumn col) => (col == AllColumns[0]);

	/// <summary>
	/// Enable the controls on the dialog to match the current state
	/// </summary>
	protected void EnableControls()
	{
		if (objectListView1.SelectedIndices.Count == 0)
		{
			buttonMoveUp.Enabled = false;
			buttonMoveDown.Enabled = false;
			buttonShow.Enabled = false;
			buttonHide.Enabled = false;
		}
		else
		{
			// Can't move the first row up or the last row down
			buttonMoveUp.Enabled = (objectListView1.SelectedIndices[0] != 0);
			buttonMoveDown.Enabled = (objectListView1.SelectedIndices[0] < (objectListView1.GetItemCount() - 1));

			OLVColumn selectedColumn = (OLVColumn)objectListView1.SelectedObject;

			// The primary column cannot be hidden (and hence cannot be Shown)
			buttonShow.Enabled = !MapColumnToVisible[selectedColumn] && !IsPrimaryColumn(selectedColumn);
			buttonHide.Enabled = MapColumnToVisible[selectedColumn] && !IsPrimaryColumn(selectedColumn);
		}
	}
	#endregion

	/// <summary>
	/// A Comparer that will sort a list of columns so that visible ones come before hidden ones,
	/// and that are ordered by their display order.
	/// </summary>
	private class SortByDisplayOrder : IComparer<OLVColumn>
	{
		public SortByDisplayOrder(ColumnSelectionForm form)
		{
			Form = form;
		}
		private ColumnSelectionForm Form;

		#region IComparer<OLVColumn> Members

		int IComparer<OLVColumn>.Compare(OLVColumn x, OLVColumn y)
		{
			if (Form.MapColumnToVisible[x] && !Form.MapColumnToVisible[y])
			{
				return -1;
			}

			if (!Form.MapColumnToVisible[x] && Form.MapColumnToVisible[y])
			{
				return 1;
			}

			if (x.DisplayIndex == y.DisplayIndex)
			{
				return x.Text.CompareTo(y.Text);
			}
			else
			{
				return x.DisplayIndex - y.DisplayIndex;
			}
		}

		#endregion
	}
}
