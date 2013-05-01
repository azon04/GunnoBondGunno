using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GunBond
{
    public class Player
    {
        // atribut
        private string peerID;
        private Vector2 position;
        private float angle;
        private int healthPoint;
        private bool Fire;

        // Enum jenis player (buat nentuin texture player)
        public enum jenisPlayer { player1, player2, player3, player4 };
        public jenisPlayer currentPlayer;

        // Texture player
        Texture2D playerTexture;

        // Rectangle tempat naroh player
        Rectangle playerRectangle;

        // Current player dipake buat nentuin update posisi player atau angle player
        bool isCurrentPlayer;

        // constructor
        public Player()
        {
            // assign nilai default atribut
            peerID = "";
            position = new Vector2();
            angle = 90.0f;
            healthPoint = 100;
            Fire = false;

            // randomize jenis player (dari segi texture)
            Array jenis = Enum.GetValues(typeof(jenisPlayer));
            Random random = new Random();
            currentPlayer = (jenisPlayer) jenis.GetValue(random.Next(jenis.Length));
        }

        public Player(string _ID, Vector2 _position)
        {
            // assign nilai atribut dengan input dan nilai default
            peerID = _ID;
            position = _position;
            angle = 90.0f;
            healthPoint = 100;
            Fire = false;

            // randomize jenis player (dari segi texture)
            Array jenis = Enum.GetValues(typeof(jenisPlayer));
            Random random = new Random();
            currentPlayer = (jenisPlayer)jenis.GetValue(random.Next(jenis.Length));
        }

        // getter

        public string getPeerID()
        {
            return peerID;
        }

        public Vector2 getPosition()
        {
            return position;
        }

        public float getAngle()
        {
            return angle;
        }

        public int getHealthPoint()
        {
            return healthPoint;
        }

        public bool getIsCurrentPlayer()
        {
            return isCurrentPlayer;
        }
        
        // setter
        public void setPeerID(string _ID)
        {
            peerID = _ID;
        }

        public void setPosition(Vector2 pos)
        {
            position = pos;
        }

        public void setAngle(float _Angle)
        {
            angle = _Angle;
        }

        public void setHealthPoint(int _HP)
        {
            healthPoint = _HP;
        }

        public void setIsCurrentPlayer(bool curr)
        {
            isCurrentPlayer = curr;
        }

        // method lain { getIsCurrentPlayer(), isFire() }
        public bool isFire()
        {
            return Fire;
        }

        // update & draw
        public void update(GameTime gameTime)
        {
            // keyboard state
            KeyboardState keys = Keyboard.GetState();
            
            // firing state true
            if (keys.IsKeyDown(Keys.Space) && isCurrentPlayer == true) { Fire = true; }
            
            // geser player
            if (keys.IsKeyDown(Keys.A) && isCurrentPlayer == true) { position.X = position.X - 1; }
            if (keys.IsKeyDown(Keys.D) && isCurrentPlayer == true) { position.X = position.X + 1; }

            // ubah angle
            if (keys.IsKeyDown(Keys.W) && isCurrentPlayer == true) { angle = angle + 0.1f; }
            if (keys.IsKeyDown(Keys.S) && isCurrentPlayer == true) { angle = angle - 0.1f; }

            // ubah Texture player berdasarkan hasil randomize
            if (currentPlayer == jenisPlayer.player1) { playerTexture = AssetsManager.AssetsList["orang1"]; }
            if (currentPlayer == jenisPlayer.player2) { playerTexture = AssetsManager.AssetsList["orang2"]; }
            if (currentPlayer == jenisPlayer.player3) { playerTexture = AssetsManager.AssetsList["orang3"]; }
            if (currentPlayer == jenisPlayer.player4) { playerTexture = AssetsManager.AssetsList["orang4"]; }

        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.DrawString(AssetsManager.FontList["default"], peerID, Vector2.Zero, Color.White);
            spriteBatch.Draw(playerTexture, Vector2.Zero, Color.White);
            
            spriteBatch.End();

        }
    }
}
