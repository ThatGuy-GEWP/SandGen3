using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    public class GameObject
    {
        public string name;
        bool _enabled = true;

        public bool destroyed { get; private set; } = false;

        public bool enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                foreach (GameObject g in children)
                {
                    g.enabled = value;
                }
                foreach (Component c in components)
                {
                    c.enabled = value;
                }
            }
        }

        /// <summary>
        /// The <see cref="Scene"/> this GameObject is in
        /// </summary>
        public Scene scene;

        /// <summary>
        /// Like a second name, you can also find gameobjects by tag with 
        /// </summary>
        public string tag = string.Empty;

        public GameObject parent { get; private set; }

        Vector2 _position = new Vector2(0, 0);

        /// <summary>
        /// Position relative to parent.
        /// </summary>
        public Vector2 position
        {
            get
            {
                // we love ternary operators
                return parent == null ? _position : _position + parent.position;
            }
            set
            {
                if (value != _position)
                {
                    //Using ?. makes sure we dont invoke a null action, neat!
                    PositionChanged?.Invoke(this);
                    foreach (GameObject g in children)
                    {
                        g.PositionChanged?.Invoke(g);
                    }
                }
                _position = value;
            }
        }


        float _rotation = 0;

        /// <summary>
        /// Rotation relative to parent
        /// </summary>
        public float rotation
        {
            get
            {
                return parent == null ? _rotation : _rotation + parent.rotation;
            }
            set
            {
                if (value != _rotation)
                {
                    RotationChanged?.Invoke(this);
                    foreach (GameObject g in children)
                    {
                        g.RotationChanged?.Invoke(g);
                    }
                }
                _rotation = value;
            }
        }



        /// <summary>
        /// Local rotation not relative to parent
        /// </summary>
        public float localRotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }
        }

        /// <summary>
        /// Fires when this gameObjects position is changed, or the parents <see cref="position"/> is changed.
        /// </summary>
        public event Action<GameObject> PositionChanged;

        /// <summary>
        /// Fires when this gameobjects rotation is changed, or the <see cref="parent"/>'s rotation is changed.
        /// </summary>
        public event Action<GameObject> RotationChanged;

        /// <summary>
        /// Position of the game object in the world, relative to parent
        /// </summary>
        public Vector2 localPosition { get { return _position; } set { _position = value; } }

        public List<Component> components { get; private set; } = new List<Component>();
        public List<GameObject> children { get; private set; } = new List<GameObject>();

        public GameObject(GameObject parent, string name, Scene scene)
        {
            this.name = name;
            this.scene = scene;
            if (parent == null) { return; }
            SetParent(parent);
        }

        public GameObject(GameObject parent, string name)
        {
            this.name = name;
            if (parent == null) { return; }
            SetParent(parent);
        }

        /// <summary>
        /// Gets the first child in <see cref="children"/> has a name that matches <paramref name="name"/>
        /// </summary>
        public GameObject? GetChild(string name)
        {
            foreach (GameObject gm in children)
            {
                if (gm.name == name) return gm;
            }
            return null;
        }

        /// <summary>
        /// Gets all <see cref="children"/> that match the <paramref name="name"/> parameter
        /// </summary>
        /// <param name="name"></param>
        public List<GameObject> GetChildren(string name)
        {
            List<GameObject> foundChildren = new List<GameObject>();
            foreach (GameObject gm in children)
            {
                if (gm.name == name) foundChildren.Add(gm);
            }
            return foundChildren;
        }

        public GameObject? GetDescendant(string name)
        {
            foreach (GameObject gm in children)
            {
                if (gm.name == name) return gm;
                if (gm.children.Count > 0) return gm.GetDescendant(name);
            }
            return null;
        }

        /// <summary>
        /// Gets the first child or child of a child that matches the supplied <paramref name="tag"/>
        /// </summary>
        /// <param name="tag">Tag to search with</param>
        /// <returns></returns>
        public GameObject? GetDescendantByTag(string tag)
        {
            foreach (GameObject gm in children)
            {
                if (gm.name == tag) return gm;
                if (gm.children.Count > 0) return gm.GetDescendant(tag);
            }
            return null;
        }

        /// <summary>
        /// Gets all children and children of children that match the supplied <paramref name="tag"/>
        /// </summary>
        public List<GameObject> GetDescendantsByTag(string tag)
        {
            List<GameObject> foundObjects = new List<GameObject>();
            foreach (GameObject gm in children)
            {
                if (gm.name == tag) { foundObjects.Add(gm); };
                if (gm.children.Count > 0) { foundObjects.AddRange(gm.GetDescendantsByTag(tag)); };
            }

            return foundObjects;
        }


        /// <summary>
        /// Changes the <see cref="parent"/> of this gameobject
        /// </summary>
        /// <param name="newParent"></param>
        public void SetParent(GameObject newParent)
        {
            if (scene == null) { scene = newParent.scene; }
            foreach (var child in newParent.children)
            {
                if (child.GetHashCode() == GetHashCode())
                {
                    return; // Dont add yourself to this parent for a second time lmao.
                }
            }

            if (parent != null)
            {
                parent.children.Remove(this); // remove self from old parent
            }

            parent = newParent;
            newParent.children.Add(this);
        }

        public T AddComponent<T>(T comp) where T : Component
        {
            comp.gameObject = this;
            components.Add(comp);
            comp.OnAdded();
            return comp;
        }

        public T[] GetComponents<T>() where T : Component
        {
            List<T> list = new List<T>();
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    list.Add((T)components[i]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets the first components of type <typeparamref name="T"/>, returns null if component could not be found
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    return (T)components[i];
                }
            }
            return null;
        }

        public bool HasComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                {
                    return true;
                }
            }
            return false;
        }

        public void RemoveComponent(Component comp)
        {
            components.Remove(comp); // not sure if this will work but fuck it we ball
        }

        public GameObject Clone()
        {
            GameObject newClone = new GameObject(parent, $"Clone of {name}");

            foreach (Component comp in components)
            {
                if (comp is ICloneable)
                {
                    Component? newComp = (Component?)comp.Clone();
                    if (newComp != null)
                    {
                        newClone.AddComponent(newComp);
                    }
                    else
                    {
                        Console.WriteLine($"Component {comp.GetType().Name} cannot be Cloned!");
                    }
                }
            }

            return newClone;
        }

        public void Start()
        {
            for (int i = 0; i < components.Count; i++)
            {
                components[i].Start();
                components[i].isStarted = true;
            }
        }

        public void Update()
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].enabled == false) { continue; }
                if (components[i].isStarted == false) { components[i].Start(); components[i].isStarted = true; continue; }
                components[i].deltaTime = scene.deltaTime;
                components[i].Update();
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].enabled == false) { continue; }
                children[i].Update();
            }
        }

        public void Destroy()
        {
            destroyed = true;
            foreach (Component component in components)
            {
                component.OnDestroy();
            }

            foreach (GameObject child in children)
            {
                child.Destroy();
            }

            components.Clear();
            parent.children.Remove(this);
        }
    }
}
