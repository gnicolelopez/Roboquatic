﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;



namespace Roboquatic
{
    enum GameState
    {
        Menu,
        Game,
        Pause,
        GameOver,
        Settings
    }

    public class Game1 : Game
    {
        #region Field Declaration
        //Fields
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player player;
        private MouseState mouse;
        private int mouseX;
        private int mouseY;
        private bool keyboardControls;
        private Texture2D backdrop;
        private Texture2D backdropSwap;
        private Rectangle backdropPos;
        private Rectangle backdropSwapPos;
        private const int oldWidth = 800;
        private int viewportWidth;
        private const int oldHeight = 480;
        private int viewportHeight;
        private Matrix drawScale;
        private float scaleX;
        private float scaleY;
        private int timer;
        private bool increment;
        private List<Projectile> projectiles;
        private Hud hud;
        private int pastCheckpoints = 0;
        private Upgrades upgrade;
        private Texture2D pauseText;
        private List<HealthPickup> pickups;
        

        //Test for FileIO
        private FileIO fileIO;
        List<Enemy>[] enemiesToAdd;
        private bool addedFormation1;
        private bool addedFormation2;
        private bool addedBoss;
        private List<Enemy> enemies;
        private Texture2D baseEnemySprite;
        private Texture2D bossEnemySprite;
        private Texture2D baseEnemyProjectileSprite;
        private Texture2D aimedEnemySprite;
        private Texture2D staticEnemySprite;
        private Texture2D homingEnemySprite;
        private Random rng;
        private GameState currentState;
        private KeyboardState previousKbState;
        private SpriteFont font;
        private GameState previousState;
        private EnemyManager enemyManager;
        private ProjectileManager projectileManager;
        private Texture2D laserSprite;
        private SpriteFont text;
        private SpriteFont upgradeFont;

        // Buttons' fields
        private List<Button> buttons = new List<Button>();
        private Texture2D startButton;
        private Texture2D controlsButton;
        private Texture2D mouseButton;
        private Texture2D kbButton;
        private Texture2D backButton;
        private Texture2D menuButton;
        private Texture2D resumeButton;
        private Texture2D continueButton;

        // Checkpoints' fields
        private List<Checkpoint> deactivedCheckpoints = new List<Checkpoint>();
        private Checkpoint activeCheckpoint;
        private Checkpoint currentCheckpoint;
        private Texture2D checkpoint;
        private bool spawnEnemy;

        // Game time
        private float time;

        // Title page
        private Texture2D titlePage;
        private Texture2D titleBubblesLoop;
        private Texture2D titleBubbles;
        private Rectangle titleBubblesPos;
        private Rectangle titleBubblesSwapPos;
        private Texture2D logo;
        private Texture2D teamName;
        private Vector2 logoVect;
        private Vector2 origin;
        private Vector2 scale;
        private int sizeChanges = 0;
        private float rotation;
        private float rotationAmount = .0005f;

        //Upgrade fields
        private Texture2D healthUpgrade;
        private Texture2D speedUpgrade;
        private Texture2D damageUpgrade;
        //Audio fields
        private Song audio;

        //Upgrade fields
        private Rectangle posHealth;
        private Rectangle posDamage;
        private Rectangle posSpeed;
        #endregion

        #region Properties

        //Properties

        //Get set for increment
        public bool Increment
        {
            get { return increment; }
            set { increment = value; }
        }

        //Get set for currentCheckpoint
        public Checkpoint CurrentCheckpoint
        {
            get { return currentCheckpoint; }
            set { currentCheckpoint = value; }
        }

        //Get property for total game time in seconds
        public float Time
        {
            get { return time; }
            set { time = value; }
        }

        //Get set property for enemies
        public List<Enemy> Enemies
        {
            get { return enemies; }
            set { enemies = value; }
        }

        //Get set property for projectiles
        public List<Projectile> Projectiles
        {
            get { return projectiles; }
            set { projectiles = value; }
        }

        //Get set for pickups
        public List<HealthPickup> Pickups
        {
            get { return pickups; }
            set { pickups = value; }
        }

