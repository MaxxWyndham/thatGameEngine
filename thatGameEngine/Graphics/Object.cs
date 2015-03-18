using System;

using OpenTK;

namespace thatGameEngine
{
    public enum Physics
    {
        None,
        Falling
    }

    public abstract class Object
    {
        Single age;
        Single maxAge = Single.MaxValue;

        Physics physics = Physics.Falling;
        Vector3 forceAccum;

        Vector3 gravity = new Vector3(0, -9.2f, 0);
        Single damping = 0.995f;
        Single inverseMass;

        Vector3 position;
        Vector3 acceleration;
        Vector3 velocity;

        public bool Dead { get { return age > maxAge; } }

        public Vector3 Gravity { get { return gravity; } }

        #region Position
        public Vector3 Position { set { SetPosition(value); } }

        public Vector3 GetPosition()
        {
            return position;
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public void SetPosition(Single x, Single y, Single z)
        {
            this.position.X = x;
            this.position.Y = y;
            this.position.Z = z;
        }
        #endregion

        #region SetAcceleration
        public void SetAcceleration(Vector3 acceleration)
        {
            this.acceleration = acceleration;
        }

        public void SetAcceleration(Single x, Single y, Single z)
        {
            this.acceleration.X = x;
            this.acceleration.Y = y;
            this.acceleration.Z = z;
        }
        #endregion

        #region Velocity
        public Vector3 GetVelocity()
        {
            return velocity;
        }
        #endregion

        #region Mass
        public void SetMass(Single mass)
        {
            inverseMass = 1.0f / mass;
        }

        public void SetInverseMass(Single mass)
        {
            inverseMass = mass;
        }
        #endregion

        public Object()
        {
            if (physics == Physics.Falling) { SetAcceleration(Gravity); }
        }

        public void AddForce(Vector3 force)
        {
            forceAccum += force;
        }

        public void ClearAccumulator()
        {
            forceAccum = Vector3.Zero;
        }

        public virtual void Update(float dt)
        {
            age += dt;

            if (physics != Physics.None)
            {
                if (inverseMass < 0) { return; }

                position = Vector3.Add(position, velocity * dt);

                Vector3 rA = acceleration;
                rA = Vector3.Add(rA, forceAccum * inverseMass);
                velocity = Vector3.Add(velocity, rA * dt);

                velocity *= (Single)Math.Pow(damping, dt);

                ClearAccumulator();
            }
        }

        public virtual void Draw()
        {
        }

        public virtual void Die()
        {
            maxAge = 0;
        }
    }
}
