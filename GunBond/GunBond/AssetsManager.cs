using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GunBond
{
    class AssetsManager
    {
        public static Dictionary<string, Texture2D> AssetsList;
        
        public static void Initialize()
        {
            AssetsList = new Dictionary<string, Texture2D>();
        }

        public static void LoadContent(ContentManager Content)
        {

        }
    }
}
