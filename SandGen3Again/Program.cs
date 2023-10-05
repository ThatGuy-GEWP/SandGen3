using SandGen3Again.Elements;
using SandGen3Again.Scripts;
using SFML.Graphics;
using SFML.Window;
using SFML_Game_Engine;

namespace SandGen3Again
{
    internal class Program
    {
        public static RenderWindow App { get; private set; } = new RenderWindow(new VideoMode(1280, 640), "SandGen3 Again", Styles.Titlebar | Styles.Close);

        static void Main(string[] args)
        {
            App.Closed += (sender, args) => { App.Close(); throw new Exception("Closed"); };

            Scene testScene = new Scene("Testing Scene", App, "Res\\");

            World testWorld = new World(8, 4);

            GameObject WorldHolder = testScene.Instantiate("World");
            WorldHolder.AddComponent(testWorld);

            GameObject mouseBox = testScene.Instantiate("MouseBox");
            RenderRect rr = new RenderRect(new Vector2(4, 4));
            rr.outlineThickness = -1f;
            rr.fillColor = Color.Transparent;
            mouseBox.AddComponent(rr).origin = new Vector2(0,0);

            while (true)
            {
                App.DispatchEvents();
                App.Clear(new Color(5, 5, 15));

                Vector2 mousePos = (Vector2)Mouse.GetPosition(App);
                mousePos = Vector2.Floor(mousePos / 4);
                mouseBox.position = mousePos*4;

                if (Mouse.IsButtonPressed(Mouse.Button.Left) && testWorld.GetElement((int)mousePos.x, (int)mousePos.y) is Air)
                {
                    testWorld.SetElement((int)mousePos.x, (int)mousePos.y, new Water());
                }

                if (Mouse.IsButtonPressed(Mouse.Button.Right) && !(testWorld.GetElement((int)mousePos.x, (int)mousePos.y) is Air))
                {
                    testWorld.SetElement((int)mousePos.x, (int)mousePos.y, new Air());
                }

                testScene.Update();

                testScene.Render(App);

                App.Display();
            }
        }
    }
}