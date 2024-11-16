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
            _pitch = (_carBehavior.engineRPM / _carBehavior.maxRPM);
            _audio.pitch = _minPitch + _pitch * _maxPitch;
        }

        public void Update()
        {
            SetPitch();
        }
    }
}
