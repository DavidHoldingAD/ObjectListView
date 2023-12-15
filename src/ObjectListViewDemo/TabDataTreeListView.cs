using System.Data;

namespace ObjectListViewDemo;

public partial class TabDataTreeListView : OlvDemoTab
{

	public TabDataTreeListView()
	{
		InitializeComponent();
		ListView = olvDataTree;
	}

	protected override void InitializeTab()
	{

		// The whole point of a DataTreeListView is to write no code. So there is very little code here.

		// Put some images against each row
		olvColumn41.ImageGetter = delegate (object row)
		{ return "user"; };

		// The DataTreeListView needs to know the key that identifies root level objects.
		// DataTreeListView can handle that key being any data type, but the Designer only deals in strings.
		// Since we want a non-string value to identify keys, we have to set it explicitly here.
		olvDataTree.RootKeyValue = 0u;

		// Finally load the data into the UI
		LoadXmlIntoTreeDataListView();

		// This does a better job of auto sizing the columns
		olvDataTree.AutoResizeColumns();
	}

	private void LoadXmlIntoTreeDataListView()
	{
		DataSet ds = Coordinator.LoadDatasetFromXml(@"Data\FamilyTree.xml");

		if (ds.Tables.Count <= 0)
		{
			Coordinator.ShowMessage(@"Failed to load data set from Data\FamilyTree.xml");
			return;
		}

		dataGridView2.DataSource = ds;
		dataGridView2.DataMember = "Person";

		// Like DataListView, the DataTreeListView can handle binding to a variety of sources
		// And again, you could create a BindingSource in the designer, and assign that BindingSource
		// to DataSource, removing the need to even write these few lines of code.

		//this.olvDataTree.DataSource = new BindingSource(ds, "Person");
		//this.olvDataTree.DataSource = ds.Tables["Person"];
		//this.olvDataTree.DataSource = new DataView(ds.Tables["Person"]);
		//this.olvDataTree.DataMember = "Person"; this.olvDataTree.DataSource = ds;
		olvDataTree.DataMember = "Person";
		olvDataTree.DataSource = new DataViewManager(ds);
	}

	#region UI event handlers

	private void filterTextBox_TextChanged(object sender, EventArgs e) => Coordinator.TimedFilter(ListView, ((TextBox)sender).Text);

	private void buttonResetData_Click(object sender, EventArgs e) => LoadXmlIntoTreeDataListView();

	#endregion
}
