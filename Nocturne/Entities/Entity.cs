using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

// modified version of the Entity class in https://github.com/prime31/Nez and https://bitbucket.org/MattThorson/monocle-engine

namespace Nocturne
{
    public class Entity
    {
        static uint _idGenerator;

        #region Properties and fields
        /// <summary>
        /// entity name. Useful for doing scene-wide searches for an entity
        /// </summary>
        public string Name;

        /// <summary>
        /// unique identifier for this Entity
        /// </summary>
        public readonly uint Id;

        /// <summary>
        /// encapsulates the Entity's position/rotation/scale and allows
        /// setting up a hiearchy
        /// </summary>
        public readonly Transform Transform;

        /// <summary>
        /// list of all components currently attached to this entity
        /// </summary>
        public readonly ComponentList Components;

        public Scene Scene { get; private set; }

        /// <summary>
        /// Can later be used to query the scene for all Entities with a specific tag
        /// </summary>
        public int Tag
        {
            get => tag;
            set
            {
                if ( this.tag != value )
                {
                    if ( Scene != null )
                    {
                        for ( int i = 0; i < BitTag.TotalTags; ++i )
                        {
                            int check = i << i;
                            bool add = ( value & check ) != 0;
                            bool has = ( value & check ) != 0;

                            if ( has != add )
                            {
                                if ( add )
                                {
                                    Scene.TagLists[i].Add(this);
                                }
                                else
                                {
                                    Scene.TagLists[i].Remove(this);
                                }
                            }
                        }
                    }
                    tag = value;
                }
            }
        }

        public bool Enabled = true;
        public bool Visible = true;

        protected int tag = 0;

        internal int depth = 0;
        internal double actualDepth = 0;
        #endregion

        public Entity( bool enabled = true, bool visible = true, string name = null )
        {
            Id = _idGenerator;
            if ( name == null )
            {
                Name = $"entity-{Id}";
            }
            Components = new ComponentList( this );
            Transform = new Transform( this );
            _idGenerator++;
        }

        internal void OnTransformChanged( Transform.Component comp )
        {
            Components.OnEntityTransformChanged( comp );
        }
        
        public virtual void Update()
        {
            Components.Update();
            OnUpdate();
        }

        public virtual void OnUpdate()
        {

        }

        public virtual void Render( RectangleF cameraBounds )
        {
            Components.Render( cameraBounds );
            OnRender(cameraBounds);
        }

        public virtual void OnRender( RectangleF cameraBounds )
        {
            
        }

        public virtual void DebugRender()
        {
            Components.DebugRender();
        }

        #region Depth
        public int Depth
        {
            get
            {
                return depth;
            }
            set
            {
                if ( depth != value )
                {
                    depth = value;
                    if ( Scene != null )
                    {
                        Scene.SetActualDepth( this );
                    }
                }
            }
        }
        #endregion

        #region Tag
        public bool TagFullCheck( int tag )
        {
            return ( this.tag & tag ) == tag;
        }

        public bool TagCheck( int tag )
        {
            return ( this.tag & tag ) != 0;
        }

        public void AddTag( int tag )
        {
            Tag |= tag;
        }

        public void RemoveTag( int tag )
        {
            Tag &= ~tag;
        }
        #endregion

        #region Transform passthroughs
        /// <summary>
        /// the parent transform for this Entity's transform instance
        /// </summary>
        public Transform Parent
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.Parent;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetParent( value );
        }

