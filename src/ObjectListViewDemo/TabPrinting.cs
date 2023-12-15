using System.Drawing.Drawing2D;

using BrightIdeasSoftware;

namespace ObjectListViewDemo;

public partial class TabPrinting : OlvDemoTab
{
	public TabPrinting()
	{
		InitializeComponent();
	}

	// Public fields (yuck) to avoid the baggage of wrapping a private field.
	// Necessary until I stop supporting .Net 2.0.

	public ObjectListView SimpleView;
	public ObjectListView ComplexView;
	public ObjectListView FileExplorerView;
	public ObjectListView DataListView;
	public ObjectListView TreeListView;

	protected override void InitializeTab()
	{

		// listViewPrinter1 is created as a component in the designer.
		// Set the print preview control's Document property to be the listViewPrinter.

		// Listen to these events to give feedback as the printing happens
		listViewPrinter1.PrintPage += delegate (object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			Coordinator.ToolStripStatus1 = string.Format("Printing page #{0}...", listViewPrinter1.PageNumber);
		};

		listViewPrinter1.EndPrint += delegate (object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			Coordinator.ToolStripStatus1 = "Printing done";
		};

		// For some reason the Form Designer loses these settings
		printPreviewControl1.Zoom = 1;
		printPreviewControl1.AutoZoom = true;

		UpdatePrintPreview();
	}

	public void UpdatePrintPreview()
	{

		// Set the list view printer's ListView property to say which ObjectListView you want to print
		if (rbShowSimple.Checked == true)
		{
			listViewPrinter1.ListView = SimpleView;
		}
		else if (rbShowComplex.Checked == true)
		{
			listViewPrinter1.ListView = ComplexView;
		}
		else if (rbShowDataset.Checked == true)
		{
			listViewPrinter1.ListView = DataListView;
		}
		else if (rbShowTree.Checked == true)
		{
			listViewPrinter1.ListView = TreeListView;
		}
		else if (rbShowFileExplorer.Checked == true)
		{
			listViewPrinter1.ListView = FileExplorerView;
		}

		// Copy settings from UI onto the list view printer
		listViewPrinter1.DocumentName = tbTitle.Text;
		listViewPrinter1.Header = tbHeader.Text.Replace("\\t", "\t");
		listViewPrinter1.Footer = tbFooter.Text.Replace("\\t", "\t");
		listViewPrinter1.Watermark = tbWatermark.Text;
		listViewPrinter1.IsShrinkToFit = cbShrinkToFit.Checked;
		listViewPrinter1.IsTextOnly = !cbIncludeImages.Checked;
		listViewPrinter1.IsPrintSelectionOnly = cbPrintOnlySelection.Checked;
		listViewPrinter1.FirstPage = (int)numericFrom.Value;
		listViewPrinter1.LastPage = (int)numericTo.Value;

		// Give the list view printer the appropriate styling
		if (rbStyleMinimal.Checked)
		{
			ApplyMinimalFormatting();
		}
		else if (rbStyleModern.Checked)
		{
			ApplyModernFormatting();
		}
		else if (rbStyleTooMuch.Checked)
		{
			ApplyOverTheTopFormatting();
		}

		if (cbCellGridLines.Checked == false)
		{
			listViewPrinter1.ListGridPen = null;
		}

		// Finally, tell the print preview to redraw itself

		printPreviewControl1.InvalidatePreview();
	}

	private void ApplyMinimalFormatting()
	{
		listViewPrinter1.CellFormat = null;
		listViewPrinter1.ListFont = new Font("Tahoma", 9);

		listViewPrinter1.HeaderFormat = BlockFormat.Header();
		listViewPrinter1.HeaderFormat.TextBrush = Brushes.Black;
		listViewPrinter1.HeaderFormat.BackgroundBrush = null;
		listViewPrinter1.HeaderFormat.SetBorderPen(Sides.Bottom, new Pen(Color.Black, 0.5f));

		listViewPrinter1.FooterFormat = BlockFormat.Footer();
		listViewPrinter1.GroupHeaderFormat = BlockFormat.GroupHeader();
		Brush brush = new LinearGradientBrush(new Point(0, 0), new Point(200, 0), Color.Gray, Color.White);
		listViewPrinter1.GroupHeaderFormat.SetBorder(Sides.Bottom, 2, brush);

		listViewPrinter1.ListHeaderFormat = BlockFormat.ListHeader();
		listViewPrinter1.ListHeaderFormat.BackgroundBrush = null;

		listViewPrinter1.WatermarkFont = null;
		listViewPrinter1.WatermarkColor = Color.Empty;
	}

