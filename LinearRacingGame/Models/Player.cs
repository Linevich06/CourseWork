using SharpDX;
using System;
using System.Windows.Input;
using LinearRacingGame.Services;
using SharpDX.Direct3D11; // Добавляем директиву
using D3D11 = SharpDX.Direct3D11;

namespace LinearRacingGame
{
    public class Player : IDisposable
    {
        public Vehicle Vehicle { get; }
        private Controls _controls;

        public Player(Vehicle vehicle, Controls controls)
        {
            Vehicle = vehicle;
            _controls = controls;
        }

        public void Update()
        {
            if (Keyboard.IsKeyDown(_controls.Left))
                Vehicle.Position -= Vehicle.Speed;
            if (Keyboard.IsKeyDown(_controls.Right))
                Vehicle.Position += Vehicle.Speed;
        }

        public void Render(D3D11.DeviceContext context)  // Изменен тип параметра
        {
            Vehicle.Render(context);  // Теперь передаем DeviceContext
        }

        public void Dispose()
        {
            Vehicle.Dispose();
        }
    }
}