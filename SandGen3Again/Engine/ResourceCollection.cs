using SFML.Graphics;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Collection of <see cref="Resource"/>'s, used primarily to have all your assets in one place.
    /// (also preventing memory leaks/buildup with SFML's texture and SoundBuffer class)
    /// </summary>
    public class ResourceCollection
    {
        public List<Resource> resources = new List<Resource>();

        /// <summary>
        /// Collects resources from a directory, pass <see cref="null"/> if you plan on adding your own manualy.
        /// </summary>
        /// <param name="dirToCollect"></param>
        public ResourceCollection(string? dirToCollect)
        {
            if (dirToCollect == null) { Console.Write("Loading no resources."); return; }

            Console.WriteLine($"Loading folder {dirToCollect}");
            searchFolder(dirToCollect);

            Image defaultSpriteTexture = new Image(25, 25);
            for (uint x = 0; x < 25; x++)
            {
                for (uint y = 0; y < 25; y++)
                {
                    defaultSpriteTexture.SetPixel(x, y, Color.White);
                }
            }

            resources.Add(new TextureResource(new Texture(defaultSpriteTexture), "DefaultSprite"));
            defaultSpriteTexture.Dispose();
        }

        void searchFolder(string path)
        {
            if (!Directory.Exists(path)) { return; }
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string fileName = Path.GetFileName(file);

                if (extension == ".png")
                {
                    resources.Add(new TextureResource(file, fileName.Replace(extension, "")));
                    Console.WriteLine($"loaded {fileName}");
                }

                if (extension == ".wav" || extension == ".ogg")
                {
                    resources.Add(new SoundResource(file, fileName.Replace(extension, "")));
                    Console.WriteLine($"loaded {fileName}");
                }
            }

            string[] dirs = Directory.GetDirectories(path);

            foreach (string directory in dirs)
            {
                searchFolder(directory);
            }
        }

        /// <summary>
        /// Adds a <see cref="Resource"/> to this collection.
        /// </summary>
        /// <param name="resource"></param>
        public void AddResource(Resource resource)
        {
            resources.Add(resource);
        }

        /// <summary>
        /// Finds a <see cref="Resource"/> by name, case sensitive, returns null if resource could not be found
        /// </summary>
        /// <typeparam name="T">Type of resource</typeparam>
        /// <param name="name">Name of the resource</param>
        /// <returns></returns>
        public T GetResourceByName<T>(string name) where T : Resource
        {
            foreach (Resource resource in resources)
            {
                if (resource.name == name)
                {
                    if (resource is T)
                    {
                        return (T)resource;
                    }
                }
            }
            return null;
        }
    }
}
