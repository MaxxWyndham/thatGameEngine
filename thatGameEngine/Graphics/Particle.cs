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

        public Model model;

        Vector3 gravity = new Vector3(0, -9.2f, 0);

        public Particle()
        {
            var texture = new Texture();
            texture.CreateFromBitmap(new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("thatGameEngine.Data.entity.png")), "entity");

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

        public override void Update(Single dt)
        {
            base.Update(dt);

            Position += Velocity * dt;

            Vector3 rA = Acceleration;
            rA += gravity * inverseMass;
            Velocity += rA * dt;

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
