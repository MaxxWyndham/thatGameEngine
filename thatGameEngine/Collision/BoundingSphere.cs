using System;

using OpenTK;

namespace thatGameEngine.Collision
{
    public class BoundingSphere
    {
        public Vector3 Centre;
        public Single Radius;

        public static BoundingSphere CreateFromBoundingBox(BoundingBox box)
        {
            var sphere = new BoundingSphere();

            sphere.Centre = Vector3.Lerp(box.Min, box.Max, 0.5f);
            sphere.Radius = (box.Min - box.Max).Length * 0.5f;

            return sphere;
        }

        public Nullable<Single> Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }
    }
}
