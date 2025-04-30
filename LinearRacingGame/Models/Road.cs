using SharpDX;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using D2D1 = SharpDX.Direct2D1;
using System;
using SharpDX.Mathematics.Interop;

namespace LinearRacingGame
{
    public class Road : IDisposable
    {
        private D2D1.Bitmap _bitmap;
        private float _length;

        public Road(D3D11.Device device, DXGI.SwapChain swapChain, float length)
        {
            _length = length;

            // Создаем фабрику Direct2D
            var d2dFactory = new D2D1.Factory();

            // Получаем back buffer из swap chain
            using (var backBuffer = swapChain.GetBackBuffer<DXGI.Surface>(0))
            {
                // Создаем RenderTarget
                var renderTarget = new D2D1.RenderTarget(
                    d2dFactory,
                    backBuffer,
                    new D2D1.RenderTargetProperties(
                        new D2D1.PixelFormat(DXGI.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied)));

                // Создаем битмап дороги
                _bitmap = new D2D1.Bitmap(
                    renderTarget,
                    new Size2((int)length, 50),
                    new D2D1.BitmapProperties(
                        new D2D1.PixelFormat(DXGI.Format.B8G8R8A8_UNorm, D2D1.AlphaMode.Premultiplied)));
            }
        }

        public void Render(D3D11.Device device)
        {
            // Реализация рендеринга дороги
        }

        public void Dispose()
        {
            _bitmap?.Dispose();
        }
    }
}