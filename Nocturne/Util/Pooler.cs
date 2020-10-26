
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nocturne
{
    public class Pooler
    {
        Dictionary<string, Type> RegisteredTypes { get; set; }
        Dictionary<Type, Queue<Entity>> Pools { get; set; }

        public Pooler()
        {
            Pools = new Dictionary<Type, Queue<Entity>>();
            RegisteredTypes = new Dictionary<string, Type>( StringComparer.OrdinalIgnoreCase );

            foreach ( Type type in Assembly.GetEntryAssembly().GetTypes() )
            {
                if ( type.GetCustomAttributes( typeof( PooledAttribute ), false ).Length > 0 )
                {
                    if ( !typeof( Entity ).IsAssignableFrom( type ) )
                        throw new Exception( $"Type '{type.Name}' cannot be pooled because it doesn't derive from Entity" );
                    if ( type.GetConstructor( Type.EmptyTypes ) == null )
                        throw new Exception( $"Type '{type.Name}' cannot be Pooled because it doesn't have a parameterless constructor" );
                    Pools.Add( type, new Queue<Entity>() );
                    RegisteredTypes.Add( type.GetCustomAttribute<PooledAttribute>().Key, type );
                }
            }
        }

        public Entity Create( Type type )
        {
            if ( !Pools.TryGetValue( type, out Queue<Entity> queue ) )
                return (Entity)Activator.CreateInstance( type );

            if ( queue.Count == 0 )
                return (Entity)Activator.CreateInstance( type );
            return queue.Dequeue();
        }

        public T Create<T>() where T : Entity, new()
        {
            return (T)Create( typeof( T ) );
        }

        public Entity Create( string typeKey )
        {
            if ( !RegisteredTypes.TryGetValue( typeKey, out Type type ) )
                throw new Exception( $"No registered type with key '{typeKey}' was found" );

            return Create( type );
        }

        public void Return( Entity entity )
        {
            Type type = entity.GetType();
            if ( Pools.TryGetValue( type, out Queue<Entity> queue ) )
            {
                if ( entity is IPoolable poolable )
                    poolable.Reset();
                queue.Enqueue( entity );
            }
        }
    }

    public class PooledAttribute : Attribute
    {
        public string Key;
        public PooledAttribute( string key )
        {
            Key = key;
        } 
    }
}
