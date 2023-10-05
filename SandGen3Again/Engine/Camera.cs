using SFML.Graphics;

namespace SFML_Game_Engine
{
    public class Camera
    {
        RenderWindow app;
        public View cameraView;

        public Vector2 cameraPosition
        {
            get { return (Vector2)cameraView.Center; }
            set { SetPosition(value); }
        }

        public float cameraRotation
        {
            get { return cameraView.Rotation; }
            set { cameraView.Rotation = value; }
        }

        public Vector2 cameraAreaSize
        {
            get { return cameraView.Size; }
            set { cameraView.Size = value; }
        }

        public Camera(RenderWindow app)
        {
            this.app = app;
            cameraView = new View(app.DefaultView);
        }

        public void SetPosition(Vector2 vec)
        {
            cameraView.Center = vec;
        }

        public void Update()
        {
            app.SetView(cameraView);
        }
    }
}
