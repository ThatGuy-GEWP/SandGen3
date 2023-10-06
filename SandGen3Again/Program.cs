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

            World testWorld = new World(8, 4); // how large the world is in chunks.
            testWorld.scalingFactor = 4f; // scales the chunk when rendering it.

            GameObject WorldHolder = testScene.Instantiate("World");
            WorldHolder.AddComponent(testWorld);

            // Just used to outline where the mouse will place
            GameObject mouseBox = testScene.Instantiate("MouseBox");
            RenderRect rRect = new RenderRect(new Vector2(testWorld.scalingFactor, testWorld.scalingFactor));

            rRect.outlineThickness = -1f;
            rRect.fillColor = Color.Transparent;
            rRect.origin = new Vector2(0, 0);

            mouseBox.AddComponent(rRect);

            while (true)
            {
                App.DispatchEvents();
                App.Clear(new Color(5, 5, 15));

                Vector2 mousePos = (Vector2)Mouse.GetPosition(App);
                mousePos = Vector2.Floor(mousePos / testWorld.scalingFactor);
                mouseBox.position = mousePos * testWorld.scalingFactor;

                if (Mouse.IsButtonPressed(Mouse.Button.Left) && testWorld.GetElement((int)mousePos.x, (int)mousePos.y) is Air)
                {
                    testWorld.SetElementImmediate(
                        (int)mousePos.x, (int)mousePos.y, 
                        new Dust() // Change to what element you want created on left click.
                        ); 
                }

                if (Mouse.IsButtonPressed(Mouse.Button.Right) && !(testWorld.GetElement((int)mousePos.x, (int)mousePos.y) is Air))
                {
                    testWorld.SetElementImmediate((int)mousePos.x, (int)mousePos.y, new Air());
                }

                testScene.Update();


                testScene.Render(App);

                App.Display();
            }
        }
    }
}