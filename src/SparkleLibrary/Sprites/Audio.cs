﻿/*
 * Audio - Audio allows sound to be played during a animation.
 * 
 * Author: Phillip Piper
 * Date: 18/01/2010 5:29 PM
 *
 * Change log:
 * 2010-01-18   JPP  - Initial version
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

using System.Media;
using System.Reflection;

namespace BrightIdeasSoftware;

/// <summary>
/// Instances of this class allow sound to be played at specified
/// points within a animation.
/// </summary>
/// <remarks>
/// <para>
/// This class uses the SoundPlayer class internally, and thus can
/// only handle system sounds and WAV sound files.
/// </para>
/// <para>A sound that is already playing cannot be paused.</para>
/// </remarks>
public class Audio : Animateable
{
	#region Life and death

	/// <summary>
	/// Load a sound from a named resource.
	/// </summary>
	/// <param name="resourceName">The name of the resource including the trailing ".wav"</param>
	/// <remarks>To embed a wav file, simple add it to the project, and change "Build Action"
	/// to "Embedded Resource".</remarks>
	/// <see cref="http://msdn.microsoft.com/en-us/library/ms950960.aspx"/>
	public static Audio FromResource(string resourceName)
	{
		Audio sound = new();
		sound.ResourceName = resourceName;
		return sound;
	}

	/// <summary>
	/// Create an empty Audio object
	/// </summary>
	public Audio()
	{
	}

	/// <summary>
	/// Creates an Audio object that will play the given "wav" file
	/// </summary>
	/// <param name="fileName"></param>
	public Audio(string fileName)
	{
		FileName = fileName;
	}

	/// <summary>
	/// Creates an Audio object that will play the given system sound
	/// </summary>
	/// <param name="sound"></param>
	public Audio(SystemSound sound)
	{
		SystemSound = sound;
	}

	#endregion

	#region Implementation properties

	/// <summary>
	/// Gets or sets the name of the audio file that will be played.
	/// </summary>
	protected string FileName;
	protected SoundPlayer Player;
	protected string ResourceName;
	protected SystemSound SystemSound;

	#endregion

	#region Animation methods

	/// <summary>
	/// Start the sound playing
	/// </summary>
	public override void Start()
	{
		// If we are supposed to play an application resource, try to load it
		if (!string.IsNullOrEmpty(ResourceName))
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string assemblyName = executingAssembly.GetName().Name;
			Stream stream = executingAssembly.GetManifestResourceStream(assemblyName + "." + ResourceName);
			if (stream != null)
			{
				Player = new SoundPlayer(stream);
			}
		}

		if (!string.IsNullOrEmpty(FileName))
		{
			Player = new SoundPlayer(FileName);
		}

		// We could just use Play() and let the player handle the threading for us, but:
		// - there is no builtin way to know when the sound has finished
		// - on XP (at least), using Play() on a Stream gives noise -- but PlaySync() works fine.

		done = false;
		Thread newThread = new((ThreadStart)delegate
		{
			if (SystemSound != null)
			{
				SystemSound.Play();
			}
			else
			{
				if (Player != null)
				{
					Player.PlaySync();
				}
			}
			done = true;
		});
		newThread.Start();
	}

	private bool done;

	/// <summary>
	/// Advance the audio and return if it is done.
	/// </summary>
	/// <param name="elapsed"></param>
	/// <returns></returns>
	public override bool Tick(long elapsed) => !done;

	/// <summary>
	/// Stop the sound
	/// </summary>
	public override void Stop()
	{
		if (SystemSound != null)
		{
			return;
		}

		if (Player != null)
		{
			Player.Stop();
			Player.Dispose();
			Player = null;
		}
	}

	#endregion
}