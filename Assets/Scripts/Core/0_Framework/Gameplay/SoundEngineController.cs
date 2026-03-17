using UnityEngine;

namespace Beetle.Framework
{
    public class SoundEngineController : MonoBehaviour
    {
        [SerializeField] private CarBehavior _carBehavior;
        [SerializeField] private AudioSource _audio;
        [SerializeField] private float _maxPitch;
        [SerializeField] private float _minPitch;
        [SerializeField] private float _pitch;

        public void SetPitch()
        {
            var engine = _carBehavior.Engine;
            _pitch = (engine.CurrentRPM / engine.MaxRPM);
            _audio.pitch = _minPitch + _pitch * _maxPitch;
        }

        public void Update()
        {
            SetPitch();
        }
    }
}
