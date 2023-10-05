using SFML.Audio;
using SFML_Game_Engine.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Game_Engine
{
    /// <summary>
    /// A Sound emiter, plays a given <see cref="SoundResource"/>.
    /// </summary>
    internal class SoundEmitter : Component
    {
        public float pan = 0f;
        public float distance = 1f;
        public float volume = 100f;
        public bool looped = false;

        // isPlaying exists so sound can resume when scene is disabled/enabled
        public bool isPlaying { get; private set; } = false;

        public bool destroyOnEnd = false;

        bool play = false;

        SoundResource _soundResource;

        // if your wondering what all this is or what its for
        // its the random garbage that stops a sound from playing when the scene is unloaded
        // it also resumes the sound when the scene reloads. im not sure why i did it like that but i did.

        public SoundResource soundResource
        {
            get { return _soundResource; }
            set
            {
                _soundResource = value;

                if (sound == null) { return; }
                bool wasPlaying = false;
                if (sound.Status == SoundStatus.Playing)
                {
                    wasPlaying = true;
                    sound.Stop();
                    sound.Dispose();
                }

                sound = new Sound(value.resource);
                _soundResource = value;
                if (wasPlaying)
                {
                    sound.Play();
                }
            }
        }
        Sound sound;


        public override void OnDisable()
        {
            if (sound != null)
            {
                sound.Pause();
            }
        }

        public override void OnEnable()
        {
            if (sound != null && isPlaying)
            {
                sound.Play();
            }
        }

        public SoundEmitter(SoundResource soundResource)
        {
            this.soundResource = soundResource;
            sound = new Sound(this.soundResource.resource);
        }

        public SoundEmitter()
        {
            sound = new Sound();
        }

        public override void Update()
        {
            sound.Loop = looped;
            sound.Volume = volume;
            sound.Position = new SFML.System.Vector3f(pan, 0, distance);

            if (play && isStarted && enabled)
            {
                sound.Play();
                play = false;
                isPlaying = true;
            }
            isPlaying = sound.Status == SoundStatus.Playing;
        }

        public void Play()
        {
            if (soundResource == null) { return; }
            play = true;
        }

        public void Stop()
        {
            if (soundResource == null) { return; }
            sound.Stop();
        }

        public override void OnDestroy()
        {
            sound.Dispose();
        }
    }
}
