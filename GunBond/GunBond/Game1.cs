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

        public GameConnection connection;

        public string text = "";

        public string WhoseTurn = "";
        public Player myPlayer;

        public bool IsCreator = false;
        // constructor
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
            connection.Start();

            System.Diagnostics.Debug.WriteLine("Sent Init");
            Random rand = new Random();
            Vector2 vec = new Vector2(rand.Next(800), 300);
            Players.Add(connection.peerID,(myPlayer = new Player(connection.peerID,vec)));
            Message msg = new Message();
            msg.msgCode = Message.INIT;
            msg.playerPos0 = vec;
            msg.PeerID = connection.peerID;
            msg.playerTexture = myPlayer.getPlayerTextureNumber();
            connection.BroadCastMessage(msg.Construct());
            myPlayer.setIsCurrentPlayer(true);

            if (IsCreator)
            {
                WhoseTurn = connection.peerID;
                myPlayer.setFire(false);
            }
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

            Message msgAlive = new Message();
            msgAlive.msgCode = Message.KEEP_ALIVE;
            msgAlive.PeerID = connection.peerID;
            msgAlive.HP = myPlayer.getHealthPoint();
            connection.BroadCastMessage(msgAlive.Construct());

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
                        RemoveBullets.Add(bullet);
                        }
                    }
            }

            if ((Bullets.Count == 0 && WhoseTurn.Equals(connection.peerID)) && myPlayer.isFire())
            {
                Message msg = new Message();
                msg.msgCode = Message.NEXT_PLAYER;
                msg.PeerID = connection.peerID;
                msg.nextPlayer = connection.PeerIDs[(connection.PeerIDs.IndexOf(connection.peerID) + 1) % connection.PeerIDs.Count];
                connection.BroadCastMessage(msg.Construct());
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(AssetsManager.AssetsList["background"], Vector2.Zero, Color.White);
            spriteBatch.DrawString(AssetsManager.FontList["default"], text, Vector2.Zero, Color.Black);
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
                if (player.getHealthPoint() > 0)
                {
                    player.draw(spriteBatch);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
