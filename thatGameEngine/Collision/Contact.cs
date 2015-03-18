using System;

using OpenTK;

using thatGameEngine;

namespace thatGameEngine.Collision
{
    public class Contact
    {
        Object a;
        Object b;
        Single resitution;
        Vector3 contactNormal;

        public void Resolve(Single dt)
        {
            resolveVelocity(dt);
        }

        public Single CalculateSeparatingVelocity()
        {
            Vector3 relativeVelocity = a.GetVelocity();
            if (b != null) { relativeVelocity -= b.GetVelocity(); }

            return Vector3.Dot(relativeVelocity, contactNormal);
        }

        private void resolveVelocity(Single dt)
        {
        }
    }
}
