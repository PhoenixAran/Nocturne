using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nocturne
{
    /// <summary>
    /// stores spritesheets and sprite animation sets
    /// note that maploader has their own storage for tileset spritesheets
    /// </summary>
    public class SpriteBank
    {
        private Dictionary<string, SpriteSheet> _spriteSheetCache;
        private Dictionary<string, Dictionary<string, SpriteAnimation>> _animationSetCache;
        private Dictionary<string, ISprite> _spriteCache;

        public SpriteBank()
        {
            _spriteSheetCache = new Dictionary<string, SpriteSheet>( StringComparer.OrdinalIgnoreCase );
            _animationSetCache = new Dictionary<string, Dictionary<string, SpriteAnimation>>( StringComparer.OrdinalIgnoreCase );
            _spriteCache = new Dictionary<string, ISprite>( StringComparer.OrdinalIgnoreCase );
        }

        public void Initialize()
        {
            foreach ( Type type in Assembly.GetEntryAssembly().GetTypes() )
            {
                if ( type.GetCustomAttributes( typeof( SpriteContributerAttribute ), false ).Length > 0 )
                {
                    if ( !typeof( ISpriteBankContributer ).IsAssignableFrom( type ) )
                        throw new Exception( $"Type '{type.Name}' cannot is marked with SpriteBankContributer attribute but it does not implement ISpriteBankContributer" );
                    if ( type.GetConstructor( Type.EmptyTypes ) == null )
                        throw new Exception( $"Type '{type.Name}' cannot be used  by SpriteBank instance since it does not have a parameterless constructor" );
                    ISpriteBankContributer contributer = (ISpriteBankContributer)Activator.CreateInstance( type );
                    contributer.Initialize();
                    contributer.Contribute();
                }
            }
        }

        public bool HasSpriteSheet( string name ) => _spriteSheetCache.ContainsKey( name );

        public void RegisterSpriteSheet( string name, SpriteSheet spriteSheet )
        {
            Insist.IsFalse( _spriteSheetCache.ContainsKey( name ) );
            _spriteSheetCache[name] = spriteSheet;
        }

        public SpriteSheet GetSpriteSheet( string name )
        {
            Insist.IsTrue( _spriteSheetCache.ContainsKey( name ) );
            return _spriteSheetCache[name];
        }

        public bool HasSprite( string name ) => _spriteCache.ContainsKey( name );

        public void RegisterSprite( string name, ISprite sprite )
        {
            Insist.IsFalse( _spriteCache.ContainsKey( name ) );
            _spriteCache[name] = sprite;
        }

        public ISprite GetSprite( string name )
        {
            Insist.IsTrue( _spriteCache.ContainsKey( name ) );
            return _spriteCache[name];
        }

        public bool HasAnimationSet( string name ) => _animationSetCache.ContainsKey( name );

        public void RegisterAnimationSet( string name, Dictionary<string, SpriteAnimation> animationSet )
        {
            Insist.IsFalse( _animationSetCache.ContainsKey( name ) );
            _animationSetCache[name] = animationSet;
        }

        public Dictionary<string, SpriteAnimation> GetAnimationSet( string name )
        {
            Insist.IsTrue( _animationSetCache.ContainsKey( name ) );
            return _animationSetCache[name];
        }

        /// <summary>
        /// note that this does not unload Texture2D, you must call Engine.ContentManager.Unload yourself
        /// </summary>
        public void ClearCache()
        {
            _spriteSheetCache.Clear();
            _animationSetCache.Clear();
            _spriteCache.Clear();
        }
    }

    /// <summary>
    /// class marked with the class will be used on startup to populate the global spritebank instance.
    /// classes must implement ISpriteBankContributer
    /// </summary>
    public class SpriteContributerAttribute : Attribute
    {

    }

    public interface ISpriteBankContributer
    {
        /// <summary>
        /// load your needed spritesheets or texture2Ds here
        /// </summary>
        void Initialize();

        /// <summary>
        /// add your sprite animations here
        /// </summary>
        void Contribute();
    }

    /// <summary>
    /// Will implement ISpriteBankContributer.Initialize and handle loading and caching of 
    /// the required spritesheet for you.
    /// </summary>
    public abstract class SpriteBankContributer : ISpriteBankContributer
    {
        protected abstract string TexturePath { get; }
        protected abstract string SpriteSheetResourceKey { get; }
        protected abstract int SpriteWidth { get; }
        protected abstract int SpriteHeight { get; }
        protected abstract int SpriteSheetPadding { get; }
        protected abstract int SpriteSheetMargin { get; }
        
        protected SpriteSheet spriteSheet;
        protected SpriteAnimationBuilder animationBuilder;

        public void Initialize()
        {
            animationBuilder = new SpriteAnimationBuilder();
            if ( Engine.SpriteBank.HasSpriteSheet( SpriteSheetResourceKey ) )
            {
                spriteSheet = Engine.SpriteBank.GetSpriteSheet( SpriteSheetResourceKey );
            }
            else
            {
                Texture2D texture = Engine.ContentManager.Load<Texture2D>( TexturePath );
                spriteSheet = new SpriteSheet( texture, SpriteWidth, SpriteHeight, SpriteSheetPadding, SpriteSheetMargin );
                Engine.SpriteBank.RegisterSpriteSheet( SpriteSheetResourceKey, spriteSheet );
            }         
        }

        public abstract void Contribute();

    }
}
