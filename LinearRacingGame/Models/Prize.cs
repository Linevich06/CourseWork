using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using D3D11 = SharpDX.Direct3D11; // Псевдоним для устранения неоднозначности

namespace LinearRacingGame
{
    public abstract class Prize : IDisposable
    {
        public Vector2 Position { get; protected set; }
        public float Width { get; } = 30f;
        public float Height { get; } = 30f;
        protected D3D11.Texture2D _texture;
        protected D3D11.ShaderResourceView _textureView;
        protected D3D11.Device _device; // Явное указание типа

        public Prize(D3D11.Device device, Vector2 position)
        {
            _device = device;
            Position = position;
            InitializeTexture();
        }

        protected abstract void InitializeTexture();

        public abstract void ApplyEffect(Player player);

        public void Render(D3D11.DeviceContext context)
        {
            var viewports = context.Rasterizer.GetViewports<RawViewport>();
            var viewport = viewports[0];

            var screenPos = new Vector2(
                Position.X * viewport.Width,
                Position.Y * viewport.Height);

            // Код отрисовки текстуры
        }

        public void Dispose()
        {
            _textureView?.Dispose();
            _texture?.Dispose();
        }
    }

    public class FuelPrize : Prize
    {
        public FuelPrize(D3D11.Device device, Vector2 position)
            : base(device, position) { }

        protected override void InitializeTexture()
        {
            var desc = new D3D11.Texture2DDescription
            {
                Width = (int)Width,
                Height = (int)Height,
                ArraySize = 1,
                BindFlags = D3D11.BindFlags.ShaderResource,
                Usage = D3D11.ResourceUsage.Default,
                Format = Format.R8G8B8A8_UNorm,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0)
            };

            _texture = new D3D11.Texture2D(_device, desc);

            var colors = new RawColor4[((int)Width) * ((int)Height)];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = new RawColor4(0f, 1f, 0f, 1f);

            var context = _device.ImmediateContext;
            var box = context.MapSubresource(
                _texture,
                0,
                D3D11.MapMode.WriteDiscard,
                D3D11.MapFlags.None); // Явное указание типа

            Utilities.Write(box.DataPointer, colors, 0, colors.Length);
            context.UnmapSubresource(_texture, 0);

            _textureView = new D3D11.ShaderResourceView(_device, _texture);
        }

        public override void ApplyEffect(Player player)
        {
            if (player.Vehicle is IFuelVehicle fuelVehicle)
            {
                const float maxFuel = 100f;
                fuelVehicle.Fuel = Math.Min(fuelVehicle.Fuel + 25f, maxFuel);
            }
        }
    }

    public interface IFuelVehicle
    {
        float Fuel { get; set; }
    }
}