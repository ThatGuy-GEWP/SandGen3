using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Diagnostics;


namespace SFML_Game_Engine
{
    /// <summary>
    /// Keeps track of <see cref="deltaTime"/>, updates gameobjects, renders the scene with the <see cref="renderManager"/> 
    /// and holds the <see cref="activeCamera"/>
    /// </summary>
    public class Scene : IDisposable
    {
        public Dictionary<string, string> sceneVariables = new Dictionary<string, string>();

        public string name = "Untitled Scene";

        public Camera activeCamera;

        public ResourceCollection resources;

        public RenderManager renderManager;

        public RenderWindow app;

        public float deltaTime { get; private set; } = 0;
        bool active = true;

        GameObject root = new GameObject(null, "Scene Root");

        public List<GameObject> instances { get { return root.children; } }

        bool started = false;

        /// <summary>
        /// Gets the mouse position in screen space
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMouseWindowPosition()
        {
            return (Vector2)Mouse.GetPosition(app);
        }

        /// <summary>
        /// Gets the mouse position in world space.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePosition()
        {
            return app.MapPixelToCoords((Vector2i)GetMouseWindowPosition());
        }

        public Scene(string Name, RenderWindow App, string? resourcePath)
        {
            activeCamera = new Camera(App);
            name = Name;
            app = App;
            renderManager = new RenderManager();
            resources = new ResourceCollection(resourcePath);
        }

        public Scene(string Name, RenderWindow App, ResourceCollection resourceCollection)
        {
            activeCamera = new Camera(App);
            name = Name;
            app = App;
            renderManager = new RenderManager();
            this.resources = resourceCollection;
        }

        public Scene(string Name, RenderWindow App)
        {
            activeCamera = new Camera(App);
            name = Name;
            app = App;
            renderManager = new RenderManager();
            resources = new ResourceCollection(null);
        }

        public void Render(RenderTarget rt)
        {
            activeCamera.Update();
            renderManager.Render(rt);
        }

        public GameObject Instantiate()
        {
            GameObject newGm = new GameObject(root, "Unnamed", this);
            return newGm;
        }

        public GameObject Instantiate(string Name)
        {
            GameObject newGm = new GameObject(root, Name, this);
            return newGm;
        }

        public GameObject Instantiate(GameObject parent)
        {
            GameObject newGm = new GameObject(parent, "Unnammed", this);
            return newGm;
        }

        public GameObject Instantiate(string Name, GameObject parent)
        {
            GameObject newGm = new GameObject(parent, Name, this);
            return newGm;
        }

        /// <summary>
        /// Searches entire gameobject tree for child with the matching name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GameObject? GetInstance(string name)
        {
            foreach (GameObject gm in root.children)
            {
                if (gm.name == name)
                {
                    return gm;
                }

                GameObject? search = gm.GetDescendant(name);
                if (search != null)
                {
                    return search;
                }
            }
            return null;
        }

        /// <summary>
        /// Searches entire gameobject tree for the first child with the tag <paramref name="tag"/>.
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <returns></returns>
        public GameObject? GetInstanceByTag(string tag)
        {
            foreach (GameObject gm in root.children)
            {
                if (gm.tag == tag)
                {
                    return gm;
                }

                GameObject? search = gm.GetDescendantByTag(tag);
                if (search != null)
                {
                    return search;
                }
            }
            return null;
        }

        public List<GameObject> GetInstancesByTag(string tag)
        {
            List<GameObject> found = new List<GameObject>();

            foreach (GameObject gm in root.children)
            {
                if (gm.tag == tag) { found.Add(gm); }
                found.AddRange(gm.GetDescendantsByTag(tag));
            }

            return found;
        }

        Stopwatch deltaWatch = new Stopwatch();
        Stopwatch physicsWatch = new Stopwatch();

        public void Start()
        {
            if (started) { return; }
            root.Start();
            started = true;
            deltaWatch.Start();
            physicsWatch.Start();
        }

        public void Update()
        {
            if (!started) { this.Start(); }
            root.Update();

            deltaTime = deltaWatch.ElapsedMilliseconds * 0.001f;
            deltaWatch.Restart();
        }

        /// <summary>
        /// if <paramref name="value"/> is true, the mouse cursor will be visible, 
        /// if false then the cursor will not be visible when within the window
        /// </summary>
        /// <param name="value"></param>
        public void SetMouseVisible(bool value)
        {
            app.SetMouseCursorVisible(value);
        }

        /// <summary>
        /// If <paramref name="value"/> is true, the mouse will not be locked into the window, 
        /// if false then the mouse can leave the window
        /// </summary>
        /// <param name="value"></param>
        public void KeepMouseInScreen(bool value)
        {
            app.SetMouseCursorGrabbed(value);
        }

        public void Disable()
        {
            foreach (GameObject g in root.children)
            {
                g.enabled = false;
            }
        }

        public void Enable()
        {
            foreach (GameObject g in root.children)
            {
                g.enabled = true;
            }
        }

        public void Dispose()
        {
            foreach (GameObject child in root.children)
            {
                child.Destroy();
            }
        }
    }
}
