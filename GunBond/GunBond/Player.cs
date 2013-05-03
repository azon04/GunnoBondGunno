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
        private int orientation; //0 = left, 1 = right

        // Enum jenis player (buat nentuin texture player)
        private enum jenisPlayer { player1, player2, player3, player4 };
        private jenisPlayer currentPlayer;

        // Enum buat fitur team + counternya
        private enum team { redTeam, blueTeam };
        private team myTeam;

        private static int redTeamCount;
        private static int blueTeamCount;

        // max player
        private static int max_player;

        // Texture player
        Texture2D playerTexture;

        // Texture Pointer
        Texture2D pointerTexture;

        // Current player dipake buat nentuin update posisi player atau angle player
        bool isCurrentPlayer = true;

        // constructor
        public Player()
        {
            // assign nilai default atribut
            peerID = "";
            position = new Vector2();
            angle = 3.14f;
            healthPoint = 100;
            Fire = false;
            orientation = 0;

            // randomize jenis player (dari segi texture)
            Random random = new Random();
            
            Array jenis = Enum.GetValues(typeof(jenisPlayer));
            currentPlayer = (jenisPlayer)jenis.GetValue(random.Next(jenis.Length));

            // randomize team
            Array tim = Enum.GetValues(typeof(team));
            myTeam = (team)tim.GetValue(random.Next(tim.Length));

            // check team max player
            if (myTeam == team.redTeam)
            {
                if (redTeamCount < max_player / 2)
                {
                    redTeamCount++;
                }
                else
                {
                    myTeam = team.blueTeam;
                    blueTeamCount++;
                }
            }
            else
            {
                if (blueTeamCount < max_player / 2)
                {
                    blueTeamCount++;
                }
                else
                {
                    myTeam = team.redTeam;
                    redTeamCount++;
                }
            }

            // Texturing pointer
            pointerTexture = AssetsManager.AssetsList["pointer"];
        }

        public Player(string _ID, Vector2 _position)
        {
            // assign nilai atribut dengan input dan nilai default
            peerID = _ID;
            position = _position;
            angle = 3.14f;
            healthPoint = 100;
            Fire = false;
            orientation = 0;

            Random random = new Random();
            
            // randomize jenis player (dari segi texture)
            Array jenis = Enum.GetValues(typeof(jenisPlayer));
            currentPlayer = (jenisPlayer)jenis.GetValue(random.Next(jenis.Length));

            // randomize team
            Array tim = Enum.GetValues(typeof(team));
            myTeam = (team)tim.GetValue(random.Next(tim.Length));

            // check team max player
            if (myTeam == team.redTeam)
            {
                if (redTeamCount < max_player / 2)
                {
                    redTeamCount++;
                }
                else
                {
                    myTeam = team.blueTeam;
                    blueTeamCount++;
                }
            }
            else 
            {
                if (blueTeamCount < max_player / 2)
                {
                    blueTeamCount++;
                }
                else
                {
                    myTeam = team.redTeam;
                    redTeamCount++;
                }
            }

            // Texturing pointer
            pointerTexture = AssetsManager.AssetsList["pointer"];
        }

        // getter
        public int getOrientation () {
            return orientation;
        }

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

        public Texture2D getPlayerTexture() 
        {
            return playerTexture;
        }

        public team getMyTeam() 
        {
            return myTeam;
        }

        public static int getMaxPlayer()
        {
            return max_player;
        }


        // setter
        public void setOrientation(int _orientation) 
        {
            orientation = _orientation;
        }

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

        public void setPlayerTexture(Texture2D _playerTexture) 
        {
            playerTexture = _playerTexture;
        }

        public void setTeam(team _team) 
        {
            myTeam = _team;
        }

        public static void setMaxPlayer(int _maxPlayer)
        {
            max_player = _maxPlayer;
        }

        // method lain { getIsCurrentPlayer(), isFire() }
        public bool isFire()
        {
            return Fire;
        }

        public void setFire(bool f)
        {
            Fire = f; 
        }

        // update & draw
        public void update(GameTime gameTime)
        {
            // keyboard state
            KeyboardState keys = Keyboard.GetState();
            
            // firing state true
            if (keys.IsKeyDown(Keys.Space) && isCurrentPlayer == true && !Fire) { 
                //Fire = true;
                int tempSpeed = 50;
                if ((angle <= 0.7f) || (angle >= 3.14f - 0.7f))
                {
                    Game1.GameObject.Bullets.Add(new Bullet(Game1.GameObject, position + new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), new Vector2(1.5f * tempSpeed, 1.5f * -tempSpeed), angle, new Vector2(0, 10)));
                }
                else if ((angle > 0.7f && angle <= 1.4f) || (angle < 3.14f - 0.7f && angle >= 3.14f - 1.4f))
                {
                    Game1.GameObject.Bullets.Add(new Bullet(Game1.GameObject, position + new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), new Vector2(tempSpeed, -tempSpeed), angle, new Vector2(0, 10)));
                }
                else if (angle > 1.4f || angle < 3.14f - 1.4f)
                {
                    Game1.GameObject.Bullets.Add(new Bullet(Game1.GameObject, position + new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), new Vector2(tempSpeed / 2, -tempSpeed / 2), angle, new Vector2(0, 10)));
                }
            }
            
            // geser player
            if (keys.IsKeyDown(Keys.A) && isCurrentPlayer == true && position.X > 0 && Fire == false)
            {
                // kalo belum keluar screen sebelah kiri
                position.X = position.X - 1;
                orientation = 0;
                if (angle < 1.67f) 
                {
                    angle = 3.14f - angle;
                }
            }
            else if (keys.IsKeyDown(Keys.A) && isCurrentPlayer == true && position.X <= 0 && Fire == false)
            {
                orientation = 0; // kalo udah keluar screen dia cuma ganti orientasi (?)
                if (angle < 1.67f)
                {
                    angle = 3.14f - angle;
                }
            }

            if (keys.IsKeyDown(Keys.D) && isCurrentPlayer == true && position.X < Game1.GameObject.GraphicsDevice.Viewport.Width - 100 && Fire == false)
            {
                // kalo belum keluar screen sebelah kanan
                position.X = position.X + 1;
                orientation = 1;
                if (angle > 1.67f)
                {
                    angle = 3.14f - angle;
                }
            }
            else if (keys.IsKeyDown(Keys.D) && isCurrentPlayer == true && position.X >= Game1.GameObject.GraphicsDevice.Viewport.Width - 100 && Fire == false)
            {
                orientation = 1; // kalo udah keluar screen dia cuma ganti orientasi (?)
                if (angle > 1.67f)
                {
                    angle = 3.14f - angle;
                }
            }

            // ubah angle
            if (keys.IsKeyDown(Keys.Q) && isCurrentPlayer == true && angle < 3.14f) { angle = angle + 0.05f; }
            if (keys.IsKeyDown(Keys.E) && isCurrentPlayer == true && angle > 0.0f) { angle = angle - 0.05f; }

            // ubah Texture player berdasarkan hasil randomize
            if (currentPlayer == jenisPlayer.player1) { playerTexture = AssetsManager.AssetsList["orang1"]; }
            if (currentPlayer == jenisPlayer.player2) { playerTexture = AssetsManager.AssetsList["orang2"]; }
            if (currentPlayer == jenisPlayer.player3) { playerTexture = AssetsManager.AssetsList["orang3"]; }
            if (currentPlayer == jenisPlayer.player4) { playerTexture = AssetsManager.AssetsList["orang4"]; }

        }

        public void draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();

            spriteBatch.DrawString(AssetsManager.FontList["default"], peerID, position, Color.White);
            
            if (orientation == 1) //hadap kanan
            {
                Rectangle s = new Rectangle((int)position.X + playerTexture.Width / 2, (int)position.Y + playerTexture.Height / 2, playerTexture.Width, playerTexture.Height);
                Rectangle r = new Rectangle(0, 0, playerTexture.Width, playerTexture.Height);

                spriteBatch.Draw(playerTexture, s, r, Color.White, 0.0f, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), SpriteEffects.FlipHorizontally, 0);

            }
            else // hadap kiri
            {
                Rectangle s = new Rectangle((int)position.X + playerTexture.Width / 2, (int)position.Y + playerTexture.Height / 2, playerTexture.Width, playerTexture.Height);
                Rectangle r = new Rectangle(0, 0, playerTexture.Width, playerTexture.Height);

                spriteBatch.Draw(playerTexture, s, r, Color.White, 0.0f, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), SpriteEffects.None, 0);

            }

            Rectangle p = new Rectangle((int)position.X + playerTexture.Width / 2, (int)position.Y + playerTexture.Height / 2, playerTexture.Width, playerTexture.Height);
            Rectangle q = new Rectangle(0, 0, playerTexture.Width, playerTexture.Height);

            spriteBatch.Draw(pointerTexture, p, q, Color.White, -angle, new Vector2(playerTexture.Width / 2, playerTexture.Height / 2), SpriteEffects.FlipHorizontally, 0);
                            
            //spriteBatch.End();

        }
    }
}
