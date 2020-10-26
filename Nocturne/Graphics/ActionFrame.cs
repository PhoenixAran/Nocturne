using System;

namespace Nocturne
{
    public class ActionFrame
    {
        public Action<Entity> Action { get; }

        public ActionFrame( Action<Entity> action )
        {
            Action = action;
        }

        public void InvokeAction( Entity entity )
        {
            Action?.Invoke( entity );
        }

        public static implicit operator Action<Entity>( ActionFrame actionFrame )
        {
            return actionFrame.Action;
        }

    }
}
