using System;

using OpenTK;

namespace thatGameEngine.Collision
{
    public class Ray
    {
        public Vector3 Position;
        public Vector3 Direction;

        public Ray(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;
        }

        public Nullable<Single> Intersects(BoundingSphere sphere)
        {
            Vector3 difference = sphere.Centre - this.Position;

            float differenceLengthSquared = difference.LengthSquared;
            float sphereRadiusSquared = sphere.Radius * sphere.Radius;

            float distanceAlongRay;


            if (differenceLengthSquared < sphereRadiusSquared)
            {
                return 0.0f;
            }

            Vector3.Dot(ref this.Direction, ref difference, out distanceAlongRay);
            
            if (distanceAlongRay < 0)
            {
                return null;
            }

            float dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;

            return (dist < 0) ? null : distanceAlongRay - (float?)Math.Sqrt(dist);
        }
    }
}
