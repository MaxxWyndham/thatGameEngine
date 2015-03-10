using System;
using System.Drawing;
using System.Reflection;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace thatGameEngine
{
    public class Particle : thatGameEngine.Object
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public Vector3 Acceleration;
        Single damping = 0.995f;
        Single inverseMass = 1.0f;

        Vector3 forceAccum;

        static Model model;
        static Texture texture;

        Vector3 gravity = new Vector3(0, -9.2f, 0);

        public Particle()
        {
            if (texture == null)
            {
                texture = new Texture();
                texture.CreateFromBitmap(new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("thatGameEngine.Data.entity.png")), "entity");
            }

            if (model == null)
            {
                var sprite = new ModelMeshPart();
                sprite.AddVertex(new Vector3(-0.05f, 0.0f, -0.05f), Vector3.UnitY, new Vector2(0, 1));
                sprite.AddVertex(new Vector3(-0.05f, 0.0f, 0.05f), Vector3.UnitY, new Vector2(0, 0));
                sprite.AddVertex(new Vector3(0.05f, 0.0f, 0.05f), Vector3.UnitY, new Vector2(1, 0));
                sprite.AddVertex(new Vector3(0.05f, 0.0f, -0.05f), Vector3.UnitY, new Vector2(1, 1));

                sprite.AddVertex(new Vector3(0.05f, 0.0f, -0.05f), Vector3.UnitY, new Vector2(0, 1));
                sprite.AddVertex(new Vector3(0.05f, 0.0f, 0.05f), Vector3.UnitY, new Vector2(0, 0));
                sprite.AddVertex(new Vector3(-0.05f, 0.0f, 0.05f), Vector3.UnitY, new Vector2(1, 0));
                sprite.AddVertex(new Vector3(-0.05f, 0.0f, -0.05f), Vector3.UnitY, new Vector2(1, 1));
                sprite.IndexBuffer.Initialise();
                sprite.VertexBuffer.Initialise();
                sprite.Material = new Material { Name = "Entity.Asset", Texture = texture };
                sprite.PrimitiveType = PrimitiveType.Quads;
                var spritemesh = new ModelMesh();
                spritemesh.AddModelMeshPart(sprite);
                model = new Model();
                model.AddMesh(spritemesh);
            }

            Position = new Vector3((Single)(SceneManager.Current.Rando.NextDouble() * 8 - 4), 15, (Single)(SceneManager.Current.Rando.NextDouble() * 8 - 4));
        }

        public void SetMass(Single mass)
        {
            inverseMass = 1.0f / mass;
        }

        public void SetInverseMass(Single mass)
        {
            inverseMass = mass;
        }

        public void addForce(Vector3 force)
        {
            forceAccum += force;
        }

        public void ClearAccumulator()
        {
            forceAccum = Vector3.Zero;
        }

        public override void Update(Single dt)
        {
            base.Update(dt);

            Vector3.Add(Position, Velocity * dt);

            Vector3 rA = Acceleration;
            Vector3.Add(rA, forceAccum * inverseMass);
            Vector3.Add(Velocity, rA * dt);

            Velocity *= (Single)Math.Pow(damping, dt);

            ClearAccumulator();

            if (Position.Y < 0) { Position.Y = 0.0001f; }
        }

        public override void Draw()
        {
            base.Draw();

            if (model != null)
            {
                GL.PushMatrix();

                var position = Matrix4.CreateTranslation(Position);

                GL.MultMatrix(ref position);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                model.Draw();
                GL.Disable(EnableCap.Blend);

                GL.PopMatrix();
            }
        }
    }
}
