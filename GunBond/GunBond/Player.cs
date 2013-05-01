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

        // constructor
        public Player()
        {
            peerID = "";
            position = new Vector2();
            angle = 90.0f;
            healthPoint = 100;
            Fire = false;
        }

        public Player(string _ID, Vector2 _position)
        {
            peerID = _ID;
            position = _position;
            angle = 90.0f;
            healthPoint = 100;
            Fire = false;
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

        // setter
        public void getPeerID(string _ID)
        {
            peerID = _ID;
        }

        public void getPosition(Vector2 pos)
        {
            position = pos;
        }

        public void getAngle(float _Angle)
        {
            angle = _Angle;
        }

        public void getHealthPoint(int _HP)
        {
            healthPoint = _HP;
        }

        // method lain = isFire()
        public bool isFire()
        {
            return Fire;
        }

        // update & draw
        public void update(GameTime gameTime)
        {
            // keyboard state
            KeyboardState keys = Keyboard.GetState();

            // geser player
            if (keys.IsKeyDown(Keys.A)) { position.X = position.X - 1; }
            if (keys.IsKeyDown(Keys.D)) { position.X = position.X + 1; }

            // ubah angle
            if (keys.IsKeyDown(Keys.W)) { angle = angle + 0.1f; }
            if (keys.IsKeyDown(Keys.S)) { angle = angle - 0.1f; }

        }

        public void draw(SpriteBatch spriteBatch)
        {

        }

    }
}
