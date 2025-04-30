using SharpDX;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using System;
using SharpDX.Mathematics.Interop;

namespace LinearRacingGame
{
    public class Obstacle : IDisposable
    {
        public Vector2 Position { get; }
        public float Width { get; } = 40f;
        public float Height { get; } = 60f;
        private D3D11.Texture2D _texture;
        private D3D11.ShaderResourceView _textureView;

        public Obstacle(D3D11.Device device, Vector2 position)
        {
            Position = position;
            InitializeTexture(device);
        }

        private void InitializeTexture(D3D11.Device device)
        {
            // Создаем описание текстуры
            var textureDesc = new D3D11.Texture2DDescription
            {
                Width = (int)Width,
                Height = (int)Height,
                ArraySize = 1,
                BindFlags = D3D11.BindFlags.ShaderResource,
                Usage = D3D11.ResourceUsage.Default,
                Format = DXGI.Format.R8G8B8A8_UNorm,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                MipLevels = 1
            };

            // Создаем временную текстуру
            _texture = new D3D11.Texture2D(device, textureDesc);

            // Копируем данные через DeviceContext
            var context = device.ImmediateContext;
            var dataBox = context.MapSubresource(
                _texture,
                0,
                D3D11.MapMode.WriteDiscard,
                D3D11.MapFlags.None,
                out var stream);

            try
            {
                for (int i = 0; i < Width * Height; i++)
                {
                    stream.Write(new RawColor4(1.0f, 0.0f, 0.0f, 1.0f));
                }
            }
            finally
            {
                context.UnmapSubresource(_texture, 0);
            }

            // Создаем ShaderResourceView для текстуры
            _textureView = new D3D11.ShaderResourceView(device, _texture);
        }

        public void Render(D3D11.DeviceContext context)
        {
            // Здесь должна быть реализация отрисовки
            // Используйте _textureView для рендеринга
        }

        public void Dispose()
        {
            _textureView?.Dispose();
            _texture?.Dispose();
        }
    }
}