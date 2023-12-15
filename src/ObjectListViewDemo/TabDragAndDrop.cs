using BrightIdeasSoftware;

namespace ObjectListViewDemo;

public partial class TabDragAndDrop : OlvDemoTab
{
	public TabDragAndDrop()
	{
		InitializeComponent();
	}

	protected override void InitializeTab()
	{

		SetupColumns();
		SetupDragAndDrop();

		comboBoxGeeksAndTweebsView.SelectedIndex = 4;
		comboBoxCoolFroodsView.SelectedIndex = 4;

		olvGeeks.SetObjects(Coordinator.PersonList);
	}

	private void SetupColumns()
	{
		olvGeeks.GetColumn(0).ImageGetter = delegate (object x)
		{ return "user"; };
		olvFroods.GetColumn(0).ImageGetter = delegate (object x)
		{ return "user"; };

		olvGeeks.GetColumn(2).Renderer = new MultiImageRenderer(Resource1.star16, 5, 0, 40);
		olvFroods.GetColumn(2).Renderer = new MultiImageRenderer(Resource1.star16, 5, 0, 40);
	}

	private void SetupDragAndDrop()
	{

		// Make each listview capable of dragging rows out
		olvGeeks.DragSource = new SimpleDragSource();
		olvFroods.DragSource = new SimpleDragSource();

		// Make each listview capable of accepting drops.
		// More than that, make it so it's items can be rearranged
		olvGeeks.DropSink = new RearrangingDropSink(true);
		olvFroods.DropSink = new RearrangingDropSink(true);

		// For a normal drag and drop situation, you will need to create a SimpleDropSink
		// and then listen for ModelCanDrop and ModelDropped events
	}

	#region UI event handlers

	private void comboBoxGeeksAndTweebsView_SelectedIndexChanged(object sender, EventArgs e) => Coordinator.ChangeView(olvGeeks, (ComboBox)sender);

	private void comboBoxCoolFroodsView_SelectedIndexChanged(object sender, EventArgs e) => Coordinator.ChangeView(olvFroods, (ComboBox)sender);

	#endregion
}
