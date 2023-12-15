using System.ComponentModel;

using BrightIdeasSoftware;

namespace ObjectListViewDemo.Models;

public class Person : INotifyPropertyChanged
{
	public bool IsActive = true;

	public Person(string name)
	{
		this.name = name;
	}

	public Person(string name, string occupation, int culinaryRating, DateTime birthDate, double hourlyRate, bool canTellJokes, string photo, string comments)
	{
		this.name = name;
		Occupation = occupation;
		CulinaryRating = culinaryRating;
		BirthDate = birthDate;
		this.hourlyRate = hourlyRate;
		CanTellJokes = canTellJokes;
		Comments = comments;
		Photo = photo;
	}

	public Person(Person other)
	{
		name = other.Name;
		Occupation = other.Occupation;
		CulinaryRating = other.CulinaryRating;
		BirthDate = other.BirthDate;
		hourlyRate = other.GetRate();
		CanTellJokes = other.CanTellJokes;
		Photo = other.Photo;
		Comments = other.Comments;
		MaritalStatus = other.MaritalStatus;
	}

	[OLVIgnore]
	public Image ImageAspect => Resource1.folder16;

	[OLVIgnore]
	public string ImageName => "user";

	// Allows tests for properties.
	[OLVColumn(ImageAspectName = "ImageName")]
	public string Name
	{
		get { return name; }
		set
		{
			if (name == value)
			{
				return;
			}

			name = value;
			OnPropertyChanged(nameof(Name));
		}
	}

	private string name;

	[OLVColumn(ImageAspectName = "ImageName")]
	public string Occupation
	{
		get { return occupation; }
		set
		{
			if (occupation == value)
			{
				return;
			}

			occupation = value;
			OnPropertyChanged(nameof(Occupation));
		}
	}

	private string occupation;

	public int CulinaryRating { get; set; }

	public DateTime BirthDate { get; set; }

	public int YearOfBirth
	{
		get { return BirthDate.Year; }
		set { BirthDate = new DateTime(value, BirthDate.Month, BirthDate.Day); }
	}

	// Allow tests for methods
	public double GetRate() => hourlyRate;

	private double hourlyRate;

	public void SetRate(double value) => hourlyRate = value;

	// Allows tests for fields.
	public string Photo;

	public string Comments;
	public int serialNumber;
	public bool? CanTellJokes;

	// Allow tests for enums
	public MaritalStatus MaritalStatus = MaritalStatus.Single;

	#region Implementation of INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged(string propertyName)
	{
		if (PropertyChanged != null)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	#endregion Implementation of INotifyPropertyChanged
}