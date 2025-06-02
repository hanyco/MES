using System.ComponentModel.DataAnnotations;

namespace Library.Data.Markers;

public interface IHasKey<TKey>
{
    [Key]
    TKey Id { get; }
}