        //Get property for enemyManager
        public EnemyManager EnemyManager
        {
            get { return enemyManager; }
        }

        //Get property for projectileManager
        public ProjectileManager ProjectileManager
        {
            get { return projectileManager; }
        }

        //Get property for player
        public Player Player
        {
            get { return player; }
            set { player = value; }
        }

        // Get set property for player health
        public int PlayerHealth
        {
            get { return player.Health; }
            set { player.Health = value; }
        }

        // Get property for player speed
        public float PlayerSpeed
        {
            get { return player.Speed; }
        }

        // Get property for player damage
        public int PlayerDamage
        {
            get { return player.ProjectileDamage; }
        }

        public Upgrades Upgrade
        {
            get { return upgrade; }
            set { upgrade = value; }
        }

        public Rectangle PosHealth
        {
            get { return posHealth; }
            set { posHealth = value; }
        }

        public Rectangle PosDamage
        {
            get { return posDamage; }
            set { posHealth = value; }
        }
        public Rectangle PosSpeed
        {
            get { return posSpeed; }
            set { posHealth = value; }

        }
        // Get property for player position
        public Rectangle PlayerPosition
        {
            get { return player.Position; }
        }

        // Get set Property for stop enemies from spawning when player reached a checkpoint
        public bool SpawnEnemy
        {
            get { return spawnEnemy; }
            set { spawnEnemy = value; }
        }

        //Get property for viewport width
        public int ViewportWidth
        {
            get { return viewportWidth; }
        }

        //Get property for viewport width
        public int ViewportHeight
        {
            get { return viewportHeight; }
        }

        //Get property for font
        public SpriteFont Font
        {
            get { return font; }
        }

        //Get property for rng
        public Random RNG
        {
            get { return rng; }
        }
        #endregion

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        #region Intialization
        protected override void Initialize()
        {
            // Initializing variables
            viewportWidth = GraphicsDevice.DisplayMode.Width;
            viewportHeight = GraphicsDevice.DisplayMode.Height;
            //Uncomment for old screen size (and comment out the fullscreen command below)
            //viewportWidth = 800;
            //viewportHeight = 480;
            scaleX = ((float)viewportWidth) / oldWidth;
            scaleY = ((float)viewportHeight) / oldHeight;
            if (scaleX < scaleY)
            {
                scaleY = scaleX;
            }
            else
            {
                scaleX = scaleY;
            }
            viewportWidth = (int)(scaleX * oldWidth);
            viewportHeight = (int)(scaleY * oldHeight);
            GlobalScalars.x = scaleX;
            GlobalScalars.y = scaleY;
            keyboardControls = false;
            backdropPos = new Rectangle(0, 0, viewportWidth, viewportHeight);
            backdropSwapPos = new Rectangle(viewportWidth, 0, viewportWidth, viewportHeight);
            timer = 0;
            projectiles = new List<Projectile>(1);
            enemies = new List<Enemy>(1);
            rng = new Random();
            enemyManager = new EnemyManager(enemies);
            projectileManager = new ProjectileManager(projectiles);
            currentState = GameState.Menu;
            spawnEnemy = true;
            addedFormation1 = false;
            addedFormation2 = false;
            addedBoss = false;
            currentCheckpoint = null;
            hud = new Hud(this);
            increment = true;
            enemiesToAdd = new List<Enemy>[10];
            pickups = new List<HealthPickup>();
            logoVect = new Vector2(400 * scaleX, viewportHeight - 390 * scaleY);
            origin = new Vector2(400, 130);
            scale = new Vector2(.78f * scaleX,.78f * scaleY);
            _graphics.PreferredBackBufferWidth = viewportWidth;
            _graphics.PreferredBackBufferHeight = viewportHeight;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            titleBubblesPos = new Rectangle(0, 0, viewportWidth, viewportHeight);
            titleBubblesSwapPos = new Rectangle(0, viewportHeight, viewportWidth, viewportHeight);

            //Initializes position for upgrades

            posHealth = new Rectangle((int)(viewportWidth / 2 - 24 * scaleX), (int)(100 * scaleY), (int)(48 * scaleX), (int)(48 * scaleY));
            posDamage = new Rectangle((int)(viewportWidth / 2 - 224 * scaleX), (int)(100 * scaleY), (int)(48 * scaleX), (int)(48 * scaleY));
            posSpeed = new Rectangle((int)(viewportWidth / 2 + 176 * scaleX), (int)(100 * scaleY), (int)(48 * scaleX), (int)(48 * scaleY));

            base.Initialize();
        }
        #endregion

