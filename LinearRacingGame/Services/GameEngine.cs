using System;
using System.Collections.Generic;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop; // Для Color4

namespace LinearRacingGame.Services
{
    public class GameEngine : IDisposable
    {
        private readonly D3D11.Device _device;
        private readonly DXGI.SwapChain _swapChain;
        private readonly D3D11.RenderTargetView _renderTargetView;
        private readonly List<IDisposable> _disposableResources = new List<IDisposable>();
        private bool _disposed = false;

        public GameEngine(D3D11.Device device, DXGI.SwapChain swapChain)
        {
            _device = device;
            _swapChain = swapChain;

            // Получаем back buffer как Texture2D (а не Surface)
            D3D11.Texture2D backBuffer = _swapChain.GetBackBuffer<D3D11.Texture2D>(0);
            _renderTargetView = new D3D11.RenderTargetView(_device, backBuffer);
            _disposableResources.Add(_renderTargetView);
            _disposableResources.Add(backBuffer); // Добавляем в список для автоматического освобождения
        }

        public void Render()
        {
            _device.ImmediateContext.ClearRenderTargetView(
                _renderTargetView,
                new RawColor4(0.1f, 0.1f, 0.1f, 1.0f)); // Используем RawColor4

            _swapChain.Present(0, DXGI.PresentFlags.None);
        }

        public void Dispose()
        {
            if (_disposed) return;

            foreach (var resource in _disposableResources)
                resource?.Dispose();

            _swapChain?.Dispose();
            _device?.Dispose();

            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    public class Controls
    {
        public System.Windows.Input.Key Left { get; set; } = System.Windows.Input.Key.A;
        public System.Windows.Input.Key Right { get; set; } = System.Windows.Input.Key.D;
        // Добавьте другие необходимые клавиши управления
    }

    public class Device
    {
        public SharpDX.Direct3D11.Device D3DDevice { get; }
        public DeviceContext Context { get; }

        public Device(SharpDX.Direct3D11.Device device)
        {
            D3DDevice = device;
            Context = device.ImmediateContext;
        }
    }
}