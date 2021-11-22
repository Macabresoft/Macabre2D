namespace Macabresoft.Macabre2D.UI.AvaloniaInterop {
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Avalonia.Platform;
    using Avalonia.Threading;
    using Avalonia.VisualTree;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop.Extensions;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A control that renders a MonoGame instance inside of Avalonia.
    /// </summary>
    public sealed class MonoGameControl : Control {
        /// <summary>
        /// Avalonia property for <see cref="Game" />.
        /// </summary>
        public static readonly StyledProperty<IBrush> FallbackBackgroundProperty = AvaloniaProperty.Register<MonoGameControl, IBrush>(
            nameof(FallbackBackground),
            DefinedColors.MacabresoftPurple.ToAvaloniaBrush());

        /// <summary>
        /// Avalonia property for <see cref="Game" />.
        /// </summary>
        public static readonly StyledProperty<IAvaloniaGame> GameProperty = AvaloniaProperty.Register<MonoGameControl, IAvaloniaGame>(
            nameof(Game),
            notifying: OnGameChanging);

        private readonly GameTime _gameTime = new();

        private readonly PresentationParameters _presentationParameters = new() {
            BackBufferWidth = 1,
            BackBufferHeight = 1,
            BackBufferFormat = SurfaceFormat.Color,
            DepthStencilFormat = DepthFormat.Depth24,
            PresentationInterval = PresentInterval.Immediate,
            IsFullScreen = false
        };

        private readonly Stopwatch _stopwatch = new();
        private WriteableBitmap _bitmap;
        private bool _isInitialized;
        private bool _isResizeProcessing;
        private MonoGameKeyboard _keyboard;
        private MonoGameMouse _mouse;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoGameControl" /> class.
        /// </summary>
        public MonoGameControl() {
            this.Focusable = true;
        }

        /// <summary>
        /// Gets or sets the fallback background brush.
        /// </summary>
        public IBrush FallbackBackground {
            get => this.GetValue(FallbackBackgroundProperty);
            set => this.SetValue(FallbackBackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        public IAvaloniaGame Game {
            get => this.GetValue(GameProperty);
            set => this.SetValue(GameProperty, value);
        }

        /// <inheritdoc />
        public override void Render(DrawingContext context) {
            if (this.IsEffectivelyVisible && this.Game is IAvaloniaGame { GraphicsDevice: GraphicsDevice device } game) {
                base.Render(context);

                this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
                this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
                this._stopwatch.Restart();

                if (this.CanBeginDraw()) {
                    using (var bitmapLock = this._bitmap.Lock()) {
                        this.RunFrame();

                        var data = new byte[bitmapLock.RowBytes * bitmapLock.Size.Height];
                        device.GetBackBufferData(data);
                        Marshal.Copy(data, 0, bitmapLock.Address, data.Length);
                    }

                    context.DrawImage(this._bitmap, new Rect(this._bitmap.Size), new Rect(this.Bounds.Size));
                }
                else if (game.Scene is IScene scene) {
                    context.DrawRectangle(scene.BackgroundColor.ToAvaloniaBrush(), null, new Rect(this.Bounds.Size));
                }
            }
            else {
                context.DrawRectangle(this.FallbackBackground, null, this.Bounds);
            }

            Dispatcher.UIThread.Post(this.InvalidateVisual, DispatcherPriority.Render);
        }

        /// <inheritdoc />
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnAttachedToVisualTree(e);
            this.Start();
        }
        
        private static void OnGameChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is MonoGameControl { _isInitialized: true } monoGameControl) {
                monoGameControl.Initialize();
            }
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(this.Bounds)) {
                this.ResetDevice();
            }
        }


        private bool CanBeginDraw() {
            // If we have no graphics device, we must be running in the designer. Make sure the
            // graphics device is big enough, and is not lost.
            return !this._isResizeProcessing && this.Game?.GraphicsDevice != null && this.Bounds.Width > 0 && this.Bounds.Height > 0 && this.HandleDeviceReset();
        }

        private bool HandleDeviceReset() {
            if (this.Game?.GraphicsDevice is not GraphicsDevice device || device.GraphicsDeviceStatus == GraphicsDeviceStatus.Lost) {
                return false;
            }

            if (device.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset ||
                device.PresentationParameters.BackBufferWidth != this._bitmap.PixelSize.Width ||
                device.PresentationParameters.BackBufferHeight != this._bitmap.PixelSize.Height) {
                this.ResetDevice();
                return false;
            }

            return true;
        }

        private void Initialize() {
            if (this.Game != null && this.GetVisualRoot() is Window window) {
                this.Game.Initialize(this._mouse, this._keyboard);
                this._presentationParameters.DeviceWindowHandle = window.PlatformImpl.Handle.Handle;
                this.ResetDevice();
                this.Game.RunOneFrame();
            }
        }

        private void ResetDevice() {
            var newWidth = Math.Max(1, (int)Math.Ceiling(this.Bounds.Width));
            var newHeight = Math.Max(1, (int)Math.Ceiling(this.Bounds.Height));

            if (this.Game?.GraphicsDevice is GraphicsDevice device) {
                this.Game.GraphicsDeviceManager.PreferredBackBufferWidth = newWidth;
                this.Game.GraphicsDeviceManager.PreferredBackBufferHeight = newHeight;
                device.Viewport = new Viewport(0, 0, newWidth, newHeight);
                this._presentationParameters.BackBufferWidth = newWidth;
                this._presentationParameters.BackBufferHeight = newHeight;
                device.Reset(this._presentationParameters);
                
                if (this.ShouldPerformResize()) {
                    try {
                        this._isResizeProcessing = true;
                        this._bitmap?.Dispose();

                        this._bitmap = new WriteableBitmap(
                            new PixelSize(device.PresentationParameters.BackBufferWidth, device.PresentationParameters.BackBufferHeight),
                            new Vector(96d, 96d),
                            PixelFormat.Rgba8888,
                            AlphaFormat.Opaque);

                        this.SetViewport();
                    }
                    finally {
                        this._isResizeProcessing = false;
                    }
                }
            }
        }

        private void RunFrame() {
            try {
                this.Game.RunOneFrame();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        private void SetViewport() {
            if (this.Game.GraphicsDevice is GraphicsDevice device) {
                var width = Math.Max(1, device.PresentationParameters.BackBufferWidth);
                var height = Math.Max(1, device.PresentationParameters.BackBufferHeight);
                device.Viewport = new Viewport(0, 0, width, height);
            }
        }

        private bool ShouldPerformResize() {
            return !this._isResizeProcessing &&
                   this.Bounds.Width > 0 &&
                   this.Bounds.Height > 0 &&
                   this.Game.GraphicsDevice is GraphicsDevice device &&
                   (this._bitmap == null ||
                    Math.Abs(this._bitmap.PixelSize.Width - Math.Ceiling(this.Bounds.Width)) > 0.01f ||
                    Math.Abs(this._bitmap.PixelSize.Height - Math.Ceiling(this.Bounds.Height)) > 0.01f ||
                    Math.Abs(device.Viewport.Width - Math.Ceiling(this.Bounds.Width)) > 0.01f ||
                    Math.Abs(device.Viewport.Height - Math.Ceiling(this.Bounds.Height)) > 0.01f);
        }

        private void Start() {
            if (this._isInitialized) {
                return;
            }

            this._mouse = new MonoGameMouse(this);
            this._keyboard = new MonoGameKeyboard(this);
            this.Initialize();
            this._stopwatch.Start();
            this._isInitialized = true;
        }
    }
}