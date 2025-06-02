using System.ComponentModel.DataAnnotations;

namespace Library.Data.Markers;

public interface ICanSetKey<TIdType> : IHasKey<TIdType>
{
    [Key]
    new TIdType Id { get; set; }
}
