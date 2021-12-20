namespace Era.Core.Event
{
    public class EntityAdded<TEntity>
    {
        public EntityAdded(TEntity entity)
        {
            Entity = entity;
        }
        public TEntity Entity { get; }
    }

    public class EntityModified<TEntity>
    {
        public EntityModified(TEntity entity)
        {
            Entity = entity;
        }
        public TEntity Entity { get; }
    }

    public class EntityDeleted<TEntity>
    {
        public EntityDeleted(TEntity entity)
        {
            Entity = entity;
        }
        public TEntity Entity { get; }
    }
}
