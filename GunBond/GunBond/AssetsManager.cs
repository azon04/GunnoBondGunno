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
        public static Dictionary<string, SpriteFont> FontList;

        public static void Initialize()
        {
            AssetsList = new Dictionary<string, Texture2D>();
            FontList = new Dictionary<string, SpriteFont>();
        }

        public static void LoadContent(ContentManager Content)
        {
            AssetsList.Add("orang1", Content.Load<Texture2D>("orang1"));
            AssetsList.Add("orang2", Content.Load<Texture2D>("orang2"));
            AssetsList.Add("orang3", Content.Load<Texture2D>("orang3"));
            AssetsList.Add("orang4", Content.Load<Texture2D>("orang4"));
            AssetsList.Add("pointer", Content.Load<Texture2D>("pointer"));
            AssetsList.Add("bullet", Content.Load<Texture2D>("bullet"));
            AssetsList.Add("background", Content.Load<Texture2D>("background"));

            FontList.Add("default", Content.Load<SpriteFont>("defaultFont"));
        }
    }
}
