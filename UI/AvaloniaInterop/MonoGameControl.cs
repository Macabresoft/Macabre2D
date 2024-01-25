namespace Macabresoft.Macabre2D.UI.AvaloniaInterop;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

/// <summary>
/// A control that renders a MonoGame instance inside of Avalonia.
/// </summary>
public sealed class MonoGameControl : Control, IObserver<AvaloniaPropertyChangedEventArgs<IAvaloniaGame>> {
    /// <summary>
    /// Avalonia property for the fallback background property.
    /// </summary>
    public static readonly StyledProperty<IBrush> FallbackBackgroundProperty = AvaloniaProperty.Register<MonoGameControl, IBrush>(
        nameof(FallbackBackground),
        Color.HotPink.ToAvaloniaBrush());

    /// <summary>
    /// Avalonia property for <see cref="Game" />.
    /// </summary>
    public static readonly StyledProperty<IAvaloniaGame> GameProperty = AvaloniaProperty.Register<MonoGameControl, IAvaloniaGame>(nameof(Game));

    /// <summary>
    /// Avalonia property for a property indicating whether or not this should render.
    /// </summary>
    public static readonly StyledProperty<bool> ShouldRenderProperty = AvaloniaProperty.Register<MonoGameControl, bool>(
        nameof(ShouldRender),
        true);

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
    private WriteableBitmap _bitmap = new(new PixelSize(1, 1), Vector.One, PixelFormat.Rgba8888, AlphaFormat.Opaque);
    private byte[] _bufferData = Array.Empty<byte>();
    private bool _isInitialized;
    private MonoGameKeyboard _keyboard;
    private MonoGameMouse _mouse;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonoGameControl" /> class.
    /// </summary>
    public MonoGameControl() {
        GameProperty.Changed.Subscribe(this);
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

    /// <summary>
    /// Gets or sets a value indicating whether or not this can render.
    /// </summary>
    public bool ShouldRender {
        get => this.GetValue(ShouldRenderProperty);
        set => this.SetValue(ShouldRenderProperty, value);
    }

    /// <inheritdoc />
    public void OnCompleted() {
    }

    /// <inheritdoc />
    public void OnError(Exception error) {
    }

    /// <inheritdoc />
    public void OnNext(AvaloniaPropertyChangedEventArgs<IAvaloniaGame> value) {
        if (this._isInitialized) {
            this.Initialize();
        }
    }

    /// <inheritdoc />
    public override void Render(DrawingContext context) {
        if (this.CanDraw(out var game, out var device)) {
            this._gameTime.ElapsedGameTime = this._stopwatch.Elapsed;
            this._gameTime.TotalGameTime += this._gameTime.ElapsedGameTime;
            this._stopwatch.Restart();
            this.RunFrame();

            try {
                using var bitmapLock = this._bitmap.Lock();
                var size = bitmapLock.RowBytes * bitmapLock.Size.Height;
                if (this._bufferData.Length != size) {
                    this._bufferData = new byte[size];
                }

                device.GetBackBufferData(this._bufferData);
                Marshal.Copy(this._bufferData, 0, bitmapLock.Address, this._bufferData.Length);
            }
            catch {
                // If we've got an exception in this block, there's a good chance resetting the graphics
                // device will fix the issue. It usually means the size of the device doesn't match the
                // bitmap size.
                this.ResetDevice(device, this.Bounds.Size);
            }

            if (!this.TryDrawBitmap(context) && game.CurrentScene is { } scene) {
                context.DrawRectangle(scene.BackgroundColor.ToAvaloniaBrush(), null, new Rect(this.Bounds.Size));
            }
        }
        else if (!this.TryDrawBitmap(context)) {
            context.DrawRectangle(this.FallbackBackground, null, this.Bounds);
        }
    }

    /// <inheritdoc />
    protected override Size ArrangeOverride(Size finalSize) {
        finalSize = base.ArrangeOverride(finalSize);
        if (finalSize != this._bitmap.Size && this.Game?.GraphicsDevice is { } device) {
            this.ResetDevice(device, finalSize);
        }

        return finalSize;
    }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);
        this.Start();
    }

    /// <inheritdoc />
    protected override void OnUnloaded(RoutedEventArgs e) {
        base.OnUnloaded(e);

        if (this.Game is { } game) {
            game.Exit();
            game.Tick(); // Required to finalize Exit() call.
            game.Dispose();
        }
    }

    private bool CanDraw(out IAvaloniaGame game, out GraphicsDevice device) {
        game = this.Game;
        device = this.Game?.GraphicsDevice;

        return this.ShouldRender && game != null && device != null &&
               this.Bounds is { Width: > 0, Height: > 0 } &&
               this.HandleDeviceReset(device);
    }

    private bool HandleDeviceReset(GraphicsDevice device) {
        if (device.GraphicsDeviceStatus == GraphicsDeviceStatus.NotReset) {
            this.ResetDevice(device, this.Bounds.Size);
        }

        return device.GraphicsDeviceStatus == GraphicsDeviceStatus.Normal;
    }

    private void Initialize() {
        if (this.Game is { } game && this.GetVisualRoot() is Window window) {
            game.Initialize(this._mouse, this._keyboard);

            if (window.TryGetPlatformHandle() is { } platformHandle) {
                this._presentationParameters.DeviceWindowHandle = platformHandle.Handle;
            }

            this.ResetDevice(game.GraphicsDevice, this.Bounds.Size);
            this.RunFrame();
        }
    }

    private void ResetDevice(GraphicsDevice device, Size newSize) {
        if (device == null) {
            return;
        }

        var newWidth = Math.Max(1, (int)Math.Ceiling(newSize.Width));
        var newHeight = Math.Max(1, (int)Math.Ceiling(newSize.Height));

        device.Viewport = new Viewport(0, 0, newWidth, newHeight);
        this._presentationParameters.BackBufferWidth = newWidth;
        this._presentationParameters.BackBufferHeight = newHeight;
        device.Reset(this._presentationParameters);

        this._bitmap?.Dispose();
        this._bitmap = new WriteableBitmap(
            new PixelSize(device.Viewport.Width, device.Viewport.Height),
            new Vector(96d, 96d),
            PixelFormat.Rgba8888,
            AlphaFormat.Opaque);
    }

    private void RunFrame() {
        try {
            this.Game.RunOneFrame();
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
        finally {
            Dispatcher.UIThread.Post(this.InvalidateVisual, DispatcherPriority.Render);
        }
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

    private bool TryDrawBitmap(DrawingContext context) {
        if (this._bitmap is { Size: { Width: > 1, Height: > 1 } }) {
            context.DrawImage(this._bitmap, new Rect(this._bitmap.Size), this.Bounds);
            return true;
        }

        return false;
    }
}