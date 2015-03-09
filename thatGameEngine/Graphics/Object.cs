using System;

namespace thatGameEngine
{
    public abstract class Object
    {
        bool bDead = false;

        public bool Dead { get { return bDead; } }

        public virtual void Update(float dt)
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void Die()
        {
            bDead = true;
        }
    }
}
