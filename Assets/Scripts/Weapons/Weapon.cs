using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;



namespace Com.FoxGames.CubeShooter
{
    // Lives on client-side only
    public class Weapon : MonoBehaviour
    {



        #region Serializable Fields ====================================================================================



        [SerializeField]
        public string fireEffectResource = null;

        [SerializeField]
        private string projectileResource = null;

        [SerializeField]
        private int maxClipAmmo = 6;

        [SerializeField]
        private int maxReserveAmmo = 18;

        [SerializeField]
        private float fireDelay = 0.1f;

        [SerializeField]
        private float reloadDelay = 0.1f;



        #endregion



        #region Public Fields =========================================================================================



        public int clipAmmo {get; private set;}

        public int reserveAmmo {get; private set;}

        public bool isReloading {get; private set;}

        public GameObject fireEffectPrefab {get; private set;}



        #endregion



        #region Private Fields =========================================================================================



        private Timer fireTimer = null;

        private Timer reloadTimer = null;



        #endregion



        #region MonoBehaviour Callbacks ================================================================================

        

        private void Start()
        {
            clipAmmo = maxClipAmmo;

            reserveAmmo = maxReserveAmmo;

            fireTimer = new Timer();

            reloadTimer = new Timer();
        }



        private void Update()
        {
            // if (isReloading)
            // {
            //     if (clipAmmo < maxClipAmmo && reserveAmmo > 0)
            //     {
            //         clipAmmo++;

            //         reloadTimer.SetTime(reloadDelay);
            //     }
            //     else
            //     {
            //         isReloading = false;
            //     }
            // }

            if (clipAmmo < maxClipAmmo && reserveAmmo > 0)
            {
                if (isReloading)
                {
                    if (reloadTimer.isDone)
                    {
                        reserveAmmo--;

                        clipAmmo++;

                        reloadTimer.SetTime(reloadDelay);
                    }
                }
                else
                {
                    isReloading = true;

                    reloadTimer.SetTime(reloadDelay);
                }
            }
            else
            {
                isReloading = false;
            }
        }



        #endregion



        #region Public Methods =========================================================================================

        

        public bool Fire(Unit owner)
        {
            if (fireTimer.isDone && clipAmmo > 0)
            {
                Projectile projectile = PhotonNetwork.Instantiate(
                    projectileResource,
                    transform.position,
                    transform.rotation
                ).GetComponent<Projectile>();

                if (projectile)
                {
                    projectile.ownerTeam = owner.team;

                    projectile.Launch();
                }
                
                clipAmmo--;

                fireTimer.SetTime(fireDelay);

                isReloading = false;

                return true;
            }

            return false;
        }



        #endregion



        #region RPC Methods ========================================================================================



        #endregion



        #region Private Methods ========================================================================================



        private void Reload()
        {
            if (!isReloading)
            {
                isReloading = true;
                reloadTimer.SetTime(reloadDelay);
            }
        }

        

        private void Die()
        {
            PhotonNetwork.Destroy(gameObject);
        }



        #endregion



    }
}
