﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Roboquatic
{
    //The most basic enemy which moves left to right and shoots in a straight line
    public class BaseEnemy : Enemy
    {
        //Fields
        private int framesToFire;
        private int projectileSpeed;
        private Texture2D projectileSprite;
        private int shootingTimer;

        //Properties

        //Get property for framesToFire
        public int FramesToFire
        {
            get { return framesToFire; }
        }

        //Get propert for shootingTimer
        public int ShootingTimer
        {
            get { return shootingTimer; }
        }

        //BaseEnemy Constructor, uses Enemy constructor
        public BaseEnemy(Texture2D sprite, Rectangle position, int speed, int framesToFire, Texture2D projectileSprite, Rectangle hitBox)
            : base(sprite, position, speed, hitBox)
        {
            this.framesToFire = framesToFire;
            this.projectileSprite = projectileSprite;
            projectileSpeed = -8;
            health = 2;
            shootingTimer = 0;
            contactDamage = 1;
        }

        //Method

        //Checks if the enemy's shooting timer is great enough for it to be able to shoot
        public bool CanShoot()
        {
            if (shootingTimer >= framesToFire)
            {
                return true;
            }
            return false;
        }

        //Creates an enemy projectile and returns it
        public EnemyProjectile Shoot()
        {
            return new EnemyProjectile(projectileSprite, projectileSpeed, new Rectangle(position.X - 32, position.Y + position.Height / 2 - 16, 32, 32));
        }

        //Updates the enemy
        public override void Update(GameTime gameTime, Game1 game)
        {
            //Moves the enemy, then checks the enemy position to see if it needs to change the speed of the enemy, and moves the hitBox to the position.
            if (position.X <= game.ViewportWidth)
            {
                position.X -= speed;
            }
            else
            {
                position.X -= 4;
            }
            hitBox.X = position.X;
            hitBox.Y = position.Y + 4;
            if (position.X <= game.GraphicsDevice.Viewport.Width / 2)
            {
                speed = -1;
            }
            //Increments the shooting timer, checks if the enemy can shoot, and shoots a projectile if it can.
            if (position.X >= game.GraphicsDevice.Viewport.Width * 3 / 4 && speed < 0)
            {
                speed = 1;
            }
            if(position.X <= game.ViewportWidth + position.Width)
            {
                shootingTimer++;
            }
            if (CanShoot())
            {
                shootingTimer = 0;
                game.Projectiles.Add(Shoot());
            }
            //Increments a hit timer if it was hit, so that it becomes invisible for 5 frames to indicate being hit
            if (hit)
            {
                hitTimer++;
                if (hitTimer == 5)
                {
                    hit = false;
                    hitTimer = 0;
                }
            }
        }
    }
}
