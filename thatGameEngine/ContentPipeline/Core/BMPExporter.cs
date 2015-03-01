using System;
using System.Drawing;
using System.Drawing.Imaging;

using thatGameEngine;
using thatGameEngine.ContentPipeline;

namespace thatGameEngine.ContentPipeline.Core
{
    public class BMPExporter : ContentExporter
    {
        public override void Export(Asset asset, string Path)
        {
            var texture = (asset as Texture);
            var b = texture.GetBitmap();

            SceneManager.Current.UpdateProgress(string.Format("Saving {0}", texture.Name));

            b.Save(Path, ImageFormat.Bmp);
        }
    }
}
