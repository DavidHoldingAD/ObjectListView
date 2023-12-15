using System.ComponentModel;

using BrightIdeasSoftware;

namespace ObjectListViewDemo;

public class OlvDemoTab : UserControl
{

	[Browsable(false),
	 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public OLVDemoCoordinator Coordinator
	{
		get { return coordinator; }
		set
		{
			coordinator = value;
			if (value != null)
			{
				InitializeTab();
				SetupGeneralListViewEvents();
			}
		}
	}
	private OLVDemoCoordinator coordinator;

	protected virtual void InitializeTab() { }

	public ObjectListView ListView { get; protected set; }

	private void SetupGeneralListViewEvents()
	{
		if (ListView == null || Coordinator == null)
		{
			return;
		}

		ListView.SelectionChanged += delegate (object sender, EventArgs args)
		{
			Coordinator.HandleSelectionChanged(ListView);
		};

		ListView.HotItemChanged += delegate (object sender, HotItemChangedEventArgs args)
		{
			Coordinator.HandleHotItemChanged(sender, args);
		};

		ListView.GroupTaskClicked += delegate (object sender, GroupTaskClickedEventArgs args)
		{
			Coordinator.ShowMessage("Clicked on group task: " + args.Group.Name);
		};

		ListView.GroupStateChanged += delegate (object sender, GroupStateChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(string.Format("Group '{0}' was {1}{2}{3}{4}{5}{6}",
				e.Group.Header,
				e.Selected ? "Selected" : "",
				e.Focused ? "Focused" : "",
				e.Collapsed ? "Collapsed" : "",
				e.Unselected ? "Unselected" : "",
				e.Unfocused ? "Unfocused" : "",
				e.Uncollapsed ? "Uncollapsed" : ""));
		};
	}
}