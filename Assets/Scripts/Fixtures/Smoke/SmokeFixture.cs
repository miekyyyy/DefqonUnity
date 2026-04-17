using UnityEngine;

namespace DefqonEngine
{
    public class SmokeFixture : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _smokeParticleSystem;

        private bool _isActive;

        public void Trigger(bool active)
        {
            //TODO: Playback time meerekenen, zodat je ook in het midden van een event kunt springen en de juiste state krijgt
            if (_smokeParticleSystem == null)
                return;

            if (active == _isActive)
                return;

            var main = _smokeParticleSystem.main;
            main.loop = true;

            if (active)
                _smokeParticleSystem.Play();
            else
                _smokeParticleSystem.Stop();

            _isActive = active;
        }
    }
}