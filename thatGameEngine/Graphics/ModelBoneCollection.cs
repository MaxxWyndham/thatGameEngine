using System;
using System.Collections.Generic;

namespace thatGameEngine
{
    public class ModelBoneCollection : List<ModelBone>
    {
        public void ReIndex()
        {
            for (int i = 0; i < this.Count; i++) { this[i].Index = i; }
        }
    }
}