        #region Content Loading
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loading in textures and initializing the player
            player = new Player(1, 20, 10, new Rectangle(0, 0, 48, 48), 7, 1, Content.Load<Texture2D>("bubble"), new Rectangle(0, 0, 47, 27));
            player.Sprite = Content.Load<Texture2D>("PlayerFishSprite");

            // Load background 
            backdrop = Content.Load<Texture2D>("bg2");
            backdropSwap = Content.Load<Texture2D>("bg2");

            // Load enemies & misc
            bossEnemySprite = Content.Load<Texture2D>("BossSprite1");
            baseEnemySprite = Content.Load<Texture2D>("EnemyFishSprite2");
            baseEnemyProjectileSprite = Content.Load<Texture2D>("bubble");
            aimedEnemySprite = Content.Load<Texture2D>("EnemyFishSprite1");
            staticEnemySprite = Content.Load<Texture2D>("EnemyFishSprite3");
            homingEnemySprite = Content.Load<Texture2D>("EnemyFishSprite4");
            font = Content.Load<SpriteFont>("text");
            laserSprite = Content.Load<Texture2D>("laser");
            pauseText = Content.Load<Texture2D>("Pause");

            // Load Buttons
            startButton = Content.Load<Texture2D>("OnStart");
            controlsButton = Content.Load<Texture2D>("OnControls");
            kbButton = Content.Load<Texture2D>("KeyboardControls");
            mouseButton = Content.Load<Texture2D>("MouseControls");
            titlePage = Content.Load<Texture2D>("titlebg");
            titleBubbles = Content.Load<Texture2D>("titleBubbles");
            titleBubblesLoop = Content.Load<Texture2D>("titleBubbles");
            logo = Content.Load<Texture2D>("titleLogo");
            teamName = Content.Load<Texture2D>("titleteamname");
            backButton = Content.Load<Texture2D>("backButton2");
            menuButton = Content.Load<Texture2D>("menuButton2");
            resumeButton = Content.Load<Texture2D>("resumeButton2");
            continueButton = Content.Load<Texture2D>("menuButton2");

            // Load checkpoint
            checkpoint = Content.Load<Texture2D>("CheckpointFlag");

            //Load Upgrades 

            healthUpgrade = Content.Load<Texture2D>("UpgradeHealth");
            damageUpgrade = Content.Load<Texture2D>("UpgradeDamage");
            speedUpgrade = Content.Load<Texture2D>("UpgradeSpeed");

            //Load font for upgrades

            upgradeFont = Content.Load<SpriteFont>("upgrade font");

            //Initializes the fileIO class with all the data and assets that it needs
            fileIO = new FileIO(rng, viewportHeight, viewportWidth, baseEnemySprite, baseEnemyProjectileSprite, aimedEnemySprite, staticEnemySprite, homingEnemySprite);
            //Loads in the file
            fileIO.LoadFormation("EnemyFormations.txt");
            
