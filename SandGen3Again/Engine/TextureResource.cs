using SFML.Graphics;

namespace SFML_Game_Engine
{
    public class TextureResource : Resource
    {
        public Texture resource { get; private set; }

        public TextureResource(Texture text, string name)
        {
            base.name = name;
            resource = text;
        }

        public TextureResource(string path, string name)
        {
            base.name = name;
            resource = new Texture(path);
        }

        public override void Dispose()
        {
            resource.Dispose();
        }

        public static implicit operator Texture(TextureResource resource)
        {
            return resource.resource;
        }
    }
}
