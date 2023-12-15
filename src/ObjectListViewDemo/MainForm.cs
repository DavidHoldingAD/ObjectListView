/*
 * ObjectListViewDemo - A simple demo to show the ObjectListView control
 *
 * User: Phillip Piper
 * Date: 15/10/2006 11:15 AM
 *
 * Change log:
 * 2015-06-12  JPP  COMPLETE REWRITE. Goal of rewrite is to make the code much easier to follow
 * 
 * 2009-07-04  JPP  Added ExampleVirtualDataSource for virtual list demo
 * [lots of stuff]
 * 2006-10-20  JPP  Added DataSet tab page
 * 2006-10-15  JPP  Initial version
 */

using BrightIdeasSoftware;

namespace ObjectListViewDemo;


public partial class MainForm
{

	[STAThread]
	public static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.Run(new MainForm());
	}

	/// <summary>
	///
	/// </summary>
	public MainForm()
	{
		//
		// The InitializeComponent() call is required for Windows Forms designer support.
		//
		InitializeComponent();
		InitializeExamples();
	}

	private void InitializeExamples()
	{
		// Use different font under Vista
		if (ObjectListView.IsVistaOrLater)
		{
			Font = new Font("Segoe UI", 9);
		}

		OLVDemoCoordinator coordinator = new(this);

		tabSimple.Coordinator = coordinator;
		tabComplex.Coordinator = coordinator;
		tabDataSet.Coordinator = coordinator;
		tabFileExplorer1.Coordinator = coordinator;
		tabFastList1.Coordinator = coordinator;
		tabTreeListView1.Coordinator = coordinator;
		tabDataTreeListView1.Coordinator = coordinator;
		tabDragAndDrop1.Coordinator = coordinator;
		tabDescribedTask1.Coordinator = coordinator;

		// Printing tab is slightly different, since it needs to know about the ObjectListViews from the other tabs
		tabPrinting1.SimpleView = tabSimple.ListView;
		tabPrinting1.ComplexView = tabComplex.ListView;
		tabPrinting1.DataListView = tabDataSet.ListView;
		tabPrinting1.FileExplorerView = tabFileExplorer1.ListView;
		tabPrinting1.TreeListView = tabTreeListView1.ListView;
		tabPrinting1.Coordinator = coordinator;

		//this.tabControl1.SelectTab(this.tabDescribedTasks);
	}

	private void tabControl1_Selected(object sender, TabControlEventArgs e)
	{
		if (tabControl1.TabPages[e.TabPageIndex].Name == "tabPagePrinting")
		{
			tabPrinting1.UpdatePrintPreview();
		}
	}
}
