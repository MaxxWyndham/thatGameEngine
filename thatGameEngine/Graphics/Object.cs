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

        Physics physics = Physics.None;
        Vector3 forceAccum;
        Vector3 torqueAccum;

        //Vector3 gravity = new Vector3(0, -9.2f, 0);
        Vector3 gravity = Vector3.Zero;
        Single linearDamping = 0.995f;
        Single angularDamping = 0.55f;
        Single inverseMass;
        Matrix4 inverseInertiaTensor;

        Vector3 lastFrameAcceleration;

        Matrix4 inverseInertiaTensorWorld;

        Vector3 position;
        Vector3 acceleration;
        Vector3 velocity;
        Quaternion orientation = Quaternion.Identity;
        Vector3 rotation;
        protected Matrix4 transformMatrix = Matrix4.Identity;

        bool isAwake = true;

        public bool Dead { get { return (position.Y < 0 || age > maxAge); } }
        public Vector3 Gravity { get { return gravity; } }

        public Single MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }

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

        #region Physics
        public Physics Physics { set { SetPhysics(value); } }

        public void SetPhysics(Physics physics)
        {
            this.physics = physics;

            if (physics == Physics.Falling) { SetAcceleration(Gravity); }
        }
        #endregion

        public void SetInertiaTensor(Matrix4 inertiaTensor)
        {
            inverseInertiaTensor = inertiaTensor.Inverted();
        }

        public void AddForce(Vector3 force)
        {
            forceAccum += force;
        }

        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            // Convert to coordinates relative to center of mass.
            Vector3 pt = point;
            pt -= position;

            forceAccum += force;
            torqueAccum += Vector3.Cross(pt, force);

            isAwake = true;
        }

        public void AddTorque(Vector3 torque)
        {
            torqueAccum += torque;
            isAwake = true;
        }

        public void ClearAccumulators()
        {
            forceAccum = Vector3.Zero;
            torqueAccum = Vector3.Zero;
        }

        public virtual void Update(float dt)
        {
            age += dt;

            if (physics != Physics.None)
            {
                if (!isAwake) { return; }

                lastFrameAcceleration = acceleration;
                lastFrameAcceleration = Vector3.Add(lastFrameAcceleration, forceAccum * inverseMass);

                Vector3 angularAcceleration = Vector3.Transform(torqueAccum, inverseInertiaTensorWorld);

                velocity = Vector3.Add(velocity, lastFrameAcceleration * dt);

                rotation = Vector3.Add(rotation, angularAcceleration * dt);

                velocity *= (Single)Math.Pow(linearDamping, dt);
                rotation *= (Single)Math.Pow(angularDamping, dt);

                position = Vector3.Add(position, velocity * dt);

                orientation = ApplyAngularVelocty(orientation, rotation * dt);

                //if (canSleep)
                //{
                //    Single currentMotion = Vector3.Dot(velocity, velocity) + Vector3.Dot(rotation, rotation);
                //    Single bias = (Single)Math.Pow(0.5f, dt);
                //    motion = bias * motion + (1 - bias) * currentMotion;

                //    if (motion < sleepEpsilon) { setAwake(false); }
                //    else if (motion > 10 * sleepEpsilon) { motion = 10 * sleepEpsilon; }
                //}
            }

            calculateDerivedData();
            ClearAccumulators();
        }

        private void calculateDerivedData()
        {
            orientation.Normalize();

            calculateTransformMatrix(ref transformMatrix, ref position, ref orientation);
            transformInertiaTensor(ref inverseInertiaTensorWorld, ref inverseInertiaTensor, ref transformMatrix);
        }

        private void calculateTransformMatrix(ref Matrix4 transformMatrix, ref Vector3 position, ref Quaternion rotation)
        {
            transformMatrix.M11 = 1 - 2 * orientation.Y * orientation.Y - 2 * orientation.Z * orientation.Z;
            transformMatrix.M21 =     2 * orientation.X * orientation.Y - 2 * orientation.W * orientation.Z;
            transformMatrix.M31 =     2 * orientation.X * orientation.Z + 2 * orientation.W * orientation.Y;
            transformMatrix.M41 = position.X;

            transformMatrix.M12 =     2 * orientation.X * orientation.Y + 2 * orientation.W * orientation.Z;
            transformMatrix.M22 = 1 - 2 * orientation.X * orientation.X - 2 * orientation.Z * orientation.Z;
            transformMatrix.M32 =     2 * orientation.Y * orientation.Z - 2 * orientation.W * orientation.X;
            transformMatrix.M42 = position.Y;

            transformMatrix.M13 =     2 * orientation.X * orientation.Z - 2 * orientation.W * orientation.Y;
            transformMatrix.M23 =     2 * orientation.Y * orientation.Z + 2 * orientation.W * orientation.X;
            transformMatrix.M33 = 1 - 2 * orientation.X * orientation.X - 2 * orientation.Y * orientation.Y;
            transformMatrix.M43 = position.Z;
        }

        private void transformInertiaTensor(ref Matrix4 iitWorld, ref Matrix4 iitBody, ref Matrix4 rotmat)
        {
            Single t4  = rotmat.M11 * iitBody.M11 + rotmat.M21 * iitBody.M12 + rotmat.M31 * iitBody.M13;
            Single t9  = rotmat.M11 * iitBody.M21 + rotmat.M21 * iitBody.M22 + rotmat.M31 * iitBody.M23;
            Single t14 = rotmat.M11 * iitBody.M31 + rotmat.M21 * iitBody.M32 + rotmat.M31 * iitBody.M33;
            Single t28 = rotmat.M12 * iitBody.M11 + rotmat.M22 * iitBody.M12 + rotmat.M32 * iitBody.M13;
            Single t33 = rotmat.M12 * iitBody.M21 + rotmat.M22 * iitBody.M22 + rotmat.M32 * iitBody.M23;
            Single t38 = rotmat.M12 * iitBody.M31 + rotmat.M22 * iitBody.M32 + rotmat.M32 * iitBody.M33;
            Single t52 = rotmat.M13 * iitBody.M11 + rotmat.M23 * iitBody.M12 + rotmat.M33 * iitBody.M13;
            Single t57 = rotmat.M13 * iitBody.M21 + rotmat.M23 * iitBody.M22 + rotmat.M33 * iitBody.M23;
            Single t62 = rotmat.M13 * iitBody.M31 + rotmat.M23 * iitBody.M32 + rotmat.M33 * iitBody.M33;

            iitWorld.M11 = t4  * rotmat.M11 + t9  * rotmat.M21 + t14 * rotmat.M31;
            iitWorld.M21 = t4  * rotmat.M12 + t9  * rotmat.M22 + t14 * rotmat.M32;
            iitWorld.M31 = t4  * rotmat.M13 + t9  * rotmat.M23 + t14 * rotmat.M33;
            iitWorld.M12 = t28 * rotmat.M11 + t33 * rotmat.M21 + t38 * rotmat.M31;
            iitWorld.M22 = t28 * rotmat.M12 + t33 * rotmat.M22 + t38 * rotmat.M32;
            iitWorld.M32 = t28 * rotmat.M13 + t33 * rotmat.M23 + t38 * rotmat.M33;
            iitWorld.M13 = t52 * rotmat.M11 + t57 * rotmat.M21 + t62 * rotmat.M31;
            iitWorld.M23 = t52 * rotmat.M12 + t57 * rotmat.M22 + t62 * rotmat.M32;
            iitWorld.M33 = t52 * rotmat.M13 + t57 * rotmat.M23 + t62 * rotmat.M33;
        }
    
        public virtual void Draw()
        {
        }

        public virtual void Die()
        {
            maxAge = 0;
        }

        public void Reset()
        {
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            orientation = Quaternion.Identity;
            transformMatrix = Matrix4.Identity;
        }

        private Quaternion ApplyAngularVelocty(Quaternion q, Vector3 v)
        {
            Quaternion qv = new Quaternion(v, 0);
            Quaternion initialAngularPosition = q;
            Quaternion changeInAngularPosition = Quaternion.Multiply(qv, initialAngularPosition);

            return Quaternion.Add(initialAngularPosition, changeInAngularPosition);
        }
    }
}
