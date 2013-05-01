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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Bullets = new List<Bullet>();
            Players = new Dictionary<string, Player>();
            RemoveBullets = new List<Bullet>();
            GameObject = this;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            AssetsManager.Initialize();

            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 540;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            AssetsManager.LoadContent(Content);
            Bullets.Add(new Bullet(this, Vector2.Zero, new Vector2(10, 0), 0f, new Vector2(0, 10)));
            Players.Add("AAA",new Player("AAA",new Vector2(100,100)));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(AssetsManager.AssetsList["background"], Vector2.Zero, Color.White);
            spriteBatch.DrawString(AssetsManager.FontList["default"], "AKU KAMU DAN MEREKA", Vector2.Zero, Color.White);
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
