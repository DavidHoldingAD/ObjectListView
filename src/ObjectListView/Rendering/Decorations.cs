﻿/*
 * Decorations - Images, text or other things that can be rendered onto an ObjectListView
 *
 * Author: Phillip Piper
 * Date: 19/08/2009 10:56 PM
 *
 * Change log:
 * 2011-04-04   JPP  - Added ability to have a gradient background on BorderDecoration
 * v2.4
 * 2010-04-15   JPP  - Tweaked LightBoxDecoration a little
 * v2.3
 * 2009-09-23   JPP  - Added LeftColumn and RightColumn to RowBorderDecoration
 * 2009-08-23   JPP  - Added LightBoxDecoration
 * 2009-08-19   JPP  - Initial version. Separated from Overlays.cs
 *
 * To do:
 *
 * Copyright (C) 2009-2014 Phillip Piper
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

using System.Drawing.Drawing2D;

namespace BrightIdeasSoftware;

/// <summary>
/// A decoration is an overlay that draws itself in relation to a given row or cell.
/// Decorations scroll when the listview scrolls.
/// </summary>
public interface IDecoration : IOverlay
{
	/// <summary>
	/// Gets or sets the row that is to be decorated
	/// </summary>
	OLVListItem ListItem { get; set; }

	/// <summary>
	/// Gets or sets the subitem that is to be decorated
	/// </summary>
	OLVListSubItem SubItem { get; set; }
}

/// <summary>
/// An AbstractDecoration is a safe do-nothing implementation of the IDecoration interface
/// </summary>
public class AbstractDecoration : IDecoration
{
	#region IDecoration Members

	/// <summary>
	/// Gets or sets the row that is to be decorated
	/// </summary>
	public OLVListItem ListItem { get; set; }

	/// <summary>
	/// Gets or sets the subitem that is to be decorated
	/// </summary>
	public OLVListSubItem SubItem { get; set; }

	#endregion

	#region Public properties

	/// <summary>
	/// Gets the bounds of the decorations row
	/// </summary>
	public Rectangle RowBounds
	{
		get
		{
			if (ListItem == null)
			{
				return Rectangle.Empty;
			}
			else
			{
				return ListItem.Bounds;
			}
		}
	}

	/// <summary>
	/// Get the bounds of the decorations cell
	/// </summary>
	public Rectangle CellBounds
	{
		get
		{
			if (ListItem == null || SubItem == null)
			{
				return Rectangle.Empty;
			}
			else
			{
				return ListItem.GetSubItemBounds(ListItem.SubItems.IndexOf(SubItem));
			}
		}
	}

	#endregion

	#region IOverlay Members

	/// <summary>
	/// Draw the decoration
	/// </summary>
	/// <param name="olv"></param>
	/// <param name="g"></param>
	/// <param name="r"></param>
	public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r)
	{
	}

	#endregion
}

/// <summary>
/// This decoration draws a slight tint over a column of the
/// owning listview. If no column is explicitly set, the selected
/// column in the listview will be used.
/// The selected column is normally the sort column, but does not have to be.
/// </summary>
public class TintedColumnDecoration : AbstractDecoration
{
	#region Constructors

	/// <summary>
	/// Create a TintedColumnDecoration
	/// </summary>
	public TintedColumnDecoration()
	{
		Tint = Color.FromArgb(15, Color.Blue);
	}

	/// <summary>
	/// Create a TintedColumnDecoration
	/// </summary>
	/// <param name="column"></param>
	public TintedColumnDecoration(OLVColumn column)
		: this()
	{
		ColumnToTint = column;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets or sets the column that will be tinted
	/// </summary>
	public OLVColumn ColumnToTint { get; set; }

	/// <summary>
	/// Gets or sets the color that will be 'tinted' over the selected column
	/// </summary>
	public Color Tint
	{
		get { return tint; }
		set
		{
			if (tint == value)
			{
				return;
			}

			if (tintBrush != null)
			{
				tintBrush.Dispose();
				tintBrush = null;
			}

			tint = value;
			tintBrush = new SolidBrush(tint);
		}
	}
	private Color tint;
	private SolidBrush tintBrush;

	#endregion

	#region IOverlay Members

	/// <summary>
	/// Draw a slight colouring over our tinted column
	/// </summary>
	/// <remarks>
	/// This overlay only works when:
	/// - the list is in Details view
	/// - there is at least one row
	/// - there is a selected column (or a specified tint column)
	/// </remarks>
	/// <param name="olv"></param>
	/// <param name="g"></param>
	/// <param name="r"></param>
	public override void Draw(ObjectListView olv, Graphics g, Rectangle r)
	{

		if (olv.View != System.Windows.Forms.View.Details)
		{
			return;
		}

		if (olv.GetItemCount() == 0)
		{
			return;
		}

		OLVColumn column = ColumnToTint ?? olv.SelectedColumn;
		if (column == null)
		{
			return;
		}

		Point sides = NativeMethods.GetScrolledColumnSides(olv, column.Index);
		if (sides.X == -1)
		{
			return;
		}

		Rectangle columnBounds = new(sides.X, r.Top, sides.Y - sides.X, r.Bottom);

		// Find the bottom of the last item. The tinting should extend only to there.
		OLVListItem lastItem = olv.GetLastItemInDisplayOrder();
		if (lastItem != null)
		{
			Rectangle lastItemBounds = lastItem.Bounds;
			if (!lastItemBounds.IsEmpty && lastItemBounds.Bottom < columnBounds.Bottom)
			{
				columnBounds.Height = lastItemBounds.Bottom - columnBounds.Top;
			}
		}
		g.FillRectangle(tintBrush, columnBounds);
	}

	#endregion
}

/// <summary>
/// This decoration draws an optionally filled border around a rectangle.
/// Subclasses must override CalculateBounds().
/// </summary>
public class BorderDecoration : AbstractDecoration
{
	#region Constructors

	/// <summary>
	/// Create a BorderDecoration
	/// </summary>
	public BorderDecoration()
		: this(new Pen(Color.FromArgb(64, Color.Blue), 1))
	{
	}

	/// <summary>
	/// Create a BorderDecoration
	/// </summary>
	/// <param name="borderPen">The pen used to draw the border</param>
	public BorderDecoration(Pen borderPen)
	{
		BorderPen = borderPen;
	}

	/// <summary>
	/// Create a BorderDecoration
	/// </summary>
	/// <param name="borderPen">The pen used to draw the border</param>
	/// <param name="fill">The brush used to fill the rectangle</param>
	public BorderDecoration(Pen borderPen, Brush fill)
	{
		BorderPen = borderPen;
		FillBrush = fill;
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets or sets the pen that will be used to draw the border
	/// </summary>
	public Pen BorderPen { get; set; }

	/// <summary>
	/// Gets or sets the padding that will be added to the bounds of the item
	/// before drawing the border and fill.
	/// </summary>
	public Size BoundsPadding
	{
		get { return boundsPadding; }
		set { boundsPadding = value; }
	}
	private Size boundsPadding = new(-1, 2);

	/// <summary>
	/// How rounded should the corners of the border be? 0 means no rounding.
	/// </summary>
	/// <remarks>If this value is too large, the edges of the border will appear odd.</remarks>
	public float CornerRounding { get; set; } = 16.0f;

	/// <summary>
	/// Gets or sets the brush that will be used to fill the border
	/// </summary>
	/// <remarks>This value is ignored when using gradient brush</remarks>
	public Brush FillBrush { get; set; } = new SolidBrush(Color.FromArgb(64, Color.Blue));

	/// <summary>
	/// Gets or sets the color that will be used as the start of a gradient fill.
	/// </summary>
	/// <remarks>This and FillGradientTo must be given value to show a gradient</remarks>
	public Color? FillGradientFrom { get; set; }

	/// <summary>
	/// Gets or sets the color that will be used as the end of a gradient fill.
	/// </summary>
	/// <remarks>This and FillGradientFrom must be given value to show a gradient</remarks>
	public Color? FillGradientTo { get; set; }

	/// <summary>
	/// Gets or sets the fill mode that will be used for the gradient.
	/// </summary>
	public LinearGradientMode FillGradientMode { get; set; } = LinearGradientMode.Vertical;

	#endregion

	#region IOverlay Members

	/// <summary>
	/// Draw a filled border 
	/// </summary>
	/// <param name="olv"></param>
	/// <param name="g"></param>
	/// <param name="r"></param>
	public override void Draw(ObjectListView olv, Graphics g, Rectangle r)
	{
		Rectangle bounds = CalculateBounds();
		if (!bounds.IsEmpty)
		{
			DrawFilledBorder(g, bounds);
		}
	}

	#endregion

	#region Subclass responsibility

	/// <summary>
	/// Subclasses should override this to say where the border should be drawn
	/// </summary>
	/// <returns></returns>
	protected virtual Rectangle CalculateBounds() => Rectangle.Empty;

	#endregion

	#region Implementation utlities

	/// <summary>
	/// Do the actual work of drawing the filled border
	/// </summary>
	/// <param name="g"></param>
	/// <param name="bounds"></param>
	protected void DrawFilledBorder(Graphics g, Rectangle bounds)
	{
		bounds.Inflate(BoundsPadding);
		GraphicsPath path = GetRoundedRect(bounds, CornerRounding);
		if (FillGradientFrom != null && FillGradientTo != null)
		{
			if (FillBrush != null)
			{
				FillBrush.Dispose();
			}

			FillBrush = new LinearGradientBrush(bounds, FillGradientFrom.Value, FillGradientTo.Value, FillGradientMode);
		}
		if (FillBrush != null)
		{
			g.FillPath(FillBrush, path);
		}

		if (BorderPen != null)
		{
			g.DrawPath(BorderPen, path);
		}
	}

	/// <summary>
	/// Create a GraphicsPath that represents a round cornered rectangle.
	/// </summary>
	/// <param name="rect"></param>
	/// <param name="diameter">If this is 0 or less, the rectangle will not be rounded.</param>
	/// <returns></returns>
	protected GraphicsPath GetRoundedRect(RectangleF rect, float diameter)
	{
		GraphicsPath path = new();

		if (diameter <= 0.0f)
		{
			path.AddRectangle(rect);
		}
		else
		{
			RectangleF arc = new(rect.X, rect.Y, diameter, diameter);
			path.AddArc(arc, 180, 90);
			arc.X = rect.Right - diameter;
			path.AddArc(arc, 270, 90);
			arc.Y = rect.Bottom - diameter;
			path.AddArc(arc, 0, 90);
			arc.X = rect.Left;
			path.AddArc(arc, 90, 90);
			path.CloseFigure();
		}

		return path;
	}

	#endregion
}

/// <summary>
/// Instances of this class draw a border around the decorated row
/// </summary>
public class RowBorderDecoration : BorderDecoration
{
	/// <summary>
	/// Gets or sets the index of the left most column to be used for the border
	/// </summary>
	public int LeftColumn { get; set; } = -1;

	/// <summary>
	/// Gets or sets the index of the right most column to be used for the border
	/// </summary>
	public int RightColumn { get; set; } = -1;

	/// <summary>
	/// Calculate the boundaries of the border
	/// </summary>
	/// <returns></returns>
	protected override Rectangle CalculateBounds()
	{
		Rectangle bounds = RowBounds;
		if (ListItem == null)
		{
			return bounds;
		}

		if (LeftColumn >= 0)
		{
			Rectangle leftCellBounds = ListItem.GetSubItemBounds(LeftColumn);
			if (!leftCellBounds.IsEmpty)
			{
				bounds.Width = bounds.Right - leftCellBounds.Left;
				bounds.X = leftCellBounds.Left;
			}
		}

		if (RightColumn >= 0)
		{
			Rectangle rightCellBounds = ListItem.GetSubItemBounds(RightColumn);
			if (!rightCellBounds.IsEmpty)
			{
				bounds.Width = rightCellBounds.Right - bounds.Left;
			}
		}

		return bounds;
	}
}

/// <summary>
/// Instances of this class draw a border around the decorated subitem.
/// </summary>
public class CellBorderDecoration : BorderDecoration
{
	/// <summary>
	/// Calculate the boundaries of the border
	/// </summary>
	/// <returns></returns>
	protected override Rectangle CalculateBounds() => CellBounds;
}

/// <summary>
/// This decoration puts a border around the cell being edited and
/// optionally "lightboxes" the cell (makes the rest of the control dark).
/// </summary>
public class EditingCellBorderDecoration : BorderDecoration
{
	#region Life and death

	/// <summary>
	/// Create a EditingCellBorderDecoration
	/// </summary>
	public EditingCellBorderDecoration()
	{
		FillBrush = null;
		BorderPen = new Pen(Color.DarkBlue, 2);
		CornerRounding = 8;
		BoundsPadding = new Size(10, 8);

	}

	/// <summary>
	/// Create a EditingCellBorderDecoration
	/// </summary>
	/// <param name="useLightBox">Should the decoration use a lighbox display style?</param>
	public EditingCellBorderDecoration(bool useLightBox) : this()
	{
		UseLightbox = useLightbox;
	}

	#endregion

	#region Configuration properties

	/// <summary>
	/// Gets or set whether the decoration should make the rest of
	/// the control dark when a cell is being edited
	/// </summary>
	/// <remarks>If this is true, FillBrush is used to overpaint
	/// the control.</remarks>
	public bool UseLightbox
	{
		get { return useLightbox; }
		set
		{
			if (useLightbox == value)
			{
				return;
			}

			useLightbox = value;
			if (useLightbox)
			{
				if (FillBrush == null)
				{
					FillBrush = new SolidBrush(Color.FromArgb(64, Color.Black));
				}
			}
		}
	}
	private bool useLightbox;

	#endregion

	#region Implementation

	/// <summary>
	/// Draw the decoration
	/// </summary>
	/// <param name="olv"></param>
	/// <param name="g"></param>
	/// <param name="r"></param>
	public override void Draw(ObjectListView olv, Graphics g, Rectangle r)
	{
		if (!olv.IsCellEditing)
		{
			return;
		}

		Rectangle bounds = olv.CellEditor.Bounds;
		if (bounds.IsEmpty)
		{
			return;
		}

		bounds.Inflate(BoundsPadding);
		GraphicsPath path = GetRoundedRect(bounds, CornerRounding);
		if (FillBrush != null)
		{
			if (UseLightbox)
			{
				using (Region newClip = new(r))
				{
					newClip.Exclude(path);
					Region originalClip = g.Clip;
					g.Clip = newClip;
					g.FillRectangle(FillBrush, r);
					g.Clip = originalClip;
				}
			}
			else
			{
				g.FillPath(FillBrush, path);
			}
		}
		if (BorderPen != null)
		{
			g.DrawPath(BorderPen, path);
		}
	}

	#endregion
}

/// <summary>
/// This decoration causes everything *except* the row under the mouse to be overpainted
/// with a tint, making the row under the mouse stand out in comparison.
/// The darker and more opaque the fill color, the more obvious the
/// decorated row becomes.
/// </summary>
public class LightBoxDecoration : BorderDecoration
{
	/// <summary>
	/// Create a LightBoxDecoration
	/// </summary>
	public LightBoxDecoration()
	{
		BoundsPadding = new Size(-1, 4);
		CornerRounding = 8.0f;
		FillBrush = new SolidBrush(Color.FromArgb(72, Color.Black));
	}

	/// <summary>
	/// Draw a tint over everything in the ObjectListView except the 
	/// row under the mouse.
	/// </summary>
	/// <param name="olv"></param>
	/// <param name="g"></param>
	/// <param name="r"></param>
	public override void Draw(ObjectListView olv, Graphics g, Rectangle r)
	{
		if (!r.Contains(olv.PointToClient(Cursor.Position)))
		{
			return;
		}

		Rectangle bounds = RowBounds;
		if (bounds.IsEmpty)
		{
			if (olv.View == View.Tile)
			{
				g.FillRectangle(FillBrush, r);
			}

			return;
		}

		using (Region newClip = new(r))
		{
			bounds.Inflate(BoundsPadding);
			newClip.Exclude(GetRoundedRect(bounds, CornerRounding));
			Region originalClip = g.Clip;
			g.Clip = newClip;
			g.FillRectangle(FillBrush, r);
			g.Clip = originalClip;
		}
	}
}

/// <summary>
/// Instances of this class put an Image over the row/cell that it is decorating
/// </summary>
public class ImageDecoration : ImageAdornment, IDecoration
{
	#region Constructors

	/// <summary>
	/// Create an image decoration
	/// </summary>
	public ImageDecoration()
	{
		Alignment = ContentAlignment.MiddleRight;
	}

	/// <summary>
	/// Create an image decoration
	/// </summary>
	/// <param name="image"></param>
	public ImageDecoration(Image image)
		: this()
	{
		Image = image;
	}

	/// <summary>
	/// Create an image decoration
	/// </summary>
	/// <param name="image"></param>
	/// <param name="transparency"></param>
	public ImageDecoration(Image image, int transparency)
		: this()
	{
		Image = image;
		Transparency = transparency;
	}

	/// <summary>
	/// Create an image decoration
	/// </summary>
	/// <param name="image"></param>
	/// <param name="alignment"></param>
	public ImageDecoration(Image image, ContentAlignment alignment)
		: this()
	{
		Image = image;
		Alignment = alignment;
	}

	/// <summary>
	/// Create an image decoration
	/// </summary>
	/// <param name="image"></param>
	/// <param name="transparency"></param>
	/// <param name="alignment"></param>
	public ImageDecoration(Image image, int transparency, ContentAlignment alignment)
		: this()
	{
		Image = image;
		Transparency = transparency;
		Alignment = alignment;
	}

	#endregion

	#region IDecoration Members

	/// <summary>
	/// Gets or sets the item being decorated
	/// </summary>
	public OLVListItem ListItem { get; set; }

	/// <summary>
	/// Gets or sets the sub item being decorated
	/// </summary>
	public OLVListSubItem SubItem { get; set; }

	#endregion

	#region Commands

	/// <summary>
	/// Draw this decoration
	/// </summary>
	/// <param name="olv">The ObjectListView being decorated</param>
	/// <param name="g">The Graphics used for drawing</param>
	/// <param name="r">The bounds of the rendering</param>
	public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) => DrawImage(g, CalculateItemBounds(ListItem, SubItem));

	#endregion
}

/// <summary>
/// Instances of this class draw some text over the row/cell that they are decorating
/// </summary>
public class TextDecoration : TextAdornment, IDecoration
{
	#region Constructors

	/// <summary>
	/// Create a TextDecoration
	/// </summary>
	public TextDecoration()
	{
		Alignment = ContentAlignment.MiddleRight;
	}

	/// <summary>
	/// Create a TextDecoration
	/// </summary>
	/// <param name="text"></param>
	public TextDecoration(string text)
		: this()
	{
		Text = text;
	}

	/// <summary>
	/// Create a TextDecoration
	/// </summary>
	/// <param name="text"></param>
	/// <param name="transparency"></param>
	public TextDecoration(string text, int transparency)
		: this()
	{
		Text = text;
		Transparency = transparency;
	}

	/// <summary>
	/// Create a TextDecoration
	/// </summary>
	/// <param name="text"></param>
	/// <param name="alignment"></param>
	public TextDecoration(string text, ContentAlignment alignment)
		: this()
	{
		Text = text;
		Alignment = alignment;
	}

	/// <summary>
	/// Create a TextDecoration
	/// </summary>
	/// <param name="text"></param>
	/// <param name="transparency"></param>
	/// <param name="alignment"></param>
	public TextDecoration(string text, int transparency, ContentAlignment alignment)
		: this()
	{
		Text = text;
		Transparency = transparency;
		Alignment = alignment;
	}

	#endregion

	#region IDecoration Members

	/// <summary>
	/// Gets or sets the item being decorated
	/// </summary>
	public OLVListItem ListItem { get; set; }

	/// <summary>
	/// Gets or sets the sub item being decorated
	/// </summary>
	public OLVListSubItem SubItem { get; set; }


	#endregion

	#region Commands

	/// <summary>
	/// Draw this decoration
	/// </summary>
	/// <param name="olv">The ObjectListView being decorated</param>
	/// <param name="g">The Graphics used for drawing</param>
	/// <param name="r">The bounds of the rendering</param>
	public virtual void Draw(ObjectListView olv, Graphics g, Rectangle r) => DrawText(g, CalculateItemBounds(ListItem, SubItem));

	#endregion
}
