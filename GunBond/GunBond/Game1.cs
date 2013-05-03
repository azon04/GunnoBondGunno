using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GunBond.Connection;

namespace GunBond
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Game1 GameObject;

        // Bullet in Game
        public List<Bullet> Bullets;
        public Dictionary<string, Player> Players;
        public List<Bullet> RemoveBullets;

        GameConnection connection;

        public Game1(GameConnection con)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Bullets = new List<Bullet>();
            Players = new Dictionary<string, Player>();
            RemoveBullets = new List<Bullet>();
            GameObject = this;
            connection = con;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            AssetsManager.Initialize();

            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 540;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            AssetsManager.LoadContent(Content);
            
            Players.Add("AAA",new Player("AAA",new Vector2(600,300)));
            Bullets.Add(new Bullet(this, Vector2.Zero, new Vector2(-600, 300), 0f, new Vector2(0, 10)));
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            foreach (Bullet bullet in Bullets)
            {
                bullet.Update(gameTime);
            }

            foreach (Player player in Players.Values)
            {
                player.update(gameTime);
            }

            // cek collision trus kurangin HP player yang kena
            foreach (Player player in Players.Values) 
            {
                foreach (Bullet bullet in Bullets) 
                {
                    if (bullet.isColide(player)) 
                    {
                        int HP = player.getHealthPoint();
                        HP = HP - 20;
                        player.setHealthPoint(HP);
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(AssetsManager.AssetsList["background"], Vector2.Zero, Color.White);
            spriteBatch.DrawString(AssetsManager.FontList["default"], "Gunno Bond Unyo", Vector2.Zero, Color.White);
            foreach (Bullet bullet in Bullets)
            {
                bullet.Draw(spriteBatch);
            }

            foreach (Bullet bullet in RemoveBullets)
            {
                Bullets.Remove(bullet);
            }

            foreach (Player player in Players.Values)
            {
                player.draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
