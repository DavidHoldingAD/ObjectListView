/*
 * ImageSprite - A sprite that draws an Image
 * 
 * Author: Phillip Piper
 * Date: 08/02/2010 6:18 PM
 *
 * Change log:
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

using System.Drawing.Imaging;

namespace BrightIdeasSoftware;

/// <summary>
/// An ImageSprite draws an image onto the animation, to which animations can be applied
/// </summary>
/// <remarks>The image can even be an animated GIF!</remarks>
public class ImageSprite : Sprite
{
	#region Life and death

	public ImageSprite(Image image)
	{
		Image = image;
	}

	#endregion

	#region Implementation properties

	protected Image Image;

	#endregion

	#region Sprite properties

	/// <summary>
	/// Gets or sets how big the image is
	/// </summary>
	/// <remarks>The image size cannot be set, since it is natural size multiplied by 
	/// the Scale property.</remarks>
	public override Size Size
	{
		get
		{
			if (Image == null)
			{
				return Size.Empty;
			}

			// Internal the Image class cannot handle being accessed by multiple threads
			// at the same time. So make sure the access is serialized.
			lock (locker)
			{
				if (Scale == 1.0f)
				{
					return Image.Size;
				}
				else
				{
					return new Size((int)(Image.Size.Width * Scale),
						(int)(Image.Size.Height * Scale));
				}
			}
		}
		set
		{
		}
	}

	#endregion

	#region Sprite methods

	public override void Start()
	{
		if (Image != null && ImageAnimator.CanAnimate(Image))
		{
			isAnimatedImage = true;
			ImageAnimator.Animate(Image, OnFrameChanged);
		}
	}

	private bool isAnimatedImage;

	public override void Stop()
	{
		if (isAnimatedImage)
		{
			ImageAnimator.StopAnimate(Image, OnFrameChanged);
		}
	}

	public override void Draw(Graphics g)
	{
		ApplyState(g);
		lock (locker)
		{
			if (Image != null)
			{
				if (isAnimatedImage)
				{
					ImageAnimator.UpdateFrames(Image);
				}

				DrawTransparentBitmap(g, Bounds, Image, Opacity);
			}
			UnapplyState(g);
		}
	}

	private object locker = new();

	#endregion

	#region Implementation methods

	/// <summary>
	/// The frame on an animated GIF has changed. Normally we would redraw, but
	/// we leave that to the animation controller.
	/// </summary>
	/// <param name="o"></param>
	/// <param name="e"></param>
	private void OnFrameChanged(object o, EventArgs e)
	{
	}

	/// <summary>
	/// Draw an image in a (possibilty) transluscent fashion
	/// </summary>
	/// <param name="g"></param>
	/// <param name="r"></param>
	/// <param name="image"></param>
	/// <param name="transparency"></param>
	protected void DrawTransparentBitmap(Graphics g, Rectangle r, Image image, float transparency)
	{
		if (transparency <= 0.0f)
		{
			return;
		}

		ImageAttributes imageAttributes = null;
		if (transparency < 1.0f)
		{
			imageAttributes = new ImageAttributes();
			float[][] colorMatrixElements = {
				new float[] {1,  0,  0,  0, 0},
				new float[] {0,  1,  0,  0, 0},
				new float[] {0,  0,  1,  0, 0},
				new float[] {0,  0,  0,  transparency, 0},
				new float[] {0,  0,  0,  0, 1}};

			imageAttributes.SetColorMatrix(new ColorMatrix(colorMatrixElements));
		}

		Rectangle dest = new(Point.Empty, Size);
		g.DrawImage(image,
		   dest,                                          // destination rectangle
		   0, 0, image.Size.Width, image.Size.Height,  // source rectangle
		   GraphicsUnit.Pixel,
		   imageAttributes);
	}

	#endregion
}
