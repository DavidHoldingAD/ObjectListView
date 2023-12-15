using System.Diagnostics;

using BrightIdeasSoftware;

namespace ObjectListViewDemo;

public partial class TabFileExplorer : OlvDemoTab
{

	public TabFileExplorer()
	{
		InitializeComponent();
		ListView = olvFiles;
	}

	protected override void InitializeTab()
	{
		SetupControls();

		SetupList();
		SetupColumns();

		PopulateListFromPath(textBoxFolderPath.Text);
	}

	private void SetupControls()
	{
		// Setup initial state of controls on tab
		if (ObjectListView.IsVistaOrLater)
		{
			comboBoxHotItemStyle.Items.Add("Vista");
		}

		comboBoxView.SelectedIndex = 4; // Details
		comboBoxHotItemStyle.SelectedIndex = 0; // None
		comboBoxNagLevel.SelectedIndex = 0; // Slight
		textBoxFolderPath.Text = @"c:\";
	}

	private void SetupList()
	{
		// We want to draw the system icon against each file name. SysImageListHelper does this work for us.

		SysImageListHelper helper = new(olvFiles);
		olvColumnName.ImageGetter = delegate (object x)
		{ return helper.GetImageIndex(((FileSystemInfo)x).FullName); };

		// Show tooltips when the appropriate checkbox is clicked
		olvFiles.ShowItemToolTips = true;
		olvFiles.CellToolTipShowing += delegate (object sender, ToolTipShowingEventArgs e)
		{
			if (showToolTipsOnFiles)
			{
				e.Text = string.Format("Tool tip for '{0}', column '{1}'\r\nValue shown: '{2}'", e.Model, e.Column.Text, e.SubItem.Text);
			}
		};

		// Show a menu -- but only when the user right clicks on the first column
		olvFiles.CellRightClick += delegate (object sender, CellRightClickEventArgs e)
		{
			System.Diagnostics.Trace.WriteLine(string.Format("right clicked {0}, {1}). model {2}", e.RowIndex, e.ColumnIndex, e.Model));
			if (e.ColumnIndex == 0)
			{
				e.MenuStrip = contextMenuStrip2;
			}
		};
	}

	private void SetupColumns()
	{
		// Get the size of the file system entity. 
		// Folders and errors are represented as negative numbers
		olvColumnSize.AspectGetter = delegate (object x)
		{
			if (x is DirectoryInfo)
			{
				return (long)-1;
			}

			try
			{
				return ((FileInfo)x).Length;
			}
			catch (System.IO.FileNotFoundException)
			{
				// Mono 1.2.6 throws this for hidden files
				return (long)-2;
			}
		};

		// Show the size of files as GB, MB and KBs. By returning the actual
		// size in the AspectGetter, and doing the conversion in the 
		// AspectToStringConverter, sorting on this column will work off the
		// actual sizes, rather than the formatted string.
		olvColumnSize.AspectToStringConverter = delegate (object x)
		{
			long sizeInBytes = (long)x;
			if (sizeInBytes < 0) // folder or error
			{
				return "";
			}

			return Coordinator.FormatFileSize(sizeInBytes);
		};
		olvColumnSize.MakeGroupies(new long[] { 0, 1024 * 1024, 512 * 1024 * 1024 },
			new string[] { "Folders", "Small", "Big", "Disk space chewer" });

		// Group by month-year, rather than date
		// This code is duplicated for FileCreated and FileModified, so we really should
		// create named methods rather than using anonymous delegates.
		olvColumnCreated.GroupKeyGetter = delegate (object x)
		{
			DateTime dt = ((FileSystemInfo)x).CreationTime;
			return new DateTime(dt.Year, dt.Month, 1);
		};
		olvColumnCreated.GroupKeyToTitleConverter = delegate (object x)
		{
			return ((DateTime)x).ToString("MMMM yyyy");
		};

		// Group by month-year, rather than date
		olvColumnModified.GroupKeyGetter = delegate (object x)
		{
			DateTime dt = ((FileSystemInfo)x).LastWriteTime;
			return new DateTime(dt.Year, dt.Month, 1);
		};
		olvColumnModified.GroupKeyToTitleConverter = delegate (object x)
		{
			return ((DateTime)x).ToString("MMMM yyyy");
		};

		// Show the system description for this object
		olvColumnFileType.AspectGetter = delegate (object x)
		{
			return ShellUtilities.GetFileType(((FileSystemInfo)x).FullName);
		};

		// Show the file attributes for this object
		// A FlagRenderer masks off various values and draws zero or more images based 
		// on the presence of individual bits.
		olvColumnAttributes.AspectGetter = delegate (object x)
		{
			return ((FileSystemInfo)x).Attributes;
		};
		FlagRenderer attributesRenderer = new();
		attributesRenderer.ImageList = imageList1;
		attributesRenderer.Add(FileAttributes.Archive, "archive");
		attributesRenderer.Add(FileAttributes.ReadOnly, "readonly");
		attributesRenderer.Add(FileAttributes.System, "system");
		attributesRenderer.Add(FileAttributes.Hidden, "hidden");
		attributesRenderer.Add(FileAttributes.Temporary, "temporary");
		olvColumnAttributes.Renderer = attributesRenderer;

		// Tell the filtering subsystem that the attributes column is a collection of flags
		olvColumnAttributes.ClusteringStrategy = new FlagClusteringStrategy(typeof(FileAttributes));
	}

	private void PopulateListFromPath(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}

		DirectoryInfo pathInfo = new(path);
		if (!pathInfo.Exists)
		{
			return;
		}

