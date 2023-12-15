using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Design;

namespace BrightIdeasSoftware;

/// <summary>
/// PenData represents the data required to create a pen.
/// </summary>
/// <remarks>Pens cannot be edited directly within the IDE (is this VCS EE only?)
/// These objects allow pen characters to be edited within the IDE and then real
/// Pen objects created.</remarks>
[Editor(typeof(PenDataEditor), typeof(UITypeEditor)),
TypeConverter(typeof(PenDataConverter))]
public class PenData
{
	public PenData() : this(new SolidBrushData())
	{
	}

	public PenData(IBrushData brush)
	{
		Brush = brush;
	}

	public Pen GetPen()
	{
		Pen p = new(Brush.GetBrush(), Width);
		p.SetLineCap(StartCap, EndCap, DashCap);
		p.DashStyle = DashStyle;
		p.LineJoin = LineJoin;
		return p;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public IBrushData Brush { get; set; }

	[DefaultValue(typeof(DashCap), "Round")]
	public DashCap DashCap { get; set; } = DashCap.Round;

	[DefaultValue(typeof(DashStyle), "Solid")]
	public DashStyle DashStyle { get; set; } = DashStyle.Solid;

	[DefaultValue(typeof(LineCap), "NoAnchor")]
	public LineCap EndCap { get; set; } = LineCap.NoAnchor;

	[DefaultValue(typeof(LineJoin), "Round")]
	public LineJoin LineJoin { get; set; } = LineJoin.Round;

	[DefaultValue(typeof(LineCap), "NoAnchor")]
	public LineCap StartCap { get; set; } = LineCap.NoAnchor;

	[DefaultValue(1.0f)]
	public float Width { get; set; } = 1.0f;
}

[Editor(typeof(BrushDataEditor), typeof(UITypeEditor)),
TypeConverter(typeof(BrushDataConverter))]
public interface IBrushData
{
	Brush GetBrush();
}

public class SolidBrushData : IBrushData
{
	public Brush GetBrush()
	{
		if (Alpha < 255)
		{
			return new SolidBrush(Color.FromArgb(Alpha, Color));
		}
		else
		{
			return new SolidBrush(Color);
		}
	}

	[DefaultValue(typeof(Color), "")]
	public Color Color { get; set; } = Color.Empty;

	[DefaultValue(255)]
	public int Alpha { get; set; } = 255;
}

public class LinearGradientBrushData : IBrushData
{
	public Brush GetBrush() => new LinearGradientBrush(new Rectangle(0, 0, 100, 100), FromColor, ToColor, GradientMode);

	public Color FromColor { get; set; } = Color.Aqua;

	public Color ToColor { get; set; } = Color.Pink;

	public LinearGradientMode GradientMode { get; set; } = LinearGradientMode.Horizontal;
}

public class HatchBrushData : IBrushData
{
	public Brush GetBrush() => new HatchBrush(HatchStyle, ForegroundColor, BackgroundColor);

	public Color BackgroundColor { get; set; } = Color.AliceBlue;

	public Color ForegroundColor { get; set; } = Color.Aqua;

	public HatchStyle HatchStyle { get; set; } = HatchStyle.Cross;
}

public class TextureBrushData : IBrushData
{
	public Brush GetBrush()
	{
		if (Image == null)
		{
			return null;
		}
		else
		{
			return new TextureBrush(Image, WrapMode);
		}
	}

	public Image Image { get; set; }

	public WrapMode WrapMode { get; set; } = WrapMode.Tile;
}
