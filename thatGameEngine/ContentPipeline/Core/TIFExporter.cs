﻿using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace thatGameEngine.ContentPipeline.Core
{
    public class TIFExporter : ContentExporter
    {
        public override void Export(Asset asset, string Path)
        {
            var texture = (asset as Texture);
            var b = texture.GetBitmap();

            SceneManager.Current.UpdateProgress(string.Format("Saving {0}", texture.Name));

            b.Save(Path, ImageFormat.Tiff);
        }
    }
}
