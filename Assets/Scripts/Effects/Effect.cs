using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Com.FoxGames.CubeShooter
{
    // Don't instantiate Effect on Photon Network. Each client should Instantiate their own Effects
    public class Effect : MonoBehaviour
    {



        #region Serializable Fields



        [SerializeField]
        private float lifeTime = 10.0f;



        #endregion



        #region Private Fields



        private new ParticleSystem particleSystem = null;

        private Timer lifeTimer = null;



        #endregion



        #region MonoBehaviour Callbacks



        private void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();

            particleSystem.Play();

            lifeTimer = new Timer(lifeTime);
        }



        private void Update()
        {
            if (lifeTimer.isDone)
            {
                Die();
            }
        }



        #endregion



        #region Private Methods



        private void Die()
        {
            Destroy(gameObject);
        }



        #endregion
    }
}