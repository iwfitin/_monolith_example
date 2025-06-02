using Common.Interfaces;

namespace DAL.Entities.Files;

public abstract class SavedFile : IHasId<string>
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Path { get; set; }
}
