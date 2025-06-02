namespace DAL.Entities.Files;

public sealed class Document : SavedFile
{
    public DateTime LastModifiedTime { get; set; }
}
