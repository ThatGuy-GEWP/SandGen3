using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    public abstract class Component
    {
        public GameObject gameObject = null!; // Cool new thing i just learned! (null!)
        public bool isStarted = false;
        public float deltaTime = 0f;

        bool _enabled = true;

        public bool enabled
        {  // just in case i want to do anything with this.
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (value)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
            }
        }

        /// <summary>
        /// Called when the component is enabled.
        /// </summary>
        public virtual void OnEnable()
        {
            return;
        }

        /// <summary>
        /// Called when the component is disabled.
        /// </summary>
        public virtual void OnDisable()
        {
            return;
        }

        /// <summary>
        /// Called when this component is being cloned
        /// Use to implment custom cloning logic.
        /// </summary>
        public virtual object? Clone()
        {
            return null;
        }

        /// <summary>
        /// Called when the Component is being destroyed
        /// </summary>
        public virtual void OnDestroy()
        {

        }

        /// <summary>
        /// Called when the Component is added to a <see cref="GameObject"/>
        /// </summary>
        public virtual void OnAdded()
        {
            return;
        }

        /// <summary>
        /// Called when the scene starts for the first time.
        /// </summary>
        public virtual void Start()
        {
            return;
        }

        /// <summary>
        /// Called every frame, get deltaTime with gameObject.scene.deltaTime
        /// </summary>
        public virtual void Update()
        {
            return;
        }
    }
}
