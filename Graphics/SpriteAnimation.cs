using System.Collections.Generic;

namespace Nocturne
{
    public class SpriteAnimation
    {
        public List<SpriteFrame> SpriteFrames { get; }
        public Dictionary<int, ActionFrame> TimedActions { get; }
        public LoopType LoopType { get; }

        public SpriteAnimation(LoopType loopType = LoopType.Once) : this(new List<SpriteFrame>(), loopType ){ }

        public SpriteAnimation( List<SpriteFrame> frames, LoopType loopType = LoopType.Once ) : this( frames, new Dictionary<int, ActionFrame>(), loopType) { }

        public SpriteAnimation( List<SpriteFrame> frames, Dictionary<int, ActionFrame> timedActions, LoopType loopType = LoopType.Once )
        {
            SpriteFrames = frames;
            TimedActions = timedActions;
            LoopType = loopType;
        } 
    }
}
