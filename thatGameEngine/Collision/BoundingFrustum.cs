using System;

using OpenTK;

namespace thatGameEngine.Collision
{
    public class BoundingFrustum
    {
        Plane bottom, top, left, right, far, near;
        Matrix4 matrix;

        public Plane Bottom { get { return bottom; } }
        public Plane Top { get { return top; } }
        public Plane Left { get { return left; } }
        public Plane Right { get { return right; } }
        public Plane Far { get { return far; } }
        public Plane Near { get { return near; } }

        public BoundingFrustum(Matrix4 matrix)
        {
            this.matrix = matrix;
            GetPlanesFromMatrix(ref matrix, out near, out far, out left, out right, out top, out bottom);
        }

        private static void GetPlanesFromMatrix(ref Matrix4 matrix, out Plane near, out Plane far, out Plane left, out Plane right, out Plane top, out Plane bottom)
        {
            //http://www.chadvernon.com/blog/resources/directx9/frustum-culling/

            left = new Plane();
            left.Normal.X = matrix.M14 + matrix.M11;
            left.Normal.Y = matrix.M24 + matrix.M21;
            left.Normal.Z = matrix.M34 + matrix.M31;
            left.D = matrix.M44 + matrix.M41;
            left.Normalise();

            right = new Plane();
            right.Normal.X = matrix.M14 - matrix.M11;
            right.Normal.Y = matrix.M24 - matrix.M21;
            right.Normal.Z = matrix.M34 - matrix.M31;
            right.D = matrix.M44 - matrix.M41;
            right.Normalise();

            top = new Plane();
            top.Normal.X = matrix.M14 - matrix.M12;
            top.Normal.Y = matrix.M24 - matrix.M22;
            top.Normal.Z = matrix.M34 - matrix.M32;
            top.D = matrix.M44 - matrix.M42;
            top.Normalise();

            bottom = new Plane();
            bottom.Normal.X = matrix.M14 + matrix.M12;
            bottom.Normal.Y = matrix.M24 + matrix.M22;
            bottom.Normal.Z = matrix.M34 + matrix.M32;
            bottom.D = matrix.M44 + matrix.M42;
            bottom.Normalise();

            near = new Plane();
            near.Normal.X = matrix.M13;
            near.Normal.Y = matrix.M23;
            near.Normal.Z = matrix.M33;
            near.D = matrix.M43;
            near.Normalise();

            far = new Plane();
            far.Normal.X = matrix.M14 - matrix.M13;
            far.Normal.Y = matrix.M24 - matrix.M23;
            far.Normal.Z = matrix.M34 - matrix.M33;
            far.D = matrix.M44 - matrix.M43;
            far.Normalise();
        }

        public ContainmentType Contains(ref BoundingSphere sphere)
        {
            var result = PlaneIntersectionType.Front;
            var planeResult = PlaneIntersectionType.Front;

            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        planeResult = near.Intersects(ref sphere);
                        break;

                    case 1:
                        planeResult = far.Intersects(ref sphere);
                        break;

                    case 2:
                        planeResult = left.Intersects(ref sphere);
                        break;

                    case 3:
                        planeResult = right.Intersects(ref sphere);
                        break;

                    case 4:
                        planeResult = top.Intersects(ref sphere);
                        break;

                    case 5:
                        planeResult = bottom.Intersects(ref sphere);
                        break;
                }

                switch (planeResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;

                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }

            switch (result)
            {
                case PlaneIntersectionType.Intersecting:
                    return ContainmentType.Intersects;

                default:
                    return ContainmentType.Contains;
            }
        }
    }
}