        /// <summary>
        /// total children this Entity's transform has
        /// </summary>
        public int ChildCount
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.ChildCount;
        }

        /// <summary>
        /// position of this Entity's transform in space
        /// </summary>
        public Vector2 Position
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.Position;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetPosition( value );
        }

        /// <summary>
        /// position of this Entity's transform relative to the parent transform's position
        /// If the entity has no parent transform, it is the same as calling Entity.Position
        /// </summary>
        public Vector2 LocalPosition
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.LocalPosition;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetLocalPosition( value );
        }

        /// <summary>
        /// rotation of this Entity's transform in world space in radians
        /// </summary>
        public float Rotation
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.Rotation;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetRotation( value );
        }

        /// <summary>
        /// rotation of this Entity's transform relative to the parent transform's rotation in radians.
        /// If the entity has no parent transform, it is the same as calling Entity.LocalRotation
        /// </summary>
        public float LocalRotation
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.LocalRotation;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetLocalRotation( value );
        }

        /// <summary>
        /// Sets the global scale for this Entity's transform
        /// </summary>
        public Vector2 Scale
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.Scale;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetScale( value );
        }

        /// <summary>
        /// The entity's transform scale relative to it's parent. If the entity's 
        /// transform has no parent, it is the same as calling Entity.scale
        /// </summary>
        public Vector2 LocalScale
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.LocalScale;
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            set => Transform.SetLocalScale( value );
        }

        public Matrix2D WorldInverseTransform
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.WorldInverseTransform;
        }

        public Matrix2D LocalToWorldTransform
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.LocalToWorldTransform;
        }

        public Matrix2D WorldToLocalTransform
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Transform.WorldToLocalTransform;
        }

        #endregion

        #region Life Cycle Methods
        /// <summary>
        /// called when the containing Scene begins
        /// </summary>
        public virtual void SceneBegin( Scene scene )
        {
            Components.SceneBegin( scene );
        }

        /// <summary>
        /// called when the containing Scene ends
        /// </summary>
        public virtual void SceneEnd( Scene scene )
        {
            Components.SceneEnd(scene);
        }

        /// <summary>
        /// Called before the frame starts, after Entities are added and removed, on that frame that the
        /// Entity was added. Useful if you added two Entities in the same frame, and need them to detect
        /// eachother before they start updating
        /// </summary>
        public virtual void Awake( Scene scene )
        {
            Components.EntityAwake();
        }

        /// <summary>
        /// Called when this Entity is added to a Scene, which only occurs immediately before each update.
        /// Keep in mind, other Entities to be added this frame may be added after this Entity.
        /// See Awake() for after all Entities are added, but still before the frame updates
        /// </summary>
        /// <param name="scene"></param>
        public virtual void Added( Scene scene )
        {
            Scene = scene;
            Components.EntityAdded(scene);
        }

        /// <summary>
        /// called when the entity is removed from a scene
        /// </summary>
        public virtual void Removed( Scene scene )
        {
            Components.EntityRemoved( scene );
            Scene = null;
        }

        #endregion
        
        #region Components
        /// <summary>
        /// Add component to entity's component list
        /// </summary>
        public void Add( Component component )
        {
            Components.Add( component );
        }

        /// <summary>
        /// Remove component from entity's component list
        /// </summary>
        /// <param name="component"></param>
        public void Remove( Component component )
        {
            Components.Remove( component );
        }

        /// <summary>
        /// Add set of components to entity's component list
        /// </summary>
        public void Add( params Component[] component )
        {
            Components.Add( component );
        }

        /// <summary>
        /// Remove set of components from entity's component list
        /// </summary>
        /// <param name="component"></param>
        public void Remove( params Component[] component )
        {
            Components.Remove( component );
        }

        /// <summary>
        /// Gets the first component in the component list of type T
        /// </summary>
        /// <typeparam name="T">Component Type</typeparam>
        public T Get<T>()
        {
            return Components.Get<T>();
        }

        /// <summary>
        /// Gets all components of type T without a List allocation
        /// </summary>
        public void GetAll<T>( List<T> list )
        {
            Components.GetAll( list );
        }

        /// <summary>
        /// Gets all the components of type T. The returned list can be put back in the 
        /// pool via ListPool.Free()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetAll<T>()
        {
            return Components.GetAll<T>();
        }
        #endregion

        #region Utils
        public T As<T>() where T : Entity
        {
            if ( this is T targetType )
            {
                return targetType;
            }
            return null;
        }

        public T SceneAs<T>() where T : Scene
        {
            if ( Scene != null )
            {
                if ( Scene is T targetType )
                {
                    return targetType;
                }
            }
            return null;
        }
        #endregion
    }
}
