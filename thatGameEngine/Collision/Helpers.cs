using System;
using System.Collections.Generic;

using OpenTK;

namespace thatGameEngine.Collision
{
    public static class CollisionHelpers
    {
        public static float? RayIntersectsModel(Ray ray, Model model, Matrix4 modelTransform,
                                                    out bool insideBoundingSphere,
                                                    out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3,
                                                    out ModelMeshPart intersectsWith
                                                )
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;
            insideBoundingSphere = false;
            intersectsWith = null;

            Matrix4 inverseTransform = Matrix4.Invert(modelTransform);

            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            float? closestIntersection = null;

            foreach (var mesh in model.Meshes)
            {
                if (mesh.BoundingSphere.Intersects(ray) != null)
                {
                    insideBoundingSphere = true;

                    foreach (var part in mesh.MeshParts)
                    {
                        for (int i = 0; i < part.IndexBuffer.Data.Count; i += 3)
                        {
                            float? intersection;

                            RayIntersectsTriangle(ref ray,
                                                  part.VertexBuffer.Data[part.IndexBuffer.Data[i + 0]].Position,
                                                  part.VertexBuffer.Data[part.IndexBuffer.Data[i + 1]].Position,
                                                  part.VertexBuffer.Data[part.IndexBuffer.Data[i + 2]].Position,
                                                  out intersection);

                            if (intersection != null)
                            {
                                if ((closestIntersection == null) ||
                                    (intersection < closestIntersection))
                                {
                                    closestIntersection = intersection;

                                    vertex1 = Vector3.Transform(part.VertexBuffer.Data[part.IndexBuffer.Data[i + 0]].Position, modelTransform);
                                    vertex2 = Vector3.Transform(part.VertexBuffer.Data[part.IndexBuffer.Data[i + 1]].Position, modelTransform);
                                    vertex3 = Vector3.Transform(part.VertexBuffer.Data[part.IndexBuffer.Data[i + 2]].Position, modelTransform);

                                    intersectsWith = part;
                                }
                            }
                        }
                    }
                }
            }

            return closestIntersection;
        }

        static void RayIntersectsTriangle(ref Ray ray,
                          Vector3 vertex1,
                          Vector3 vertex2,
                          Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }
    }
}
