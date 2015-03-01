using System;
using System.Collections.Generic;

namespace thatGameEngine
{
    public class Material : Asset
    {
        Texture texture;

        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Material()
        {
            texture = new Texture();
        }
    }

    public class MaterialList : AssetList
    {
        public MaterialList() : base() { }

        public IEnumerator<Material> GetEnumerator()
        {
            return base.GetEnumerator<Material>();
        }
    }
}
