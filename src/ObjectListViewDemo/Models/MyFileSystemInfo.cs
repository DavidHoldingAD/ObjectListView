using System.Collections;

namespace ObjectListViewDemo.Models;

/// <summary>
/// Standard .NET FileSystemInfos are always not equal to each other.
/// When we try to refresh a directory, our controls can't match up new
/// files with existing files. They are also sealed so we can't just subclass them.
/// This class is a wrapper around a FileSystemInfo that simply provides
/// equality.
/// </summary>
public class MyFileSystemInfo : IEquatable<MyFileSystemInfo>
{
	public MyFileSystemInfo(FileSystemInfo fileSystemInfo)
	{
		Info = fileSystemInfo ?? throw new ArgumentNullException(nameof(fileSystemInfo));
	}

	public bool IsDirectory => AsDirectory != null;

	public DirectoryInfo AsDirectory => Info as DirectoryInfo;
	public FileInfo AsFile => Info as FileInfo;

	public FileSystemInfo Info { get; }

	public string Name => Info.Name;

	public string Extension => Info.Extension;

	public DateTime CreationTime => Info.CreationTime;

	public DateTime LastWriteTime => Info.LastWriteTime;

	public string FullName => Info.FullName;

	public FileAttributes Attributes => Info.Attributes;

	public long Length => AsFile.Length;

	public IEnumerable GetFileSystemInfos()
	{
		ArrayList children = new();
		if (IsDirectory)
		{
			foreach (FileSystemInfo x in AsDirectory.GetFileSystemInfos())
			{
				children.Add(new MyFileSystemInfo(x));
			}
		}
		return children;
	}

	// Two file system objects are equal if they point to the same file system path

	public bool Equals(MyFileSystemInfo other)
	{
		if (ReferenceEquals(null, other))
		{
			return false;
		}

		if (ReferenceEquals(this, other))
		{
			return true;
		}

		return Equals(other.Info.FullName, Info.FullName);
	}
	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj))
		{
			return false;
		}

		if (ReferenceEquals(this, obj))
		{
			return true;
		}

		if (obj.GetType() != typeof(MyFileSystemInfo))
		{
			return false;
		}

		return Equals((MyFileSystemInfo)obj);
	}
	public override int GetHashCode() => (Info != null ? Info.FullName.GetHashCode() : 0);
	public static bool operator ==(MyFileSystemInfo left, MyFileSystemInfo right)
	{
		return Equals(left, right);
	}
	public static bool operator !=(MyFileSystemInfo left, MyFileSystemInfo right)
	{
		return !Equals(left, right);
	}
}
