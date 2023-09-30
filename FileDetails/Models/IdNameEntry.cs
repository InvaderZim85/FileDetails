namespace FileDetails.Models;

/// <summary>
/// Represents an Id / Text entry
/// </summary>
internal class IdNameEntry
{
    /// <summary>
    /// Gets or sets the id of the entry
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the entry
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new, empty instance of the <see cref="IdNameEntry"/>
    /// </summary>
    public IdNameEntry() { }

    /// <summary>
    /// Creates a new instance of the <see cref="IdNameEntry"/>
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="name">The name</param>
    public IdNameEntry(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Name;
    }
}