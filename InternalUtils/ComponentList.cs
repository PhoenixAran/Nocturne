using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Nocturne
{
    public sealed class ComponentList
    {
        public enum LockModes { Open, Locked, Error }
        public Entity Entity { get; internal set; }

        private readonly List<Component> _components;
        private readonly List<RenderableComponent> _drawableComponents;
        private readonly List<Component> _toAdd;
        private readonly List<Component> _toRemove;

        private readonly HashSet<Component> _current;
        private readonly HashSet<Component> _adding;
        private readonly HashSet<Component> _removing;

        private LockModes lockMode = LockModes.Open;

        internal ComponentList( Entity entity )
        {
            Entity = entity;

            _components = new List<Component>();
            _drawableComponents = new List<RenderableComponent>();
            _toAdd = new List<Component>();
            _toRemove = new List<Component>();

            _current = new HashSet<Component>();
            _adding = new HashSet<Component>();
            _removing = new HashSet<Component>();
        }

        internal LockModes LockMode
        {
            get => lockMode;
            set => SetLockMode( value );
        }

        internal void SetLockMode( LockModes lockMode )
        {
            if ( _toAdd.Count > 0 )
            {
                for ( int i = 0; i < _toAdd.Count; ++i )
                {
                    Component component = _toAdd[i];
                    if ( !_current.Contains( component ) )
                    {
                        _current.Add( component );
                        _components.Add( component );
                        if ( component is RenderableComponent drawableComponent )
                        {
                            _drawableComponents.Add( drawableComponent );
                        }
                        component.Added( Entity );
                    }
                }

                _adding.Clear();
                _toAdd.Clear();
            }

            if ( _toRemove.Count > 0 )
            {
                for ( int i = 0; i < _toRemove.Count; ++i )
                {
                    Component component = _toRemove[i];
                    if ( _current.Contains( component ) )
                    {
                        _current.Remove( component );
                        _components.Remove( component );
                        if ( component is RenderableComponent drawableComponent )
                        {
                            _drawableComponents.Remove( drawableComponent );
                        }
                        component.Removed( Entity );
                    }
                }
                _removing.Clear();
                _toRemove.Clear();
            }
        }

        public void Add( Component component )
        {
            switch ( lockMode )
            {
                case LockModes.Open:
                    if ( !_current.Contains( component ) )
                    {
                        _current.Add( component );
                        _components.Add( component );
                        if ( component is RenderableComponent drawableComponent )
                        {
                            _drawableComponents.Add( drawableComponent );
                        }
                        component.Added( Entity );
                    }
                    break;
                case LockModes.Locked:
                    if ( !_current.Contains( component ) && !_adding.Contains( component ) )
                    {
                        _adding.Remove( component );
                        _toAdd.Add( component );
                    }
                    break;
                case LockModes.Error:
                    throw new System.Exception( "Cannot add or remove Entities at this time" );
            }
        }

        public void Remove( Component component )
        {
            switch ( lockMode )
            {
                case LockModes.Open:
                    if ( _current.Contains( component ) )
                    {
                        _current.Remove( component );
                        _components.Remove( component );
                        component.Removed( Entity );
                    }
                    break;
                case LockModes.Locked:
                    if ( _current.Contains( component ) && !_removing.Contains( component ) )
                    {
                        _removing.Add( component );
                        _toRemove.Add( component );
                    }
                    break;
                case LockModes.Error:
                    throw new System.Exception( "Cannot add or remove Entities at this time" );
            }
        }

        public void Add( IEnumerable<Component> components )
        {
            foreach ( Component component in components )
            {
                Add( component );
            }
        }

        public void Remove( IEnumerable<Component> components )
        {
            foreach ( Component component in components )
            {
                Remove( component );
            }
        }

        /// <summary>
        /// called when the entity gets added to a scene
        /// </summary>
        public void EntityAdded( Scene scene )
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                Component component = _components[i];
                component.EntityAdded( scene );
            }
        }

        /// <summary>
        /// called when the entity is awoken
        /// </summary>
        public void EntityAwake()
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                Component component = _components[i];
                component.EntityAwake();
            }
        }

        /// <summary>
        /// called when the entity is removed from the scene
        /// </summary>
        /// <param name="scene"></param>
        public void EntityRemoved( Scene scene )
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                Component component = _components[i];
                component.EntityRemoved( scene );
            }
        }

        public void SceneBegin( Scene scene )
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                Component component = _components[i];
                component.SceneBegin(scene);
            }
        }

        public void SceneEnd( Scene scene )
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                Component component = _components[i];
                component.SceneEnd( scene );
            }
        }
        public int Count => _components.Count;

        public Component this[int index]
        {
            get
            {
                if ( index < 0 || index >= _components.Count )
                    throw new IndexOutOfRangeException();
                else
                    return _components[index];
            }
        }

        internal void Update()
        {
            LockMode = LockModes.Locked;
            for ( int i = 0; i < _components.Count; ++i )
            {
                Component component = _components[i];
                if ( component.Enabled )
                {
                    component.Update();
                }
            }
            LockMode = LockModes.Open;
        }

        internal void Render( RectangleF cameraBounds )
        {
            LockMode = LockModes.Error;
            for ( int i = 0; i < _drawableComponents.Count; ++i )
            {
                RenderableComponent component = _drawableComponents[i];
                if ( component.Visible && cameraBounds.Intersects( component.Bounds ) )
                {
                    component.Render();
                }
            }
            LockMode = LockModes.Open;
        }

        internal void DebugRender()
        {
            LockMode = LockModes.Error;
            for ( int i = 0; i < _components.Count; ++i )
            {
                _components[i].DebugRender();
            }
            LockMode = LockModes.Open;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal void OnEntityTransformChanged(Transform.Component comp )
        {
            LockMode = LockModes.Locked;
            for ( int i = 0; i < _components.Count; ++i )
            {
                _components[i].OnEntityTransformChanged( comp );
            }

            LockMode = LockModes.Open;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>()
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                if ( _components[i] is T component )
                {
                    return component;
                }
            }
            return default(T);
        }

        /// <summary>
        /// Gets all the components of type T without a list allocation
        /// </summary>
        /// <typeparam name="T">1st type parameter</typeparam>
        /// <returns>The components</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void GetAll<T>( List<T> components )
        {
            for ( int i = 0; i < _components.Count; ++i )
            {
                if ( _components[i] is T component )
                {
                    components.Add( component );
                }
            }
        }

        /// <summary>
        /// Gets all the components of type T. The returned list can be 
        /// put back via ListPool.Free(list)
        /// </summary>
        /// <typeparam name="T">The 1st type parameter</typeparam>
        /// <returns>The components</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> GetAll<T>()
        {
            List<T> components = ListPool<T>.Obtain();
            GetAll( components );
            return components;
        }

    }
}