	private void ApplyModernFormatting()
	{
		listViewPrinter1.CellFormat = null;
		listViewPrinter1.ListFont = new Font("Ms Sans Serif", 9);
		listViewPrinter1.ListGridPen = new Pen(Color.DarkGray, 0.5f);

		listViewPrinter1.HeaderFormat = BlockFormat.Header(new Font("Verdana", 24, FontStyle.Bold));
		listViewPrinter1.HeaderFormat.BackgroundBrush = new LinearGradientBrush(new Point(0, 0), new Point(200, 0), Color.DarkBlue, Color.White);

		listViewPrinter1.FooterFormat = BlockFormat.Footer();
		listViewPrinter1.FooterFormat.BackgroundBrush = new LinearGradientBrush(new Point(0, 0), new Point(200, 0), Color.White, Color.Blue);

		listViewPrinter1.GroupHeaderFormat = BlockFormat.GroupHeader();
		listViewPrinter1.ListHeaderFormat = BlockFormat.ListHeader(new Font("Verdana", 12));

		listViewPrinter1.WatermarkFont = null;
		listViewPrinter1.WatermarkColor = Color.Empty;
	}

	private void ApplyOverTheTopFormatting()
	{
		listViewPrinter1.CellFormat = null;
		listViewPrinter1.ListFont = new Font("Ms Sans Serif", 9);
		listViewPrinter1.ListGridPen = new Pen(Color.Blue, 0.5f);

		listViewPrinter1.HeaderFormat = BlockFormat.Header(new Font("Comic Sans MS", 36));
		listViewPrinter1.HeaderFormat.TextBrush = new LinearGradientBrush(new Point(0, 0), new Point(900, 0), Color.Black, Color.Blue);
		listViewPrinter1.HeaderFormat.BackgroundBrush = new TextureBrush(Resource1.star16, WrapMode.Tile);
		listViewPrinter1.HeaderFormat.SetBorder(Sides.All, 10, new LinearGradientBrush(new Point(0, 0), new Point(300, 0), Color.Purple, Color.Pink));

		listViewPrinter1.FooterFormat = BlockFormat.Footer(new Font("Comic Sans MS", 12));
		listViewPrinter1.FooterFormat.TextBrush = Brushes.Blue;
		listViewPrinter1.FooterFormat.BackgroundBrush = new LinearGradientBrush(new Point(0, 0), new Point(200, 0), Color.Gold, Color.Green);
		listViewPrinter1.FooterFormat.SetBorderPen(Sides.All, new Pen(Color.FromArgb(128, Color.Green), 5));

		listViewPrinter1.GroupHeaderFormat = BlockFormat.GroupHeader();
		Brush brush = new HatchBrush(HatchStyle.LargeConfetti, Color.Blue, Color.Empty);
		listViewPrinter1.GroupHeaderFormat.SetBorder(Sides.Bottom, 5, brush);

		listViewPrinter1.ListHeaderFormat = BlockFormat.ListHeader(new Font("Comic Sans MS", 12));
		listViewPrinter1.ListHeaderFormat.BackgroundBrush = Brushes.PowderBlue;
		listViewPrinter1.ListHeaderFormat.TextBrush = Brushes.Black;

		listViewPrinter1.WatermarkFont = new Font("Comic Sans MS", 72);
		listViewPrinter1.WatermarkColor = Color.Red;
	}

	private void buttonPageSetup_Click(object sender, EventArgs e) => listViewPrinter1.PageSetup();

	private void buttonPreview_Click(object sender, EventArgs e) => listViewPrinter1.PrintPreview();

	private void buttonPrint_Click(object sender, EventArgs e) => listViewPrinter1.PrintWithDialog();

	private void rbShowSimple_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void rbShowComplex_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void rbShowDataset_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void rbShowFileExplorer_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void rbStyleMinimal_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void rbStyleModern_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void rbStyleTooMuch_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void cbIncludeImages_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void cbShrinkToFit_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void cbPrintOnlySelection_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void cbCellGridLines_CheckedChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void numericFrom_ValueChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void numericTo_ValueChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void tbTitle_TextChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void tbHeader_TextChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void tbFooter_TextChanged(object sender, EventArgs e) => UpdatePrintPreview();

	private void tbWatermark_TextChanged(object sender, EventArgs e) => UpdatePrintPreview();
}
