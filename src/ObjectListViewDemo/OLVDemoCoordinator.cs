using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Globalization;

using BrightIdeasSoftware;

namespace ObjectListViewDemo;

/// <summary>
/// This class holds all the common bits of code that are used across multiple tabs.
/// It also provides the tabs with access to form level elements, like status bar texts.
/// </summary>
public class OLVDemoCoordinator
{
	public OLVDemoCoordinator(MainForm mainForm)
	{
		Form = mainForm;
	}

	public List<Person> PersonList => personList ?? (personList = InitializePersonList());
	private List<Person> personList;

	private MainForm Form { get; }

	private string prefixForNextSelectionMessage;

	public string ToolStripStatus1
	{
		get { return Form.toolStripStatusLabel1.Text; }
		set
		{
			Form.toolStripStatusLabel1.Text = value;
			Form.Update();
		}
	}

	private static List<Person> InitializePersonList()
	{
		List<Person> list = new()
		{
			new Person("Wilhelm Frat", "Gymnast", 21, new DateTime(1984, 9, 23), 45.67, false, "ak", "Aggressive, belligerent, pacifically challenged "),
			new Person("Alana Roderick", "Gymnast", 21, new DateTime(1974, 9, 23), 245.67, false, "gp", "Beautiful, exquisite, "),
			new Person("Frank Price", "Dancer", 30, new DateTime(1965, 11, 1), 75.5, false, "ns", "Competitive, spirited, timidically challenged"),
			new Person("Eric", "Half-a-bee", 1, new DateTime(1966, 10, 12), 12.25, true, "cp", "Diminutive, vertically challenged"),
			new Person("Nicola Scotts", "Nurse", 42, new DateTime(1965, 10, 29), 1245.7, false, "np", "Wise, fun, lovely"),
			new Person("Madeline Alright", "School Teacher", 21, new DateTime(1964, 9, 23), 145.67, false, "jr", "Extensive, dimensionally challenged"),
			new Person("Ned Peirce", "School Teacher", 21, new DateTime(1960, 1, 23), 145.67, false, "gab", "Fulsome, effusive, monosyllabically challenged"),
			new Person("Felicity Brown", "Economist", 30, new DateTime(1975, 1, 12), 175.5, false, "sp", "Gifted, exceptional, mudanically challenged"),
			new Person("Urny Unmin", "Economist", 41, new DateTime(1956, 9, 24), 212.25, true, "cr", "Heinous, aesthetically challenged"),
			new Person("Terrance Darby", "Singer", 35, new DateTime(1970, 9, 29), 1145, false, "mb", "Introverted, relationally challenged"),
			new Person("Phillip Nottingham", "Programmer", 27, new DateTime(1974, 8, 28), 245.7, false, "sj", "Jocular, gregarious, introvertially challenged"),
			new Person("Mister Null")
		};

		// Add copies of each person to the list
		List<Person> list2 = new();
		foreach (Person p in list)
		{
			list2.Add(new Person(p));
		}

		// Change this value to see the performance on bigger lists.
		// Each list builds about 1000 rows per second.
		//while (list.Count < 5000) {
		//    foreach (Person p in list)
		//        list2.Add(new Person(p));
		//}

		return list2;
	}

