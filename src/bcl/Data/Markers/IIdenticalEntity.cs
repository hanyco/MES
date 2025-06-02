namespace Library.Data.Markers;

public interface IIdenticalEntity<TIdType> : ICanSetKey<TIdType>, IEntity;

public interface IIdenticalEntity : IIdenticalEntity<long>;