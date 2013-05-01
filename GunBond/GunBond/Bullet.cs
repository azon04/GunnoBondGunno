using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GunBond
{
    public class Bullet : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Vector2 position;
        Texture2D image;
        Vector2 V0, V, A;
        float sudut;
        Game1 game; 

        public Bullet(Game game, Bullet bullet) : base(game)
        {
            position = bullet.position;
            image = bullet.image;
            V0 = bullet.V0;
            V = bullet.V;
            sudut = bullet.sudut;
            A = bullet.A; 
        }

        public Bullet(Game game) : base(game)
        {
            position = new Vector2();
            V0 = new Vector2();
            V = new Vector2();
            sudut = 0;
            A = new Vector2();
        }

        public Bullet(Game game, Vector2 Position, Vector2 v_nol, float Sudut, Vector2 a) : base(game)
        {
            position = Position;
            image = AssetsManager.AssetsList["bullet"];
            V0 = v_nol;
            sudut = Sudut;
            A = a;
            V.X = V0.X * (float) Math.Cos(Sudut);
            V.Y = V0.Y * (float) Math.Sin(Sudut);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 v0
        {
            get { return V0; }
            set { V0 = value;  }
        }

        public Vector2 v
        {
            get { return V; }
            set { V = value; }
        }

        public float Sudut
        {
            get { return sudut;  }
            set { sudut = value;  }
        }

        public Vector2 a
        {
            get { return A; }
            set { A = value;  }
        }

        public void isColide (Player p)
        {
            Boolean isColiding = false;
            Rectangle bulletRect = new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);
            Rectangle playerRect = new Rectangle((int)p.getPosition().X, (int)p.getPosition().Y, 100,100);
            if (bulletRect.Intersects(playerRect))
            {
                p.setHealthPoint(p.getHealthPoint() - 20);
                game.Bullets.Remove(this);
            }
        }

        public override void Update(GameTime gameTime)
        {
            position += (V * gameTime.ElapsedGameTime.Milliseconds/150f);
            V += (A * gameTime.ElapsedGameTime.Milliseconds/150f);
            if (position.Y == (540 - image.Height))
            {
                game.Bullets.Remove(this);
            }
            base.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(image, new Rectangle((int)(position.X),(int)(position.Y),image.Width,image.Height), null, Color.White);
            spriteBatch.End();
        }
    }
}
