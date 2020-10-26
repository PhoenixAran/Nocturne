
namespace Nocturne
{
    public class SpriteFrame
    {
        public ISprite Sprite;
        public int Delay;

        public SpriteFrame( ISprite sprite, int delay = 6 )
        {
            Sprite = sprite;
            Delay = delay;
        }
    }
}
