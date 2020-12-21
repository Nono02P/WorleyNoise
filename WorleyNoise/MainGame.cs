using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace WorleyNoise
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        private const int _NB_POINTS = 100;
        private const int _DEPTH = 100;

        private const double _TIMER = 0.4;
        private double _currentTime = _TIMER;

        private int _currentDepth = 0;
        private WorleyNoise _worleyNoise;
        private Vector2[] _points;
        private Random _rnd;
        private Texture2D _texture;
        private Color[] _textureColors;
        private KeyboardState _previousKbState;
        private Viewport _vp;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _rnd = new Random();

            graphics.PreferredBackBufferWidth = 400;
            graphics.PreferredBackBufferHeight = 400;
            graphics.ApplyChanges();

            GraphicsDevice gd = graphics.GraphicsDevice;
            _vp = gd.Viewport;
            int w = 100;
            int h = 100;
            _texture = new Texture2D(gd, w, h);
            _textureColors = new Color[w * h];
            _points = new Vector2[_NB_POINTS];
            
            for (int i = 0; i < _NB_POINTS; i++)
            {
                _points[i] = new Vector2(_rnd.Next(w), _rnd.Next(h));
            }

            GenerateMap();
            RefreshScreen();

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
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || kb.IsKeyDown(Keys.Escape))
                Exit();

            if (kb.IsKeyDown(Keys.G) && !_previousKbState.IsKeyDown(Keys.G))
                GenerateMap();

            _currentTime -= gameTime.ElapsedGameTime.TotalSeconds;

            if (_currentTime <= 0)
            {
                _currentDepth = (_currentDepth + 1) % _DEPTH;
                RefreshScreen();
                _currentTime = _TIMER;
            }

            _previousKbState = kb;

            base.Update(gameTime);
        }

        private void GenerateMap()
        {
            int w = _texture.Width;
            int h = _texture.Height;
            _worleyNoise = new WorleyNoise(_rnd);
            _worleyNoise.GenerateMap(_NB_POINTS, w, h, _DEPTH);
        }

        private void RefreshScreen()
        {
            int w = _texture.Width;

            for (int i = 0; i < _textureColors.Length; i++)
            {   
                int x = i % w;
                int y = i / w;

                float r = _worleyNoise.GetValue(x, y, _currentDepth, 0);
                float g = _worleyNoise.GetValue(x, y, _currentDepth, 1);
                float b = _worleyNoise.GetValue(x, y, _currentDepth, 2);
                
                _textureColors[i] = new Color(r, g, b);
            }
            _texture.SetData(_textureColors);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(_texture, graphics.GraphicsDevice.Viewport.Bounds, Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