		Stopwatch sw = new();

		Cursor.Current = Cursors.WaitCursor;
		sw.Start();
		olvFiles.SetObjects(pathInfo.GetFileSystemInfos());
		sw.Stop();
		Cursor.Current = Cursors.Default;

		float msPerItem = (olvFiles.Items.Count == 0 ? 0 : (float)sw.ElapsedMilliseconds / olvFiles.Items.Count);
		Coordinator.ToolStripStatus1 = string.Format("Timed build: {0} items in {1}ms ({2:F}ms per item)",
			olvFiles.Items.Count, sw.ElapsedMilliseconds, msPerItem);
	}

	#region UI event handlers

	private void olvFiles_ItemActivate(object sender, EventArgs e)
	{
		object rowObject = olvFiles.SelectedObject;
		if (rowObject == null)
		{
			return;
		}

		if (rowObject is DirectoryInfo)
		{
			textBoxFolderPath.Text = ((DirectoryInfo)rowObject).FullName;
			buttonGo.PerformClick();
		}
		else
		{
			ShellUtilities.Execute(((FileInfo)rowObject).FullName);
		}
	}

	private void textBoxFolderPath_TextChanged(object sender, EventArgs e)
	{
		if (Directory.Exists(textBoxFolderPath.Text))
		{
			textBoxFolderPath.ForeColor = Color.Black;
			buttonGo.Enabled = true;
			buttonUp.Enabled = true;
		}
		else
		{
			textBoxFolderPath.ForeColor = Color.Red;
			buttonGo.Enabled = false;
			buttonUp.Enabled = false;
		}
	}

	private void textBoxFolderPath_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == (char)13)
		{
			buttonGo.PerformClick();
			e.Handled = true;
		}
	}

	private void buttonGo_Click(object sender, EventArgs e)
	{
		string path = textBoxFolderPath.Text;
		PopulateListFromPath(path);
	}

	private void buttonUp_Click(object sender, EventArgs e)
	{
		DirectoryInfo di = Directory.GetParent(textBoxFolderPath.Text);
		if (di == null)
		{
			System.Media.SystemSounds.Asterisk.Play();
		}
		else
		{
			textBoxFolderPath.Text = di.FullName;
			buttonGo.PerformClick();
		}
	}

	private void comboBoxNagLevel_SelectedIndexChanged(object sender, EventArgs e)
	{
		ListView.RemoveOverlay(nagOverlay);

		nagOverlay = new TextOverlay();
		switch (comboBoxNagLevel.SelectedIndex)
		{
			case 0:
				nagOverlay.Alignment = ContentAlignment.BottomRight;
				nagOverlay.Text = "Trial version";
				nagOverlay.BackColor = Color.White;
				nagOverlay.BorderWidth = 2.0f;
				nagOverlay.BorderColor = Color.RoyalBlue;
				nagOverlay.TextColor = Color.DarkBlue;
				ListView.OverlayTransparency = 255;
				break;
			case 1:
				nagOverlay.Alignment = ContentAlignment.TopRight;
				nagOverlay.Text = "TRIAL VERSION EXPIRED";
				nagOverlay.TextColor = Color.Red;
				nagOverlay.BackColor = Color.White;
				nagOverlay.BorderWidth = 2.0f;
				nagOverlay.BorderColor = Color.DarkGray;
				nagOverlay.Rotation = 20;
				nagOverlay.InsetX = 5;
				nagOverlay.InsetY = 50;
				ListView.OverlayTransparency = 192;
				break;
			case 2:
				nagOverlay.Alignment = ContentAlignment.MiddleCenter;
				nagOverlay.Text = "TRIAL EXPIRED! BUY NOW!";
				nagOverlay.TextColor = Color.Red;
				nagOverlay.BorderWidth = 4.0f;
				nagOverlay.BorderColor = Color.Red;
				nagOverlay.Rotation = -30;
				nagOverlay.Font = new Font("Stencil", 36);
				ListView.OverlayTransparency = 192;
				break;
		}
		ListView.AddOverlay(nagOverlay);
	}

	private TextOverlay nagOverlay;

	private void checkBoxGroups_CheckedChanged(object sender, EventArgs e) => Coordinator.ShowGroupsChecked(ListView, (CheckBox)sender);

	private void checkBoxTooltips_CheckedChanged(object sender, EventArgs e) => showToolTipsOnFiles = !showToolTipsOnFiles;

	private bool showToolTipsOnFiles = false;

	private void comboBoxHotItemStyle_SelectedIndexChanged(object sender, EventArgs e) => Coordinator.ChangeHotItemStyle(ListView, (ComboBox)sender);

	private void comboBoxView_SelectedIndexChanged(object sender, EventArgs e) => Coordinator.ChangeView(ListView, (ComboBox)sender);

	private void buttonSaveState_Click(object sender, EventArgs e)
	{
		// SaveState() returns a byte array that holds the current state of the columns.
		// For this demo, we just hold onto that value in an instance variable. For your
		// application, you should persist it some more permanent fashion than this.
		fileListViewState = olvFiles.SaveState();
		buttonRestoreState.Enabled = true;
	}

	private byte[] fileListViewState;

	private void buttonRestoreState_Click(object sender, EventArgs e) =>
		// Restore the state is just a single call
		olvFiles.RestoreState(fileListViewState);

	private void buttonColumns_Click(object sender, EventArgs e)
	{
		ColumnSelectionForm form = new();
		form.OpenOn(olvFiles);
	}

	#endregion
}
