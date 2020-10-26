using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Nocturne
{
    public class Scene
    {
        public bool Paused;
        public float TimeActive;
        public float RawTimeActive;
        public bool DebugDrawEntities = false;
        public Camera Camera;

        public EntityList Entities { get; private set; }
        public TagLists TagLists { get; private set; }
        public event Action OnEndOfFrame;

        protected Dictionary<int, double> actualDepthLookup;

        public Scene() : this( new Camera() ) { }

        public Scene( Camera camera )
        {
            Camera = camera;
            Entities = new EntityList(this);
            TagLists = new TagLists();
            actualDepthLookup = new Dictionary<int, double>();
        }

        /// <summary>
        /// Add your entities here
        /// </summary>
        public virtual void Initialize()
        {

        }

        public virtual void Begin()
        {
            Entities.SceneBegin();
        }

        public virtual void BeforeUpdate()
        {
            if ( !Paused )
            {
                TimeActive += Time.DeltaTime;
            }
            RawTimeActive += Time.UnscaledDeltaTime;

            Entities.UpdateLists();
            TagLists.UpdateLists();
        }

        public virtual void Update()
        {
            if ( !Paused )
            {
                Entities.Update();
            }
        }

        public virtual void AfterUpdate()
        {
            if ( OnEndOfFrame != null )
            {
                OnEndOfFrame.Invoke();
                OnEndOfFrame = null;
            }
        }
        public virtual void End()
        {
            Entities.SceneEnd();
            foreach ( Entity entity in Entities )
            {
                Engine.Pooler.Return( entity );
            }
        }

        public virtual void BeforeRender()
        {
            
        }

        public virtual void Render()
        {
            Draw.SpriteBatch.Begin( SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Camera.Matrix * Engine.ScreenMatrix );
            Entities.Render( Camera.Bounds );
            if ( DebugDrawEntities )
            {
                Entities.DebugRender();
            }
            Draw.SpriteBatch.End();
        }

        public virtual void AfterRender()
        {

        }

        #region Entity Shortcuts
        public virtual void Add( Entity entity )
        {
            Entities.Add( entity );
            SetActualDepth( entity );
        }

        public virtual void Remove( Entity entity )
        {
            Entities.Remove( entity );
            Engine.Pooler.Return( entity );
        }

        /// <summary>
        /// Returned list can be freed via ListPool.free
        /// </summary>
        public List<Entity> GetEntitiesByTagMask( int mask )
        {
            List<Entity> list = ListPool<Entity>.Obtain();
            foreach ( var entity in Entities )
                if ( ( entity.Tag & mask ) != 0 )
                    list.Add( entity );
            return list;
        }

        /// <summary>
        /// Returned list can be freed via ListPool.free
        /// </summary>
        public List<Entity> GetEntitiesExcludingTagMask( int mask )
        {
            List<Entity> list = new List<Entity>();
            foreach ( var entity in Entities )
                if ( ( entity.Tag & mask ) == 0 )
                    list.Add( entity );
            return list;
        }


        #endregion

        #region Entity Depth
        public void SetActualDepth( Entity entity )
        {
            const double theta = .00001f;
            double add = 0;

            if ( actualDepthLookup.TryGetValue( entity.depth, out add ) )
            {
                actualDepthLookup[entity.depth] += theta;
            }
            else
            {
                actualDepthLookup.Add( entity.depth, theta );
            }

            entity.actualDepth = entity.depth - add;

            // Mark lists unsorted
            for ( int i = 0; i < BitTag.TotalTags; ++i )
            {
                if ( entity.TagCheck( 1 << i ) )
                {
                    TagLists.MarkUnsorted( i );
                }
            }

        }
        #endregion

        #region Utils
        public ContentManager ContentManager => Engine.ContentManager;
        #endregion

        #region GraphicsDevice Callbacks
        public virtual void HandleGraphicsReset()
        {

        }

        public virtual void HandleGraphicsCreate()
        {

        }

        public virtual void GainFocus()
        {

        }

        public virtual void LoseFocus()
        {

        }
        #endregion
    }
}