            // Add buttons
            // Menu
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(93 * scaleX), viewportHeight / 2 - (int)(49 * scaleY), 187, 65),
                startButton,
                startButton
                ));

            // Settings
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(93 * scaleX), viewportHeight / 2 + (int)(39 * scaleY), 187, 65),
                controlsButton,
                controlsButton
                ));

            // Settings Keyboard
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(250 * scaleX), viewportHeight / 2 - (int)(100 * scaleY), 187, 187),
                kbButton,
                kbButton
                ));

            // Settings Mouse
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 + (int)(93 * scaleX), viewportHeight / 2 - (int)(100 * scaleY), 187, 187),
                mouseButton,
                mouseButton
                ));

            // Back to menu
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(30 * scaleX), viewportHeight / 2 + (int)(100 * scaleY), 100, 100),
                backButton,
                backButton
                ));

            // Pause
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(89 * scaleX), (int)(100 * scaleY), 179, 54),
                resumeButton,
                resumeButton
                ));

            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(89 * scaleX), (int)(200 * scaleY), 179, 54),
                menuButton,
                menuButton
                ));

            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(93 * scaleX), (int)(300 * scaleY), 187, 65),
                controlsButton,
                controlsButton
                ));

            // Gameover
            buttons.Add(new Button(
                _graphics.GraphicsDevice,
                new Rectangle(viewportWidth / 2 - (int)(89 * scaleX), (int)(250 * scaleY), 179, 54),
                continueButton,
                continueButton
                ));

            // Assign methods to the buttons' event 
            buttons[0].OnLeftButtonClick += this.StartButton;
            buttons[1].OnLeftButtonClick += this.SettingsButton;
            buttons[2].OnLeftButtonClick += this.KeyboardControlButton;
            buttons[3].OnLeftButtonClick += this.MouseControlButton;
            buttons[4].OnLeftButtonClick += this.BackButton;
            buttons[5].OnLeftButtonClick += this.ResumeButton;
            buttons[6].OnLeftButtonClick += this.MenuButton;
            buttons[7].OnLeftButtonClick += this.SettingsButton;
            buttons[8].OnLeftButtonClick += this.ContinueButton;

            // Add Checkpoints
            deactivedCheckpoints.Add(new Checkpoint("checkpoint1", checkpoint, new Rectangle(viewportWidth, viewportHeight / 2 - (int)(50 * scaleY), 100, 100), 60));
            deactivedCheckpoints.Add(new Checkpoint("checkpoint2", checkpoint, new Rectangle(viewportWidth, viewportHeight / 2 - (int)(50 * scaleY), 100, 100), 120));
            deactivedCheckpoints.Add(new Checkpoint("checkpoint3", checkpoint, new Rectangle(viewportWidth, viewportHeight / 2 - (int)(50 * scaleY), 100, 100), 180));

            //Loads audiofile
            this.audio = Content.Load<Song>("UnderwaterSounds");
            MediaPlayer.Play(audio);
            MediaPlayer.IsRepeating = true;
            //Use this to adjust volume
            //1.0 is full and 0.0 is silent
            MediaPlayer.Volume = 1.0f;
        }
        #endregion

        #region Update
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //Update based on the GameState
            switch (currentState)
            {
                case GameState.Menu:
                    // Reset the game
                    GameReset();

                    previousState = currentState;
                    
                    //Update the size and rotation of the logo on the main menu
                    if (sizeChanges > 0)
                    {    
                        scale.X -= .0005f;
                        scale.Y -= .0005f;
                                            
                        sizeChanges--;
                    }
                    
                    if (sizeChanges == 200 || sizeChanges == -200 )
                    {
                        rotationAmount = rotationAmount * -1;
                        sizeChanges--;
                    }

                    else if (sizeChanges <= 0 && sizeChanges > -400)
                    {
                        scale.X += .0005f;
                        scale.Y += .0005f;
                        sizeChanges--;                        
                    }
                    if (sizeChanges == -400)
                    {
                        
                        sizeChanges = 400;                
                    }

                    rotation += rotationAmount;

                    //Moves the backdrop
                    MoveBackdrop(1, -1);

                    // Buttons Update
                    buttons[0].Update();
                    buttons[1].Update();

                    IsMouseVisible = true;
                    break;

                case GameState.Settings:
                    IsMouseVisible = true;

                    //Moves the backdrop
                    MoveBackdrop(1, -1);

                    // Buttons Update
                    buttons[2].Update();
                    buttons[3].Update();
                    buttons[4].Update();
                    break;

                case GameState.Game:
                    // Update actual game time
                    if (increment)
                    {
                        time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    IsMouseVisible = false;

                    // Press ESC to pause the game
                    if (SingleKeyPress(Keys.P, kbState))
                    {
                        currentState = GameState.Pause;
                    }

                    if (player.IsAlive)
                    {
                        //Updates all the projectiles and enemies
                        projectileManager.ManageProjectiles(this, gameTime);
                        enemyManager.ManageEnemies(this, gameTime);

                        for(int i = 0; i < pickups.Count; i++)
                        {
                            if (pickups[i].Update(player))
                            {
                                pickups.Remove(pickups[i]);
                                i--;
                            }
                        }

                        //Processes player input through the player object
                        if (keyboardControls)
                        {
                            player.ProcessInputKeyboard(Keyboard.GetState(), this);
                        }
                        else
                        {
                            player.ProcessInputMouse(Mouse.GetState(), this);
                        }

                        //Updates the player object
                        player.Update(gameTime, this);

                        //MovesBackdrop
                        MoveBackdrop(2, -1);

                        if(pastCheckpoints == 3)
                        {
                            spawnEnemy = true;
                        }

                        if(upgrade == null)
                        {
                            // Randomly creates enemies based on a timer at random positions
                            //
                            // Will need to be changed, only here for testing purposes
                            if (spawnEnemy)// If a checkpoint appears, then stop ememies from spawning 
                            {
                                //FileIO

                                //Spawns boss after reaching end
                                if (deactivedCheckpoints[2].Contact == true)
                                {
                                    if (!addedBoss)
                                    {
                                        enemies.Add(new Boss(bossEnemySprite, new Rectangle(oldWidth - 128, oldHeight / 2 - 64, 256, 128), 0, baseEnemyProjectileSprite, -10, -20, 6, 50, rng, 3, 1, laserSprite, new Rectangle(oldWidth - 128, oldHeight / 2 - 64, 256, 128)));
                                        addedBoss = true;
                                    }
                                }
                                //Formation spawns for 3rd section
                                else if (deactivedCheckpoints[1].Contact == true)
                                {
                                    if ((timer - 1 + 1 * pastCheckpoints) % 3601 == 15)
                                    {
                                        enemies.AddRange(fileIO.AddFormation(3, rng.Next(0, oldHeight - 343)));
                                    }
                                    if ((timer - 1 + 1 * pastCheckpoints) % 3601 == 500)
                                    {
                                        enemies.AddRange(fileIO.AddFormation(7, 0));
                                    }
                                    if ((timer - 1 + 1 * pastCheckpoints) % 3601 == 2550)
                                    {
                                        enemies.AddRange(fileIO.AddFormation(4, rng.Next(0, oldHeight - 343)));
                                    }
                                    if ((timer - 1 + 1 * pastCheckpoints) % 3601 == 2950)
                                    {
                                        enemies.AddRange(fileIO.AddFormation(8, 0));
                                    }
                                    if ((timer - 1 + 1 * pastCheckpoints) % 3601 == 3550)
                                    {
                                        enemies.AddRange(fileIO.AddFormation(9, 0));
                                    }
                                    /*
                                    if (!addedFormation2)
                                    {
                                        enemiesToAdd[0] = fileIO.AddFormation(1, viewportHeight - 343);
                                        addedFormation2 = true;
                                    }
                                    if (enemiesToAdd[0] != null)
                                    {
                                        enemies.AddRange(enemiesToAdd[0]);
                                        enemiesToAdd[0] = null;
                                    }
                                    */
                                }
                                //Formation spawns for 2nd section
                                else if (deactivedCheckpoints[0].Contact == true)
                                {
                                    if (timer % 240 == rng.Next(0, 240))
                                    {
                                        enemies.AddRange(fileIO.AddFormation(5, rng.Next(0, oldHeight - 63)));
                                    }
                                    if (timer % 360 == rng.Next(0, 360))
                                    {
                                        enemies.AddRange(fileIO.AddFormation(2, rng.Next(0, oldHeight - 63)));
                                    }
                                    if (timer % 480 == rng.Next(0, 480))
                                    {
                                        enemies.AddRange(fileIO.AddFormation(6, rng.Next(0, oldHeight - 203)));
                                    }
                                    if (timer % 240 == rng.Next(0, 240))
                                    {
                                        enemies.Add(new BaseEnemy(baseEnemySprite, new Rectangle(oldWidth, rng.Next(0, oldHeight - 63), 64, 64), 2, 120, baseEnemyProjectileSprite, new Rectangle(0, 0, 64, 52)));
                                    }

                                }
                                //Spawns for 1st section
                                else
                                {
                                    if (timer % 240 == rng.Next(0, 240))
                                    {
                                        enemies.Add(new BaseEnemy(baseEnemySprite, new Rectangle(oldWidth, rng.Next(0, oldHeight - 63), 64, 64), 2, 120, baseEnemyProjectileSprite, new Rectangle(0, 0, 64, 52)));
                                    }
                                    if (timer % 240 == rng.Next(0, 240))
                                    {
                                        enemies.Add(new AimingEnemy(aimedEnemySprite, new Rectangle(oldWidth, rng.Next(0, oldHeight - 63), 64, 64), 2, 120, baseEnemyProjectileSprite, new Rectangle(0, 0, 62, 40)));
                                    }
                                    if (timer % 240 == rng.Next(0, 240))
                                    {
                                        enemies.Add(new StaticEnemy(staticEnemySprite, new Rectangle(oldWidth, rng.Next(0, oldHeight - 63), 64, 64), 4, new Rectangle(0, 0, 62, 56)));
                                    }
                                    if (timer % 240 == rng.Next(0, 240))
                                    {
                                        enemies.Add(new RangedHomingEnemy(homingEnemySprite, new Rectangle(oldWidth, rng.Next(0, oldHeight - 63), 64, 64), 2, 240, baseEnemyProjectileSprite, new Rectangle(0, 0, 62, 36)));
                                    }
                                }
                            }
                        }
                        

                        if(enemies.Count == 0 && addedBoss && player.Health > 0)
                        {
                            currentState = GameState.Menu;
                        }

                        //Timers for time/update based actions
                        if (increment)
                        {
                            timer += 1;
                        }

                        if (player.Health <= 0)
                        {
                            player.IsAlive = false;
                            //Change state if the player dies 
                            currentState = GameState.GameOver;
                        }

                        // Find the next uncontacted checkpoint
                        CheckpointManager();

                        // Update the activated checkpoint
                        if(activeCheckpoint != null)
                        {
                            if (activeCheckpoint.Update(this))
                            {
                                upgrade = new Upgrades(1, 1, 1, posHealth, posDamage, posSpeed, healthUpgrade, damageUpgrade, speedUpgrade);
                            }
                        }

                        //Checks if upgrades arent null and update sthem

                        if (upgrade != null)
                        {
                            upgrade.Update(this);
                        }
                    }
                    break;

                case GameState.Pause:
                    IsMouseVisible = true;
                    previousState = currentState;

                    //Moves the backdrop
                    MoveBackdrop(1, -1);

                    // Press ESC to resume 
                    if (SingleKeyPress(Keys.Escape, kbState))
                    {
                        currentState = GameState.Game;
                    }

                    // Buttons Update
                    buttons[5].Update();
                    buttons[6].Update();
                    buttons[7].Update();
                    break;

                case GameState.GameOver:
                    IsMouseVisible = true;

                    //Moves the backdrop
                    MoveBackdrop(1, -1);

                    // Buttons Update
                    buttons[8].Update();
                    break;
            }

            //Get the previous keyboard state
            previousKbState = kbState;

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draws the all objects to the window
            _spriteBatch.Begin();

            switch (currentState)
            {
                case GameState.Menu:
                    // Draw title page
                    _spriteBatch.Draw(titlePage, new Rectangle(0, 0, _graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height), Color.White);
                    _spriteBatch.Draw(titleBubbles, titleBubblesPos, Color.White);
                    _spriteBatch.Draw(titleBubblesLoop, titleBubblesSwapPos, Color.White);
                    _spriteBatch.Draw(logo, logoVect, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
                    _spriteBatch.Draw(teamName, new Rectangle(0,0, _graphics.GraphicsDevice.Viewport.Width, _graphics.GraphicsDevice.Viewport.Height), Color.White);

                    // Draw buttons
                    buttons[0].Draw(_spriteBatch); // Start
                    buttons[1].Draw(_spriteBatch); // Settings 
                    break;

                case GameState.Settings:
                    _spriteBatch.Draw(titlePage, new Rectangle(0, 0, viewportWidth, viewportHeight), Color.White);
                    _spriteBatch.Draw(titleBubbles, titleBubblesPos, Color.White);
                    _spriteBatch.Draw(titleBubblesLoop, titleBubblesSwapPos, Color.White);
                    _spriteBatch.DrawString(font, "Select Your Control Scheme", new Vector2(70 * scaleX, 30 * scaleY), Color.Black, 0, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);
                    buttons[2].Draw(_spriteBatch);
                    buttons[3].Draw(_spriteBatch);
                    buttons[4].Draw(_spriteBatch);
                    break;

                case GameState.Game:
                    _spriteBatch.Draw(backdrop, backdropPos, Color.White);
                    _spriteBatch.Draw(backdropSwap, backdropSwapPos, Color.White);

                    // Draw a timer 
                    //_spriteBatch.DrawString(font, string.Format("{0:f0}", time), new Vector2(10, 10), Color.White);
                    //_spriteBatch.DrawString(font, string.Format(currentCheckpoint.GetName), new Vector2(50, 10), Color.White);
                    

                    // Draw the HUD
                    hud.Draw(_spriteBatch, player, oldHeight, pastCheckpoints, (((timer - 1) % 3601.0) / 3601.0), upgrade);

                    // Draw projectiles
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        if (projectiles[i] is PlayerProjectile)
                        {
                            _spriteBatch.Draw(projectiles[i].Sprite, projectiles[i].Position, Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(projectiles[i].Sprite, projectiles[i].Position, Color.Red);
                        }
                    }

                    // Draw enemies
                    enemyManager.Draw(_spriteBatch);

                    foreach(HealthPickup pickup in pickups)
                    {
                        pickup.Draw(_spriteBatch);
                    }

                    // Draw player
                    if (player != null)
                    {
                        if(player.IFrameTimer % 10 > 4 || player.IFrameTimer == 0)
                        {
                            _spriteBatch.Draw(player.Sprite, player.Position, Color.White);
                        }
                    }

                    // Find the next uncontacted checkpoint
                    CheckpointManager();

                    // Draw checkpoints
                    if(activeCheckpoint != null)
                    {
                        activeCheckpoint.Draw(_spriteBatch, this);
                    }

                    // Print "game saved" message
                    foreach(Checkpoint c in deactivedCheckpoints)
                    {
                        c.PrintMessage(_spriteBatch,this);
                    }

                    //Draws the upgrades

                    if (upgrade != null)
                    {
                        upgrade.Draw(_spriteBatch, upgradeFont, this);
                    }
                    break;

                case GameState.Pause:
                    _spriteBatch.Draw(titlePage, new Rectangle(0, 0, viewportWidth, viewportHeight), Color.White);
                    _spriteBatch.Draw(titleBubbles, titleBubblesPos, Color.White);
                    _spriteBatch.Draw(titleBubblesLoop, titleBubblesSwapPos, Color.White);
                    _spriteBatch.Draw(pauseText, new Vector2(5, 5), Color.White);
                    buttons[5].Draw(_spriteBatch);
                    buttons[6].Draw(_spriteBatch);
                    buttons[7].Draw(_spriteBatch);
                    break;

                case GameState.GameOver:
                    _spriteBatch.Draw(titlePage, new Rectangle(0, 0, viewportWidth, viewportHeight), Color.White);
                    _spriteBatch.Draw(titleBubbles, titleBubblesPos, Color.White);
                    _spriteBatch.Draw(titleBubblesLoop, titleBubblesSwapPos, Color.White);
                    _spriteBatch.DrawString(font, "GameOver", new Vector2(270 * scaleX, 150 * scaleY), Color.Black, 0, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0);
                    buttons[8].Draw(_spriteBatch);
                    break;
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion

        //Finds the current mouse position and stores the values in variables mouseX and mouseY
        protected void UpdateMouse()
        {
            MouseState currentMouse = Mouse.GetState();
            mouseX = currentMouse.X;
            mouseY = currentMouse.Y;
        }

        // Check for a single key press
        private bool SingleKeyPress(Keys key, KeyboardState kbState)
        {
            return previousKbState.IsKeyDown(key) && kbState.IsKeyUp(key);
        }

        #region Button Methods
        // Buttons' methods for state changing 
        public void StartButton()
        {
            currentState = GameState.Game;
        }

        public void SettingsButton()
        {
            currentState = GameState.Settings;
        }

        public void MouseControlButton()
        {
            keyboardControls = false;
            buttons[3].SetButtonColor = Color.DeepSkyBlue;
            buttons[2].SetButtonColor = Color.White;
        }

        public void KeyboardControlButton()
        {
            keyboardControls = true;
            buttons[3].SetButtonColor = Color.White;
            buttons[2].SetButtonColor = Color.DeepSkyBlue;
        }

        public void BackButton()
        {
            currentState = previousState;
        }

        public void ResumeButton()
        {
            currentState = GameState.Game;
        }

        public void MenuButton()
        {
            currentState = GameState.Menu;
        }

        public void ContinueButton()
        {
            currentState = GameState.Menu;
        }
        #endregion

        #region Backdrop Method
        //Moves the backdrop
        public void MoveBackdrop(int speed, int multiplier)
        {
            //Sorry Zander, I stole your method and expanded it for the main menu :) - Quinn

            //Moves the backdrop, and if the backdrop goes past a certain x or y value, gets placed at the right edge of
            //the screen.
            //There are two backdrops because if you were to suddenly reposition the first once it reaches the end
            //it is really jarring, also I mirrored the image for the second backdrop so it looks a bit better

            switch (currentState)
            {
                case (GameState.Game):

                    backdropPos.X -= speed;
                    backdropSwapPos.X -= speed;

                    if (backdropPos.X == multiplier * viewportWidth)
                    {
                        backdropPos.X = viewportWidth;
                    }
                    if (backdropSwapPos.X == multiplier * viewportWidth)
                    {
                        backdropSwapPos.X = viewportWidth;
                    }

                    break;

                case (GameState.Menu):
                case (GameState.Settings):
                case (GameState.Pause):
                case (GameState.GameOver):

                    titleBubblesPos.Y -= speed;
                    titleBubblesSwapPos.Y -= speed;

                    if (titleBubblesPos.Y == multiplier * viewportHeight)
                    {
                        titleBubblesPos.Y = viewportHeight;
                    }
                    if (titleBubblesSwapPos.Y == multiplier * viewportHeight)
                    {
                        titleBubblesSwapPos.Y = viewportHeight;
                    }

                    break;
                          
            } 
        }
        #endregion

        /// <summary>
        /// Reset the game in menu state
        /// </summary>
        private void GameReset()
        {
            // Reset the game
            enemies.Clear();
            projectiles.Clear();
            player.Position = GlobalScalars.scaleRect(new Rectangle(0, viewportHeight / 2, 48, 48));
            player.MaxHP = 7;
            player.Health = player.MaxHP;
            player.ProjectileDamage = 1;
            player.Speed = ((GlobalScalars.x + GlobalScalars.y) / 2);
            player.IsAlive = true;
            time = 0;
            timer = 0;
            addedBoss = false;
            currentCheckpoint = deactivedCheckpoints[0];
            pastCheckpoints = 0;
            pickups.Clear();

            // reset checkpoints
            foreach (Checkpoint c in deactivedCheckpoints)
            {
                c.Contact = false;
                c.Position = GlobalScalars.scaleRect(new Rectangle(oldWidth, oldHeight / 2 - 50, 100, 100));
            }
        }

        // Find the first uncontacted checkpoint 
        private void CheckpointManager()
        {
            for (int i = 0; i < deactivedCheckpoints.Count; i++)
            {
                if (!deactivedCheckpoints[i].Contact)
                {
                    activeCheckpoint = deactivedCheckpoints[i];
                    break;
                }
                else
                {
                    pastCheckpoints = i + 1;
                }
            }
            if(pastCheckpoints == deactivedCheckpoints.Count)
            {
                activeCheckpoint = null;
            }
        }
    }
}
