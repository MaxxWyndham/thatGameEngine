using System;

using OpenTK;

namespace thatGameEngine.Collision
{
    public class Plane
    {
        public Vector3 Normal;
        public Single D;

        public PlaneIntersectionType Intersects(ref BoundingSphere sphere)
        {
            return CollisionHelpers.PlaneIntersectsSphere(this, ref sphere);
        }

        public void Normalise()
        {
            float magnitude = 1.0f / (float)(Math.Sqrt((Normal.X * Normal.X) + (Normal.Y * Normal.Y) + (Normal.Z * Normal.Z)));

            Normal.X *= magnitude;
            Normal.Y *= magnitude;
            Normal.Z *= magnitude;
            D *= magnitude;
        }
    }
}
