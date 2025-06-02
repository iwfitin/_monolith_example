namespace Common.Interfaces;

public interface IHasId<out TKey>
{
    TKey Id { get; }
}
