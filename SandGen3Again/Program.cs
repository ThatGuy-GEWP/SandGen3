using SandGen3Again.Elements;
using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine;
using System.Diagnostics;

namespace SandGen3Again
{
    internal class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 720), "SandGen3Again", Styles.Titlebar | Styles.Close);

        static void Main(string[] args)
        {
            App.Closed += (sender, args) => { App.Close(); throw new Exception("Closed"); };

            Scene testScene = new Scene("Testing Scene", App, "Res\\");

            World testWorld = new World(2, 2);

            GameObject WorldHolder = testScene.Instantiate("World");
            WorldHolder.AddComponent(testWorld);

            while (true)
            {
                Vector2 mousePos = (Vector2)Mouse.GetPosition(App);
                mousePos = Vector2.Floor(mousePos / 4);

                if (Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    testWorld.SetElement((int)mousePos.x, (int)mousePos.y, new Sand());
                }

                App.DispatchEvents();
                App.Clear();

                testScene.Update();

                testScene.Render(App);

                testWorld.TestDraw(App, 4);

                App.Display();
            }
        }
    }
}