
using System.Runtime.CompilerServices;

//inspired by https://bitbucket.org/MattThorson/monocle-engine

namespace Nocturne
{
    public abstract class Component
    {
        /// <summary>
        /// the Entity this component is attached to
        /// </summary>
        public Entity Entity;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                SetEnabled( value );
            }
        }

        private bool _enabled;

        /// <summary>
        /// shortcut to the Entity's transform
        /// </summary>
        public Transform Transform
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => Entity.Transform;
        }

        /// <summary>
        /// called when the entity's position changes. This allows components to be aware
        /// that they have moved due to parent entity moving.
        /// </summary>
        /// <param name="comp"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnEntityTransformChanged( Transform.Component comp )
        {
            
        }

        public Component( bool enabled = true )
        {
            Enabled = enabled;
        }

        public void SetEnabled(bool value)
        {
            _enabled = value;
            if ( value )
            {
                OnEnabled();
            }
            else
            {
                OnDisabled();
            }
        }

        public virtual void OnEnabled() { }
        public virtual void OnDisabled() { }

        #region Life Cycle
        /// <summary>
        /// Called when the scene begins
        /// </summary>
        public virtual void SceneBegin( Scene scene )
        {

        }

        /// <summary>
        /// Called when the scene ends
        /// </summary>
        public virtual void SceneEnd( Scene scene )
        {

        }

        /// <summary>
        /// Called when this component gets added to an entity
        /// </summary>
        public virtual void Added( Entity entity )
        {
            Entity = entity;
        }

        /// <summary>
        /// Called when this component gets removed from an entity
        /// </summary>
        public virtual void Removed( Entity entity )
        {
            Entity = null;
        }

        /// <summary>
        /// Called when entity gets added to a scene
        /// </summary>
        public virtual void EntityAdded( Scene scene )
        {

        }

        /// <summary>
        /// Called when entity gets removed from a scene
        /// </summary>
        public virtual void EntityRemoved( Scene scene )
        {

        }

        public virtual void EntityAwake()
        {

        }
        #endregion
        public virtual void Update()
        {

        }

        public virtual void DebugRender()
        {

        }

        public void RemoveSelf()
        {
            Entity?.Remove( this );
        }

        #region Utils
        public T As<T>() where T : Component
        {
            if ( this is T targetType )
            {
                return targetType;
            }
            return null;
        }

        public T EntityAs<T>() where T : Entity
        {
            if ( Entity != null )
            {
                return Entity.As<T>();
            }

            return null;
        }

        public Scene Scene => Entity?.Scene;

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
