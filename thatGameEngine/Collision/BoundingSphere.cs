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

            sphere.Centre = box.Centre;
            sphere.Radius = (Single)Math.Max(
                                    Math.Abs(Math.Sqrt(Math.Pow(box.Min.X, 2) + Math.Pow(box.Min.Y, 2) + Math.Pow(box.Min.Z, 2))),
                                    Math.Abs(Math.Sqrt(Math.Pow(box.Max.X, 2) + Math.Pow(box.Max.Y, 2) + Math.Pow(box.Max.Z, 2)))
                                  );

            return sphere;
        }

        public Nullable<Single> Intersects(Ray ray)
        {
            return ray.Intersects(this);
        }
    }
}
