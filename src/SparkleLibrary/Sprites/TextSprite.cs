/*
 * TextSprite - A sprite that draws text
 * 
 * Author: Phillip Piper
 * Date: 08/02/2010 6:18 PM
 *
 * Change log:
 * 2010-03-31   JPP  - Correctly calculate the height of wrapped text
 *                   - Cleaned up
 * 2010-02-29   JPP  - Add more formatting options (wrap, border, background)
 * 2010-02-08   JPP  - Initial version
 *
 * To do:
 *
 * Copyright (C) 2010 Phillip Piper
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
/// A TextSprite is animated text. Like all Sprites, animation is achieved
/// by adding animations to it.
/// </summary>
public class TextSprite : Sprite
{
	#region Life and death

	public TextSprite()
	{
		Font = new Font("Tahoma", 12);
		ForeColor = Color.Blue;
		BackColor = Color.Empty;
	}

	public TextSprite(string text)
	{
		Text = text;
	}

	public TextSprite(string text, Font font, Color foreColor)
	{
		Text = text;
		Font = font;
		ForeColor = foreColor;
	}

	public TextSprite(string text, Font font, Color foreColor, Color backColor, Color borderColor, float borderWidth)
	{
		Text = text;
		Font = font;
		ForeColor = foreColor;
		BackColor = backColor;
		BorderColor = borderColor;
		BorderWidth = borderWidth;
	}

	#endregion

	#region Configuration properties

	/// <summary>
	/// Gets or sets the text that will be rendered by the sprite
	/// </summary>
	public string Text { get; set; }

	/// <summary>
	/// Gets or sets the font in which the text will be rendered.
	/// This will be scaled before being used to draw the text.
	/// </summary>
	public Font Font { get; set; }

	/// <summary>
	/// Gets or sets the color of the text
	/// </summary>
	public Color ForeColor { get; set; } = Color.Empty;

	/// <summary>
	/// Gets or sets the background color of the text
	/// Set this to Color.Empty to not draw a background
	/// </summary>
	public Color BackColor { get; set; } = Color.Empty;

	/// <summary>
	/// Gets or sets the color of the border around the billboard.
	/// Set this to Color.Empty to remove the border
	/// </summary>
	public Color BorderColor { get; set; } = Color.Empty;

	/// <summary>
	/// Gets or sets the width of the border around the text
	/// </summary>
	public float BorderWidth { get; set; }

	/// <summary>
	/// How rounded should the corners of the border be? 0 means no rounding.
	/// </summary>
	/// <remarks>If this value is too large, the edges of the border will appear odd.</remarks>
	public float CornerRounding { get; set; } = 16.0f;

	/// <summary>
	/// Gets the font that will be used to draw the text or a reasonable default
	/// </summary>
	public Font FontOrDefault => Font ?? new Font("Tahoma", 16);

	/// <summary>
	/// Does this text have a background?
	/// </summary>
	public bool HasBackground => BackColor != Color.Empty;

	/// <summary>
	/// Does this overlay have a border?
	/// </summary>
	public bool HasBorder => BorderColor != Color.Empty && BorderWidth > 0;

	/// <summary>
	/// Gets or sets the maximum width of the text. Text longer than this will wrap.
	/// 0 means no maximum.
	/// </summary>
	public int MaximumTextWidth { get; set; } = 0;

	/// <summary>
	/// Gets or sets the formatting that should be used on the text
	/// </summary>
	public StringFormat StringFormat
	{
		get
		{
			if (stringFormat == null)
			{
				stringFormat = new StringFormat();
				stringFormat.Alignment = StringAlignment.Center;
				stringFormat.LineAlignment = StringAlignment.Center;
				stringFormat.Trimming = StringTrimming.EllipsisCharacter;
				if (!Wrap)
				{
					stringFormat.FormatFlags = StringFormatFlags.NoWrap;
				}
			}
			return stringFormat;
		}
		set { stringFormat = value; }
	}
	private StringFormat stringFormat;

	/// <summary>
	/// Gets or sets whether the text will wrap when it exceeds its bounds
	/// </summary>
	public bool Wrap { get; set; }

	#endregion

	#region Sprite properties

	/// <summary>
	/// Gets the size of the drawn text. This is always calculated from the 
	/// natural size of the text when drawn in the correctly scaled font.
	/// It cannot be set directly.
	/// </summary>
	public override Size Size
	{
		get
		{
			if (string.IsNullOrEmpty(Text))
			{
				return Size.Empty;
			}

			// This is a stupid hack to get a Graphics object. How should this be done?
			lock (dummyImageLock)
			{
				using (Graphics g = Graphics.FromImage(dummyImage))
				{
					return g.MeasureString(Text, ActualFont, CalcMaxLineWidth(), StringFormat).ToSize();
				}
			}
		}
		set { }
	}
	private Image dummyImage = new Bitmap(1, 1);
	private object dummyImageLock = new();

	#endregion

	#region Implementation properties

	/// <summary>
	/// Gets the font that will be used to draw the text. This takes
	/// scaling into account.
	/// </summary>
	protected Font ActualFont
	{
		get
		{
			if (Scale == 1.0f)
			{
				return Font;
			}
			else
			{
				// TODO: Cache this font and discard it when either Font or Scale changed.
				return new Font(Font.FontFamily, Font.SizeInPoints * Scale, Font.Style);
			}
		}
	}

	#endregion

	#region Sprite methods

	public override void Draw(Graphics g)
	{
		if (string.IsNullOrEmpty(Text) || Opacity <= 0.0f)
		{
			return;
		}

		ApplyState(g);
		DrawText(g, Text, Opacity);
		UnapplyState(g);
	}

	#endregion

	#region Drawing methods

	protected void DrawText(Graphics g, string s, float opacity)
	{
		Font f = ActualFont;
		SizeF textSize = g.MeasureString(s, f, CalcMaxLineWidth(), StringFormat);
		DrawBorderedText(g, new Rectangle(0, 0, 1 + (int)textSize.Width, 1 + (int)textSize.Height), s, f, opacity);
	}

	protected int CalcMaxLineWidth() => MaximumTextWidth > 0 ? (int)(MaximumTextWidth * Scale) : int.MaxValue;

	protected Brush GetTextBrush(float opacity)
	{
		if (opacity < 1.0f)
		{
			return new SolidBrush(Color.FromArgb((int)(opacity * 255), ForeColor));
		}
		else
		{
			return new SolidBrush(ForeColor);
		}
	}

	protected Brush GetBackgroundBrush(float opacity)
	{
		if (opacity < 1.0f)
		{
			return new SolidBrush(Color.FromArgb((int)(opacity * 255), BackColor));
		}
		else
		{
			return new SolidBrush(BackColor);
		}
	}

	protected Pen GetBorderPen(float opacity)
	{
		if (opacity < 1.0f)
		{
			return new Pen(Color.FromArgb((int)(opacity * 255), BorderColor), BorderWidth);
		}
		else
		{
			return new Pen(BorderColor, BorderWidth);
		}
	}

	/// <summary>
	/// Draw the text with a border
	/// </summary>
	/// <param name="g">The Graphics used for drawing</param>
	/// <param name="textRect">The bounds within which the text should be drawn</param>
	/// <param name="text">The text to draw</param>
	protected void DrawBorderedText(Graphics g, Rectangle textRect, string text, Font font, float opacity)
	{
		Rectangle borderRect = textRect;
		if (BorderWidth > 0.0f)
		{
			borderRect.Inflate((int)BorderWidth / 2, (int)BorderWidth / 2);
		}

		borderRect.Y -= 1; // Looks better a little higher

		using (GraphicsPath path = ShapeSprite.GetRoundedRect(borderRect, CornerRounding * Scale))
		{
			if (HasBackground)
			{
				using (Brush brush = GetBackgroundBrush(opacity))
				{
					g.FillPath(brush, path);
				}
			}

			using (Brush textBrush = GetTextBrush(opacity))
			{
				g.DrawString(text, font, textBrush, textRect, StringFormat);
			}

			if (HasBorder)
			{
				using (Pen pen = GetBorderPen(opacity))
				{
					g.DrawPath(pen, path);
				}
			}
		}

	}


	#endregion
}
