using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing.Design;

namespace BrightIdeasSoftware;

/// <summary>
/// Editor for use within the IDE when changing BrushFactory values
/// </summary>
internal class BrushDataEditor : UITypeEditor
{
	public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

	/// <summary>
	/// The user has asked to edit a value. Do it.
	/// </summary>
	public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
	{
		// For future reference. When we are running within the IDE, IDesignerHost will be non-null, e.g.
		//IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;

		// We need editor service otherwise we can't do anything
		if (provider.GetService(typeof(IWindowsFormsEditorService)) is not IWindowsFormsEditorService wfes)
		{
			return value;
		}

		BrushForm form = new();
		form.SetBrush(value as IBrushData);
		wfes.DropDownControl(form);
		return form.GetBrushData();
	}

	/// <summary>
	/// Can we paint a representation of our value?
	/// </summary>
	public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

	/// <summary>
	/// Draw a representation of our value
	/// </summary>
	public override void PaintValue(PaintValueEventArgs e)
	{
		if (e.Value is not IBrushData bd)
		{
			base.PaintValue(e);
		}
		else
		{
			e.Graphics.FillRectangle(BlockFormat.PrepareBrushForDrawing(bd.GetBrush(), e.Bounds), e.Bounds);
		}
	}
}

/// <summary>
/// Editor for use within the IDE when changing BrushFactory values
/// </summary>
internal class PenDataEditor : UITypeEditor
{
	public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

	/// <summary>
	/// The user has asked to edit a value. Do it.
	/// </summary>
	public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
	{
		// For future reference. When we are running within the IDE, IDesignerHost will be non-null, e.g.
		//IDesignerHost host = provider.GetService(typeof(IDesignerHost)) as IDesignerHost;

		// We need editor service otherwise we can't do anything
		if (provider.GetService(typeof(IWindowsFormsEditorService)) is not IWindowsFormsEditorService wfes)
		{
			return value;
		}

		PenForm form = new();
		form.SetPenData(value as PenData);
		wfes.DropDownControl(form);
		return form.GetPenData();
	}

	/// <summary>
	/// Can we paint a representation of our value?
	/// </summary>
	public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

	/// <summary>
	/// Draw a representation of our value
	/// </summary>
	public override void PaintValue(PaintValueEventArgs e)
	{
		if (e.Value is not PenData p)
		{
			base.PaintValue(e);
		}
		else
		{
			e.Graphics.SetClip(e.Bounds);
			e.Graphics.DrawLine(p.GetPen(), e.Bounds.Left, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);
			e.Graphics.ResetClip();
		}
	}
}

/// <summary>
/// Instances of this class handle converting BrushFactories to and from various other forms.
/// This class can convert BrushFactories to and from string representations, and can convert
/// to InstanceDescriptor (which are used to convert an object into source code).
/// </summary>
internal class BrushDataConverter : ExpandableObjectConverter
{
	/// <summary>
	/// Indicate that we can convert to a string or to an InstanceDescriptor (these
	/// are used to serial an object to code).
	/// </summary>
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}

		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			if (value is not IBrushData b)
			{
				return "(none)";
			}
			else
			{
				return b.GetType().Name;
			}
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}

		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
	{
		if (value != null && value.GetType() == typeof(string))
		{
			string s = value as string;
			if (s.Length == 0)
			{
				return null;
			}

			switch (s[0])
			{
				case 'S':
					return new SolidBrushData();
				case 'G':
					return new LinearGradientBrushData();
				case 'H':
					return new HatchBrushData();
				default:
					return null;
			}
		}
		return base.ConvertFrom(context, culture, value);
	}
}

/// <summary>
/// Instances of this class handle converting BrushFactories to and from various other forms.
/// This class can convert BrushFactories to and from string representations, and can convert
/// to InstanceDescriptor (which are used to convert an object into source code).
/// </summary>
internal class PenDataConverter : ExpandableObjectConverter
{
	/// <summary>
	/// Indicate that we can convert to a string or to an InstanceDescriptor (these
	/// are used to serial an object to code).
	/// </summary>
	public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			return true;
		}

		return base.CanConvertTo(context, destinationType);
	}

	public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
	{
		if (destinationType == typeof(string))
		{
			if (value is not PenData p || p.Brush == null)
			{
				return "(none)";
			}
			else
			{
				string name = p.Brush.GetType().Name;
				return name.Substring(0, name.Length - "BrushData".Length) + "Pen";
			}
		}

		return base.ConvertTo(context, culture, value, destinationType);
	}

	public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
	{
		if (sourceType == typeof(string))
		{
			return true;
		}

		return base.CanConvertFrom(context, sourceType);
	}

	public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
	{
		if (value != null && value.GetType() == typeof(string))
		{
			string s = value as string;
			if (s.Length == 0)
			{
				return null;
			}

			switch (s[0])
			{
				case 'S':
					return new PenData(new SolidBrushData());
				case 'G':
					return new PenData(new LinearGradientBrushData());
				case 'H':
					return new PenData(new HatchBrushData());
				default:
					return null;
			}
		}
		return base.ConvertFrom(context, culture, value);
	}
}
