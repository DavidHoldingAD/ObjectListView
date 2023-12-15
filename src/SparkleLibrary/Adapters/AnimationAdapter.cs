/*
 * AnimationAdapter - An adaptor that gives animation capacity to a Control
 * 
 * Author: Phillip Piper
 * Date: 08/02/2010 6:18 PM
 *
 * Change log:
 * 2010-03-30   JPP  - Optimize invalidating so that we don't call Invalidate
 *                     dozens of times unnecessarily
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

using System.Reflection;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace BrightIdeasSoftware;

/// <summary>
/// An AnimationAdapter makes the given Control able to show an Animation.
/// </summary>
/// <remarks>
/// <para>
/// To function correctly, the given control must trigger Paint events. 
/// That is: panels, buttons, labels, picture boxes, user controls, numeric spin controls, 
/// and (oddly enough) data grid view.
/// </para>
/// </remarks>
/// <example>
/// AnimationAdapter animatedControl = new AnimationAdapter(this.userControl1);
/// Animation animation = animatedControl.Animation;
/// // add sprites to animation
/// animation.Start();
/// </example>
public class AnimationAdapter
{
	#region Life and death

	public AnimationAdapter(Control control)
	{
		Animation = new Animation();
		Control = control;

		Animation.Started += new EventHandler<StartAnimationEventArgs>(Animation_Started);
		Animation.Stopped += new EventHandler<StopAnimationEventArgs>(Animation_Stopped);
		Animation.Redraw += new EventHandler<RedrawEventArgs>(Animation_Redraw);
		Animation.Ticked += new EventHandler<TickEventArgs>(Animation_Ticked);

		// Make the given control double buffered. 
		// Use reflection to get around DoubleBuffered being protected
		PropertyInfo pi = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
		if (pi != null)
		{
			pi.SetValue(control, true, null);
		}

		// Default values
		TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		SmoothingMode = SmoothingMode.HighQuality;
	}

	#endregion

	#region Public properties

	/// <summary>
	/// Gets or sets the control on which the animation will be drawn
	/// </summary>
	public Control Control { get; private set; }

	/// <summary>
	/// Gets or sets the control on which the animation will be drawn
	/// </summary>
	public Animation Animation { get; private set; }

	/// <summary>
	/// Gets or sets the smoothing mode that will be applied to the 
	/// graphic context that is used to draw the animation
	/// </summary>
	public SmoothingMode SmoothingMode { get; private set; }

	/// <summary>
	/// Gets or sets the text rendering hint that will be applied to the 
	/// graphic context that is used to draw the animation
	/// </summary>
	public TextRenderingHint TextRenderingHint { get; private set; }


	#endregion

	#region Event handlers

	protected virtual void Control_Disposed(object sender, EventArgs e) => Animation.Stop();

	protected virtual void Control_Paint(object sender, PaintEventArgs e)
	{

		// Lock this section so we are aren't troubled by interrupts from the ticker thread
		lock (myLock)
		{

			// Setup the graphics context and draw the animation
			Graphics g = e.Graphics;
			g.TextRenderingHint = TextRenderingHint;
			g.SmoothingMode = SmoothingMode;
			Animation.Draw(g);

			// Allow new invalidates on the control
			allowInvalidate = true;
		}
	}

	protected virtual void Animation_Started(object sender, StartAnimationEventArgs e)
	{
		SetAnimationBounds();

		Control.Paint += new PaintEventHandler(Control_Paint);
		Control.Disposed += new EventHandler(Control_Disposed);
	}

	/// <summary>
	/// Give the animation its outer bounds. 
	/// </summary>
	/// <remarks>
	/// This is normally the DisplayRectangle of the underlying Control.
	/// </remarks>
	protected virtual void SetAnimationBounds() => Animation.Bounds = Control.DisplayRectangle;

	protected virtual void Animation_Stopped(object sender, StopAnimationEventArgs e)
	{
		Control.Paint -= new PaintEventHandler(Control_Paint);
		Control.Disposed -= new EventHandler(Control_Disposed);
	}

	protected virtual void Animation_Redraw(object sender, RedrawEventArgs e)
	{
		// Don't trigger multiple invalidates
		lock (myLock)
		{
			if (allowInvalidate)
			{
				Control.Invalidate();
				allowInvalidate = false;
			}
		}
	}

	private object myLock = new();

	protected virtual void Animation_Ticked(object sender, TickEventArgs e)
	{
	}

	#endregion

	#region Private variables

	private bool allowInvalidate = true;

	#endregion
}
