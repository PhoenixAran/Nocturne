using System;
using System.Collections;
using System.Collections.Generic;

//modified from https://bitbucket.org/MattThorson/monocle-engine

namespace Nocturne
{
    public class EntityList : IEnumerable<Entity>, IEnumerable
    {
        public Scene Scene { get; private set; }

        private List<Entity> entities;
        private List<Entity> toAdd;
        private List<Entity> toAwake;
        private List<Entity> toRemove;

        private HashSet<Entity> current;
        private HashSet<Entity> adding;
        private HashSet<Entity> removing;

        private bool unsorted;

        internal EntityList( Scene scene )
        {
            Scene = scene;

            entities = new List<Entity>();
            toAdd = new List<Entity>();
            toAwake = new List<Entity>();
            toRemove = new List<Entity>();

            current = new HashSet<Entity>();
            adding = new HashSet<Entity>();
            removing = new HashSet<Entity>();
        }

        internal void MarkUnsorted()
        {
            unsorted = true;
        }

        public void UpdateLists()
        {
            if ( toAdd.Count > 0 )
            {
                for ( int i = 0; i < toAdd.Count; ++i )
                {
                    Entity entity = toAdd[i];
                    if ( !current.Contains( entity ) )
                    {
                        current.Add( entity );
                        entities.Add( entity );
                        if ( Scene != null )
                        {
                            Scene.TagLists.EntityAdded( entity );
                            entity.Added( Scene );
                        }
                    }
                }

                unsorted = true;
            }

            if ( toRemove.Count > 0 )
            {
                for ( int i = 0; i < toRemove.Count; ++i )
                {
                    Entity entity = toRemove[i];
                    if ( entities.Contains(entity) )
                    {
                        current.Remove( entity );
                        entities.Remove( entity );
                        if ( Scene != null )
                        {
                            Scene.TagLists.EntityRemoved( entity );
                            entity.Removed( Scene );
                        }
                    }
                }

                toRemove.Clear();
                removing.Clear();
            }

            if ( unsorted )
            {
                unsorted = false;
                entities.Sort( CompareDepth );
            }

            if ( toAdd.Count > 0 )
            {
                for ( int i = 0; i < toAdd.Count; ++i )
                {
                    toAwake.Add( toAdd[i] );
                }

                toAdd.Clear();
                adding.Clear();

                for ( int i = 0; i < toAwake.Count; ++i )
                {
                    Entity entity = toAwake[i];
                    if ( entity.Scene == Scene )
                        entity.Awake( Scene );
                }

                toAwake.Clear();
            }

        }

        public void Add( Entity entity )
        {
            if ( !adding.Contains(entity) && !current.Contains( entity ) )
            {
                adding.Add( entity );
                toAdd.Add( entity );
            }
        }

        public void Remove( Entity entity )
        {
            if ( !removing.Contains(entity) && current.Contains( entity ) )
            {
                removing.Add( entity );
                toRemove.Add( entity );
            }
        }

        public int Count => entities.Count;

        public int AmountOf<T>() where T : Entity
        {
            int count = 0; 
            for ( int i = 0; i < entities.Count; ++i )
            {
                if ( entities[i] is T )
                {
                    ++count;
                }
            }

            return count;
        }

        public T FindFirst<T>() where T : Entity
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                if ( entities[i] is T targetType )
                    return targetType;
            }
            return null;
        }

        /// <summary>
        /// Return the list returned with ListPool.Free()
        /// </summary>
        public List<T> FindAll<T>() where T : Entity
        {
            List<T> list = ListPool<T>.Obtain();

            for ( int i = 0; i < entities.Count; ++i )
            {
                if ( entities[i] is T targetType )
                {
                    list.Add( targetType );
                }
            }

            return list;
        }

        public void With<T>(Action<T> action) where T : Entity
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                if ( entities[i] is T targetType )
                {
                    action?.Invoke( targetType );
                }
            }
        }

        internal void SceneBegin()
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                entities[i].SceneBegin( Scene );
            }
        }

        internal void SceneEnd()
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                entities[i].SceneEnd( Scene );
            }
        }


        /// <summary>
        /// default way to update entities, feel free to not use this and implement your own way to
        /// update entities
        /// </summary>
        public void Update()
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                Entity entity = entities[i];
                if ( entity.Enabled )
                {
                    entity.Update();
                }
            }
        }

        /// <summary>
        /// default way to draw entities, feel free to not use this if you desire more control over the 
        /// ordering of drawing entities
        /// </summary>
        public void Render( RectangleF cameraBounds )
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                Entity entity = entities[i];
                if ( entity.Visible )
                {
                    entity.Render( cameraBounds );
                }
            }
        }

        internal void DebugRender()
        {
            for ( int i = 0; i < entities.Count; ++i )
            {
                Entity entity = entities[i];
                entity.DebugRender();
            }
        }

        public static Comparison<Entity> CompareDepth = ( a, b ) => Math.Sign( b.actualDepth - a.actualDepth );

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return entities.GetEnumerator();
        }
    }


}
