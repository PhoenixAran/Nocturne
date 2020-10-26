using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime;

namespace Nocturne
{
    public class Engine : Game
    {
        public string Title;

        //static references
        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Commands Commands { get; private set; }
        public static ContentManager ContentManager { get; private set; }
        public static Pooler Pooler { get; private set; }
        public static SpriteBank SpriteBank { get; private set; }


        //screen size
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int ViewWidth { get; private set; }
        public static int ViewHeight { get; private set; }
        public static int ViewPadding
        {
            get => viewPadding;
            set
            {
                viewPadding = value;
                Instance.UpdateView();
            }
        }

        private static int viewPadding = 0;
        private static bool resizing;

        // util
        public static Color ClearColor;
        public static bool ExitOnEscapeKeypress;

        //scene
        public static Scene Scene
        {
            get => Instance.scene;
            set
            {
                if ( Instance.scene == null )
                {
                    Instance.scene = value;
                    Instance.scene.Initialize();
                    Instance.scene.Begin();
                }
            }
        }

        Scene scene;
        Scene nextScene;

        //screen
        public static Viewport Viewport { get; private set; }
        public static Matrix ScreenMatrix;


        public Engine(int width, int height, int windowWidth, int windowHeight, string windowTitle, bool fullscreen)
        {
            Instance = this;

            Title = Window.Title = windowTitle;
            Width = width;
            Height = height;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager( this );
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.DeviceCreated += OnGraphicsCreate;
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.PreferMultiSampling = false;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
           
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;
            if ( fullscreen )
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = windowWidth;
                Graphics.PreferredBackBufferHeight = windowHeight;
                Graphics.IsFullScreen = false;
            }
            Graphics.ApplyChanges();


            base.Content.RootDirectory = @"Content";
            IsMouseVisible = false;
            IsFixedTimeStep = false;
            ExitOnEscapeKeypress = true;

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Physics.Reset();
        }

        #region Game Overrides
        protected override void Initialize()
        {
            base.Initialize();
            Pooler = new Pooler();
            Commands = new Commands();
            SpriteBank = new SpriteBank();
            SpriteBank.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            Engine.ContentManager = base.Content;
            Nocturne.Draw.Initialize( GraphicsDevice );
        }

        protected override void Update( GameTime gameTime )
        {
            Time.Update( (float)gameTime.ElapsedGameTime.TotalSeconds );
            Input.Update();

            if ( GamePad.GetState( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown( Keys.Escape ) )
                Exit();


            if ( Scene != null )
            {
                Scene.BeforeUpdate();
                Scene.Update();
                Scene.AfterUpdate();
            }

            //Debug Console
            if ( Commands.Open )
                Commands.UpdateOpen();
            else if ( Commands.Enabled )
                Commands.UpdateClosed();

            base.Update( gameTime );
        }

        protected override void Draw( GameTime gameTime )
        {

            if ( scene != null )
            {
                Scene.BeforeRender();
            }

            GraphicsDevice.SetRenderTarget( null );
            GraphicsDevice.Viewport = Viewport;
            GraphicsDevice.Clear( ClearColor );
            if ( scene != null )
            {
                scene.Render();
                scene.AfterRender();
            }
           

            //base.Draw( gameTime );
            if ( Commands.Open )
                Commands.Render();
        }
        #endregion

        #region Graphics Device Callbacks
        protected virtual void OnClientSizeChanged( object sender, EventArgs e )
        {
            if ( Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing )
            {
                resizing = true;

                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                UpdateView();

                resizing = false;
            }
        }

        protected virtual void OnGraphicsReset( object sender, EventArgs e )
        {
            UpdateView();

            if ( scene != null )
                scene.HandleGraphicsReset();
            if ( nextScene != null && nextScene != scene )
                nextScene.HandleGraphicsReset();
        }

        protected virtual void OnGraphicsCreate( object sender, EventArgs e )
        {
            UpdateView();
            if ( scene != null )
                scene.HandleGraphicsCreate();
            if ( nextScene != null && nextScene != scene )
                nextScene.HandleGraphicsCreate();
        }

        protected override void OnActivated(object sender, EventArgs args )
        {
            base.OnActivated( sender, args );

            if ( scene != null )
            {
                scene.GainFocus();
            }
        }
        #endregion

        #region screen stuff
        public static void SetWindowed( int width, int height )
        {
            if ( width > 0 && height > 0 )
            {
                resizing = true;
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.ApplyChanges();
                resizing = false;
            }
        }

        public static void SetFullscreen()
        {
            resizing = true;
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            resizing = false;
        }

        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // get View Size
            if ( screenWidth / Width > screenHeight / Height )
            {
                ViewWidth = (int)( screenHeight / Height * Width );
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)screenWidth;
                ViewHeight = (int)( screenWidth / Width * Height );
            }

            // apply View Padding
            var aspect = ViewHeight / (float)ViewWidth;
            ViewWidth -= ViewPadding * 2;
            ViewHeight -= (int)( aspect * ViewPadding * 2 );

            // update screen matrix
            ScreenMatrix = Matrix.CreateScale( ViewWidth / (float)Width );

            // update viewport
            Viewport = new Viewport
            {
                X = (int)( screenWidth / 2 - ViewWidth / 2 ),
                Y = (int)( screenHeight / 2 - ViewHeight / 2 ),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0,
                MaxDepth = 1
            };
        }

        #endregion
    }
}

