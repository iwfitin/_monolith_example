namespace Common.Attributes;

public sealed class EnumIdAttribute : Attribute
{
    public string Id { get; set; }

    public EnumIdAttribute(string id)
    {
        Id = id;
    }
}
