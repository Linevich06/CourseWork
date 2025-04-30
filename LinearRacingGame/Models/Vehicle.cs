using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using SharpDX.WIC;
using D3D11 = SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using System.Runtime.InteropServices;

namespace LinearRacingGame
{
    public abstract class Vehicle : IDisposable
    {
        public float Speed { get; protected set; } = 5f;
        public float Position { get; set; }
        protected D3D11.Texture2D _texture;
        protected D3D11.ShaderResourceView _textureView;

        public abstract void Render(D3D11.DeviceContext context);

        public virtual void Dispose()
        {
            _textureView?.Dispose();
            _texture?.Dispose();
        }

        protected D3D11.Texture2D LoadTextureFromFile(D3D11.Device device, string filePath)
        {
            var factory = new ImagingFactory();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var decoder = new BitmapDecoder(factory, stream, DecodeOptions.CacheOnDemand))
            using (var frame = decoder.GetFrame(0))
            using (var converter = new FormatConverter(factory))
            {
                converter.Initialize(
                    frame,
                    PixelFormat.Format32bppPRGBA,
                    BitmapDitherType.None,
                    null,
                    0.0,
                    BitmapPaletteType.Custom);

                var desc = new D3D11.Texture2DDescription()
                {
                    Width = converter.Size.Width,
                    Height = converter.Size.Height,
                    ArraySize = 1,
                    BindFlags = D3D11.BindFlags.ShaderResource,
                    Usage = D3D11.ResourceUsage.Immutable,
                    Format = Format.B8G8R8A8_UNorm,
                    MipLevels = 1,
                    SampleDescription = new SampleDescription(1, 0)
                };

                int stride = converter.Size.Width * 4;
                int bufferSize = stride * converter.Size.Height;

                // Создаем DataStream нужного размера
                using (var dataStream = new DataStream(bufferSize, true, true))
                {
                    // Копируем пиксели напрямую в DataStream
                    converter.CopyPixels(stride, dataStream.DataPointer, bufferSize);
                    dataStream.Position = 0;

                    // Создаем текстуру
                    return new D3D11.Texture2D(device, desc, new DataRectangle(dataStream.DataPointer, stride));
                }
            }
        }
    }

    public class BasicVehicle : Vehicle
    {
        public BasicVehicle(D3D11.Device device, string texturePath = "car.png")
        {
            try
            {
                if (File.Exists(texturePath))
                {
                    _texture = LoadTextureFromFile(device, texturePath);
                }
                else
                {
                    CreateDefaultTexture(device);
                }
                _textureView = new D3D11.ShaderResourceView(device, _texture);
            }
            catch
            {
                CreateDefaultTexture(device);
                _textureView = new D3D11.ShaderResourceView(device, _texture);
            }
        }

        private void CreateDefaultTexture(D3D11.Device device)
        {
            var desc = new D3D11.Texture2DDescription()
            {
                Width = 32,
                Height = 32,
                ArraySize = 1,
                BindFlags = D3D11.BindFlags.ShaderResource,
                Usage = D3D11.ResourceUsage.Default,
                Format = Format.B8G8R8A8_UNorm,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0)
            };

            var color = new RawColor4(1.0f, 0.0f, 0.0f, 1.0f);
            var data = new RawColor4[32 * 32];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }

            // Фиксируем массив в памяти
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var dataPointer = new DataPointer(handle.AddrOfPinnedObject(), data.Length * Utilities.SizeOf<RawColor4>());
                _texture = new D3D11.Texture2D(device, desc, new DataRectangle(dataPointer.Pointer, 32 * Utilities.SizeOf<RawColor4>()));
            }
            finally
            {
                handle.Free();
            }
        }

        public override void Render(D3D11.DeviceContext context)
        {
            context.PixelShader.SetShaderResource(0, _textureView);
        }
    }
}