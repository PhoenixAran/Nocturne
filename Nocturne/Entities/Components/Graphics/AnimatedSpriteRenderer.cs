using System.Collections.Generic;

namespace Nocturne
{
    public class AnimatedSpriteRenderer : SpriteRenderer
    {
        public enum State
        {
            None,
            Running,
            Paused,
            Completed
        }


        public State AnimationState { get; private set; }
        public Dictionary<string, SpriteAnimation> Animations { get; private set; }
        public SpriteAnimation CurrentAnimation { get; private set; }
        public string CurrentAnimationKey { get; private set; }
        public int CurrentFrameIndex { get; private set; }
        public int CurrentTime { get; private set; }
        public bool Playing => AnimationState == State.Running;

        public AnimatedSpriteRenderer( bool enabled = true, bool visible = true ) : base( enabled, visible ) 
        {
            AnimationState = State.None;
            Animations = new Dictionary<string, SpriteAnimation>();
        }

        public void Play( string animationKey )
        {
            CurrentAnimation = Animations[animationKey];
            CurrentAnimationKey = animationKey;
            CurrentFrameIndex = 0;
            CurrentTime = 0;
            AnimationState = State.Running;
        }

        public bool IsAnimationActive( string name )
        {
            return CurrentAnimation != null && CurrentAnimationKey.Equals( name );
        }

        public void Pause()
        {
            AnimationState = State.Paused;
        }

        public void UnPause()
        {
            AnimationState = State.Running;
        }

        public void Stop()
        {
            CurrentAnimation = null;
            CurrentAnimationKey = null;
            CurrentFrameIndex = 0;
            AnimationState = State.None;
        }

        public void AddAnimation( string key, SpriteAnimation animation )
        {
            Animations.Add( key, animation );
        }

        public void SetAnimationSet( Dictionary<string, SpriteAnimation> animationSet )
        {
            Animations = animationSet;
        }

        public override void Update()
        {
            if ( !Playing )
                return;

            if ( CurrentAnimation.TimedActions.TryGetValue( CurrentTime, out ActionFrame actionFrame ) )
                actionFrame.InvokeAction( Entity );

            // some animations can have no spriteframes and just action frames
            if ( CurrentAnimation.SpriteFrames.Count == 0 )
                return;

            SpriteFrame currentFrame = CurrentAnimation.SpriteFrames[CurrentFrameIndex];

            CurrentTime += 1;
            if ( currentFrame.Delay <= CurrentTime)
            {
                CurrentTime = 0;
                CurrentFrameIndex += 1;
                if ( CurrentFrameIndex >= CurrentAnimation.SpriteFrames.Count )
                {
                    if ( CurrentAnimation.LoopType == LoopType.Once )
                    {
                        AnimationState = State.Completed;
                        CurrentFrameIndex -= 1;
                    }
                    else if ( CurrentAnimation.LoopType == LoopType.Cycle )
                    {
                        CurrentFrameIndex = 0;
                    }
                }
                currentFrame = CurrentAnimation.SpriteFrames[CurrentFrameIndex];
            }
            Sprite = currentFrame.Sprite;
        }
       
    }
}
