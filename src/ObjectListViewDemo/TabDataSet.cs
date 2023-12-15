using System.Diagnostics;
using System.Data;
using BrightIdeasSoftware;

namespace ObjectListViewDemo;

public partial class TabDataSet : OlvDemoTab
{

	public TabDataSet()
	{
		InitializeComponent();
		ListView = olvData;
	}

	protected override void InitializeTab()
	{

		comboBoxView.SelectedIndex = 4;
		comboBoxEditable.SelectedIndex = 0;
		rowHeightUpDown.Value = 32;

		SetupColumns();
		SetupCellFormatting();

		ReloadDataGridAndListView();
	}

	private void SetupCellFormatting()
	{

		// Setup cell formatting so that any cell that contains of the following colours
		// will have a foreground text of the same colour. If you prefix the colour with
		// "bk-" the colour will be applied to the background instead.
		// Similarly, if the text contains "bold", "italic", "underline" or "strikeout"...
		// well... you get the picture :)

		string[] colorNames = new string[] { "red", "green", "blue", "yellow", "black", "white" };

		olvData.UseCellFormatEvents = true;
		olvData.FormatCell += delegate (object sender, FormatCellEventArgs e)
		{
			string text = e.SubItem.Text.ToLowerInvariant();
			foreach (string name in colorNames)
			{
				if (text.Contains(name))
				{
					if (text.Contains("bk-" + name))
					{
						e.SubItem.BackColor = Color.FromName(name);
					}
					else
					{
						e.SubItem.ForeColor = Color.FromName(name);
					}
				}
			}
			FontStyle style = FontStyle.Regular;
			if (text.Contains("bold"))
			{
				style |= FontStyle.Bold;
			}

			if (text.Contains("italic"))
			{
				style |= FontStyle.Italic;
			}

			if (text.Contains("underline"))
			{
				style |= FontStyle.Underline;
			}

			if (text.Contains("strikeout"))
			{
				style |= FontStyle.Strikeout;
			}

			if (style != FontStyle.Regular)
			{
				e.SubItem.Font = new Font(e.SubItem.Font, style);
			}
		};
	}

	private void SetupColumns()
	{

		// DataListView are for the laziest of developers, so we really don't need any code to make it just work.
		// But with these couple of lines, we give each row an icon, and improve the grouping of a couple of columns

		olvColumn1.ImageGetter = delegate (object row)
		{ return "user"; };

		salaryColumn.MakeGroupies(
			new uint[] { 20000, 100000 },
			new string[] { "Lowly worker", "Middle management", "Rarified elevation" });

		heightColumn.MakeGroupies(
			new double[] { 1.50, 1.70, 1.85 },
			new string[] { "Shortie", "Normal", "Tall", "Really tall" });
	}

	private void ReloadDataGridAndListView()
	{
		DataSet ds = LoadXmlIntoDataGrid();

		if (ds != null)
		{
			LoadDataSetIntoListView(ds);
		}
	}

	private DataSet LoadXmlIntoDataGrid()
	{
		DataSet ds = Coordinator.LoadDatasetFromXml(@"Data\Persons.xml");

		if (ds.Tables.Count <= 0)
		{
			Coordinator.ShowMessage(@"Failed to load data set from Data\Persons.xml");
			return null;
		}

		dataGridView1.DataSource = ds;
		dataGridView1.DataMember = "Person";

		return ds;
	}

	private void LoadDataSetIntoListView(DataSet ds) =>

		// Install this data source

		// DataListView can bind to many different types of data source.
		// You can also set up a BindingSource in the designer and assign that 
		// to the DataListView, removing the need to even write a single line of code.

		// Test with BindingSource
		olvData.DataSource = new BindingSource(ds, "Person");// Test with DataTable//DataTable personTable = ds.Tables["Person"];//this.olvData.DataSource = personTable;// Test with DataView//DataTable personTable = ds.Tables["Person"];//this.olvData.DataSource = new DataView(personTable);// Test with DataSet//this.olvData.DataMember = "Person";//this.olvData.DataSource = ds;// Test with DataViewManager//this.olvData.DataMember = "Person";//this.olvData.DataSource = new DataViewManager(ds);// Test with nulls//this.olvData.DataMember = null;//this.olvData.DataSource = null;

	#region UI event handlers 

	private void buttonResetData_Click(object sender, EventArgs e)
	{
		Stopwatch stopWatch = Stopwatch.StartNew();

		try
		{
			Cursor = Cursors.WaitCursor;
			ReloadDataGridAndListView();
		}
		finally
		{
			stopWatch.Stop();
			Cursor = Cursors.Default;
		}

		Coordinator.ToolStripStatus1 =
			string.Format("XML Load: {0} items in {1}ms, average per item: {2:F}ms",
				olvData.Items.Count,
				stopWatch.ElapsedMilliseconds,
				stopWatch.ElapsedMilliseconds / olvData.Items.Count);
	}

	private void textBoxFilterData_TextChanged(object sender, EventArgs e) => Coordinator.TimedFilter(ListView, ((TextBox)sender).Text);

	private void checkBoxGroups_CheckedChanged(object sender, EventArgs e) => Coordinator.ShowGroupsChecked(ListView, (CheckBox)sender);

	private void checkBoxItemCounts_CheckedChanged(object sender, EventArgs e) => Coordinator.ShowLabelsOnGroupsChecked(ListView, (CheckBox)sender);

	private void checkBoxPause_CheckedChanged(object sender, EventArgs e) => ListView.PauseAnimations(((CheckBox)sender).Checked);

	private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e) => Coordinator.ChangeView(ListView, (ComboBox)sender);

	private void comboBoxEditable_SelectedIndexChanged(object sender, EventArgs e) => Coordinator.ChangeEditable(ListView, (ComboBox)sender);

	private void rowHeightUpDown_ValueChanged(object sender, EventArgs e) => olvData.RowHeight = Convert.ToInt32(rowHeightUpDown.Value);

	#endregion
}
