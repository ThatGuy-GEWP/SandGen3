using SFML.Graphics;
using SFML_Game_Engine;

namespace SandGen3Again.Scripts
{
    /// <summary>
    /// Used to define a selection of colors to pick from.
    /// Salvaged from my older project.
    /// </summary>
    public class ColorPalette
    {
        Color[] colors;

        public ColorPalette(params string[] args)
        {
            colors = new Color[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                Color curCol = new Color((uint)Convert.ToInt32($"{args[i].Replace("#", "")}FF", 16));


                colors[i] = new Color(curCol);
            }
        }

        public ColorPalette(params uint[] args)
        {
            colors = new Color[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                colors[i] = new Color(args[i]);
            }
        }

        public ColorPalette(params Color[] args)
        {
            colors = args;
        }

        public Color pickRandom()
        {
            return colors[RandomGen.Next(0, colors.Length)];
        }
    }
}
