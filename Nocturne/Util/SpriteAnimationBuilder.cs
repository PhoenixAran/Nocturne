
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Nocturne
{
    public class SpriteAnimationBuilder
    {

        private Dictionary<string, SpriteAnimation> animations;
        private List<SpriteFrame> spriteFrames;
        private Dictionary<int, ActionFrame> actionFrames;
        private LoopType loopType;
        private string key;


        public SpriteAnimationBuilder()
        {
            animations = new Dictionary<string, SpriteAnimation>();
            spriteFrames = new List<SpriteFrame>();
            actionFrames = new Dictionary<int, ActionFrame>();
        }

        // sprite frame
        public SpriteAnimationBuilder AddSpriteFrame( Subtexture subtexture, int delay = 6, float offsetX = 0, float offsetY = 0 )
        {

            Sprite sprite = new Sprite( subtexture, new Vector2(offsetX, offsetY) );
            spriteFrames.Add( new SpriteFrame( sprite, delay ) );
            return this;
        }

        public SpriteAnimationBuilder AddSpriteFrame( ISprite sprite, int delay = 6 )
        {
            spriteFrames.Add( new SpriteFrame( sprite, delay ) );
            return this;
        }

        public SpriteAnimationBuilder AddSpriteFrame( SpriteFrame spriteFrame )
        {
            spriteFrames.Add( spriteFrame );
            return this;
        }

        // action frame
        public SpriteAnimationBuilder AddActionFrame( int timeKey, Action<Entity> action )
        {
            actionFrames[timeKey] = new ActionFrame( action );
            return this;
        }

        public SpriteAnimationBuilder AddActionFrame( int timeKey, ActionFrame actionFrame )
        {
            actionFrames[timeKey] = actionFrame;
            return this;
        }

        // loop type
        public SpriteAnimationBuilder SetLoopType( LoopType loopType )
        {
            this.loopType = loopType;
            return this;
        }

        // string key
        public SpriteAnimationBuilder SetKey( string key )
        {
            this.key = key;
            return this;
        }


        public SpriteAnimationBuilder BuildAnimation( bool clearBuilderVariables = true )
        {
            // create a copy of the sprite frames and action frames collections since we need to reuse the collections
            List<SpriteFrame> spriteFramesCopy = new List<SpriteFrame>( spriteFrames );
            Dictionary<int, ActionFrame> actionFramesCopy = new Dictionary<int, ActionFrame>( actionFrames );
            animations.Add(key, new SpriteAnimation( spriteFramesCopy, actionFramesCopy, loopType ) );

            if ( clearBuilderVariables )
                return this.ClearBuilderVariables();

            return this;
        }

        /// <summary>
        /// clears the builder variables
        /// </summary>
        public SpriteAnimationBuilder ClearBuilderVariables()
        {
            spriteFrames.Clear();
            actionFrames.Clear();
            key = null;
            loopType = LoopType.Once;
            return this;
        }

        /// <summary>
        /// get the animations built with this builder
        /// </summary>
        public Dictionary<string, SpriteAnimation> GetAnimations()
        {
            return animations;
        }
    }
}