	public void ShowMessage(string message) => MessageBox.Show(message, "Object List View Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);

	public void TimedRebuildList(ObjectListView olv)
	{
		Stopwatch stopWatch = new();

		try
		{
			Form.Cursor = Cursors.WaitCursor;
			stopWatch.Start();
			olv.BuildList();
		}
		finally
		{
			stopWatch.Stop();
			Form.Cursor = Cursors.Default;
		}

		ToolStripStatus1 = prefixForNextSelectionMessage =
			string.Format("Build time: {0} items in {1}ms, average per item: {2:F}ms",
						  olv.Items.Count,
						  stopWatch.ElapsedMilliseconds,
						  (float)stopWatch.ElapsedMilliseconds / olv.Items.Count);
	}

	public void ShowGroupsChecked(ObjectListView olv, CheckBox cb)
	{
		if (cb.Checked && olv.View == View.List)
		{
			cb.Checked = false;
			ShowMessage("ListView's cannot show groups when in List view.");
		}
		else
		{
			olv.ShowGroups = cb.Checked;
			olv.BuildList();
		}
	}

	public void ShowLabelsOnGroupsChecked(ObjectListView olv, CheckBox cb)
	{
		olv.ShowItemCountOnGroups = cb.Checked;
		if (olv.ShowGroups)
		{
			olv.BuildGroups();
		}
	}

	public void HandleSelectionChanged(ObjectListView listView)
	{
		// Most ListViews in the demo handle lists of people, so we try to cast SelectedObject and FocusedObject to be Persons.

		string msg = listView.SelectedObject is not Person p ? listView.SelectedIndices.Count.ToString(CultureInfo.CurrentCulture) : string.Format("'{0}'", p.Name);
		string focusedMsg = listView.FocusedObject is not Person focused ? "" : string.Format(". Focused on '{0}'", focused.Name);
		ToolStripStatus1 =
			string.IsNullOrEmpty(prefixForNextSelectionMessage) ?
				string.Format("Selected {0} of {1} items" + focusedMsg, msg, listView.GetItemCount())
				: string.Format("{2}. Selected {0} of {1} items" + focusedMsg, msg, listView.GetItemCount(), prefixForNextSelectionMessage);
		prefixForNextSelectionMessage = null;

	}

	public void ChangeView(ObjectListView listview, ComboBox comboBox)
	{
		// Handle restrictions on Tile view
		if (comboBox.SelectedIndex == 3)
		{
			if (listview.VirtualMode)
			{
				ShowMessage("Sorry, Microsoft says that virtual lists can't use Tile view.");
				return;
			}
			if (listview.CheckBoxes)
			{
				ShowMessage("Microsoft says that Tile view can't have checkboxes, so CheckBoxes have been turned off on this list.");
				listview.CheckBoxes = false;
			}
		}

		switch (comboBox.SelectedIndex)
		{
			case 0:
				listview.View = View.SmallIcon;
				break;
			case 1:
				listview.View = View.LargeIcon;
				break;
			case 2:
				listview.View = View.List;
				break;
			case 3:
				listview.View = View.Tile;
				break;
			case 4:
				listview.View = View.Details;
				break;
		}
	}

	public void ChangeEditable(ObjectListView objectListView, ComboBox comboBox)
	{
		if (comboBox.Text == "No")
		{
			objectListView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
		}
		else if (comboBox.Text == "Single Click")
		{
			objectListView.CellEditActivation = ObjectListView.CellEditActivateMode.SingleClick;
		}
		else if (comboBox.Text == "Double Click")
		{
			objectListView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
		}
		else
		{
			objectListView.CellEditActivation = ObjectListView.CellEditActivateMode.F2Only;
		}
	}

	public void TimedFilter(ObjectListView olv, string txt) => TimedFilter(olv, txt, 0);

	public void TimedFilter(ObjectListView olv, string txt, int matchKind)
	{
		TextMatchFilter filter = null;
		if (!string.IsNullOrEmpty(txt))
		{
			switch (matchKind)
			{
				case 0:
				default:
					filter = TextMatchFilter.Contains(olv, txt);
					break;
				case 1:
					filter = TextMatchFilter.Prefix(olv, txt);
					break;
				case 2:
					filter = TextMatchFilter.Regex(olv, txt);
					break;
			}
		}

		// Text highlighting requires at least a default renderer
		if (olv.DefaultRenderer == null)
		{
			olv.DefaultRenderer = new HighlightTextRenderer(filter);
		}

		Stopwatch stopWatch = new();
		stopWatch.Start();
		olv.AdditionalFilter = filter;
		//olv.Invalidate();
		stopWatch.Stop();

		if (olv.Objects is not IList objects)
		{
			ToolStripStatus1 = prefixForNextSelectionMessage =
				string.Format("Filtered in {0}ms", stopWatch.ElapsedMilliseconds);
		}
		else
		{
			ToolStripStatus1 = prefixForNextSelectionMessage =
				string.Format("Filtered {0} items down to {1} items in {2}ms",
							  objects.Count,
							  olv.Items.Count,
							  stopWatch.ElapsedMilliseconds);
		}
	}

	public void ChangeHotItemStyle(ObjectListView olv, ComboBox cb)
	{
		olv.UseTranslucentHotItem = false;
		olv.UseHotItem = true;
		olv.UseExplorerTheme = false;

		switch (cb.SelectedIndex)
		{
			case 0:
				olv.UseHotItem = false;
				break;
			case 1:
				HotItemStyle hotItemStyle = new();
				hotItemStyle.ForeColor = Color.AliceBlue;
				hotItemStyle.BackColor = Color.FromArgb(255, 64, 64, 64);
				olv.HotItemStyle = hotItemStyle;
				break;
			case 2:
				RowBorderDecoration rbd = new();
				rbd.BorderPen = new Pen(Color.SeaGreen, 2);
				rbd.FillBrush = null;
				rbd.CornerRounding = 4.0f;
				HotItemStyle hotItemStyle2 = new();
				hotItemStyle2.Decoration = rbd;
				olv.HotItemStyle = hotItemStyle2;
				break;
			case 3:
				olv.UseTranslucentHotItem = true;
				break;
			case 4:
				HotItemStyle hotItemStyle3 = new();
				hotItemStyle3.Decoration = new LightBoxDecoration();
				olv.HotItemStyle = hotItemStyle3;
				break;
			case 5:
				olv.FullRowSelect = true;
				olv.UseHotItem = false;
				olv.UseExplorerTheme = true;
				break;
		}
		olv.Invalidate();
	}

	public void HandleHotItemChanged(object sender, HotItemChangedEventArgs e)
	{
		if (sender == null)
		{
			Form.toolStripStatusLabel3.Text = "";
			return;
		}

		switch (e.HotCellHitLocation)
		{
			case HitTestLocation.Nothing:
				Form.toolStripStatusLabel3.Text = @"Over nothing";
				break;
			case HitTestLocation.Header:
			case HitTestLocation.HeaderCheckBox:
			case HitTestLocation.HeaderDivider:
				Form.toolStripStatusLabel3.Text = string.Format("Over {0} of column #{1}", e.HotCellHitLocation, e.HotColumnIndex);
				break;
			case HitTestLocation.Group:
				Form.toolStripStatusLabel3.Text = string.Format("Over group '{0}', {1}", e.HotGroup.Header, e.HotCellHitLocationEx);
				break;
			case HitTestLocation.GroupExpander:
				Form.toolStripStatusLabel3.Text = string.Format("Over group expander of '{0}'", e.HotGroup.Header);
				break;
			default:
				Form.toolStripStatusLabel3.Text = string.Format("Over {0} of ({1}, {2})", e.HotCellHitLocation, e.HotRowIndex, e.HotColumnIndex);
				break;
		}
	}


	/// <summary>
	/// Format a file size into a more intelligible value
	/// </summary>
	/// <param name="size"></param>
	/// <returns></returns>
	public string FormatFileSize(long size)
	{
		int[] limits = new int[] { 1024 * 1024 * 1024, 1024 * 1024, 1024 };
		string[] units = new string[] { "GB", "MB", "KB" };

		for (int i = 0; i < limits.Length; i++)
		{
			if (size >= limits[i])
			{
				return string.Format("{0:#,##0.##} " + units[i], ((double)size / limits[i]));
			}
		}

		return string.Format("{0} bytes", size);
	}

	/// <summary>
	/// Read and return a DataSet from a given XML file
	/// </summary>
	/// <param name="fileName"></param>
	/// <returns></returns>
	public DataSet LoadDatasetFromXml(string fileName)
	{
		DataSet ds = new();
		FileStream fs = null;

		try
		{
			fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			using (StreamReader reader = new(fs))
			{
				ds.ReadXml(reader);
			}
		}
		catch (Exception e)
		{
			MessageBox.Show(e.ToString());
		}
		finally
		{
			if (fs != null)
			{
				fs.Close();
			}
		}

		return ds;
	}
}
