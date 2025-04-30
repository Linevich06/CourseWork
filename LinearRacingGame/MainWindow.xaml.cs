using System;
using System.Windows;
using System.Windows.Forms.Integration;
using LinearRacingGame.Services;
using System.Windows.Media;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace LinearRacingGame
{
    public partial class MainWindow : Window
    {
        private D3D11.Device _device;
        private DXGI.SwapChain _swapChain;
        private GameEngine _gameEngine;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeDirect3D();
                _gameEngine = new GameEngine(_device, _swapChain);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}");
                Close();
            }
        }

        private void InitializeDirect3D()
        {
            var swapChainDesc = new DXGI.SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new DXGI.ModeDescription(
                    RenderControl.ClientSize.Width,
                    RenderControl.ClientSize.Height,
                    new DXGI.Rational(60, 1),
                    DXGI.Format.R8G8B8A8_UNorm),
                Usage = DXGI.Usage.RenderTargetOutput,
                OutputHandle = RenderControl.Handle,
                SampleDescription = new DXGI.SampleDescription(1, 0),
                SwapEffect = DXGI.SwapEffect.Discard,
                IsWindowed = true
            };

            // Исправленный вызов с правильным DriverType
            D3D11.Device.CreateWithSwapChain(
                D3D.DriverType.Hardware, // Используем пространство имен SharpDX.Direct3D
                D3D11.DeviceCreationFlags.BgraSupport,
                swapChainDesc,
                out _device,
                out _swapChain);
        }

        private void OnRenderControlSizeChanged(object sender, EventArgs e)
        {
            if (_swapChain == null) return;

            _device.ImmediateContext.ClearState();
            _swapChain.ResizeBuffers(
                1,
                RenderControl.ClientSize.Width,
                RenderControl.ClientSize.Height,
                DXGI.Format.R8G8B8A8_UNorm,
                DXGI.SwapChainFlags.None);
        }

        private void OnStartClick(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        {
            // Проверяем наличие метода Update перед вызовом
            _gameEngine?.Render();

            // Если GameEngine должен обновляться, добавьте метод Update в класс GameEngine
            // _gameEngine?.Update(); 
        }

        protected override void OnClosed(EventArgs e)
        {
            _gameEngine?.Dispose();
            _swapChain?.Dispose();
            _device?.Dispose();
            base.OnClosed(e);
        }
    }
}