using System;
using System.Drawing;
using System.Reflection;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace thatGameEngine
{
    public class Sprite : Object
    {
        Model model;
        Texture texture;

        public Sprite()
            : base()
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
        }

        public override void Draw()
        {
            base.Draw();

            if (model != null)
            {
                GL.PushMatrix();

                var position = Matrix4.CreateTranslation(GetPosition());

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
