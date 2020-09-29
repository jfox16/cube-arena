using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Com.FoxGames.CubeShooter
{
    public class AmmoCounter : MonoBehaviour
    {



        #region Private Fields =========================================================================================



        private Text clipAmmoText = null;

        private Text reserveAmmoText = null;



        #endregion



        #region MonoBehaviour Callbacks ================================================================================



        private void Awake()
        {
            clipAmmoText = transform.Find("Clip Ammo").GetComponent<Text>();

            reserveAmmoText = transform.Find("Reserve Ammo").GetComponent<Text>();
        }



        private void Update()
        {
            if (ClientManager.Instance && ClientManager.Instance.player && ClientManager.Instance.player.equippedWeapon)
            {
                Weapon targetWeapon = ClientManager.Instance.player.equippedWeapon;

                if (targetWeapon)
                {
                    clipAmmoText.text = targetWeapon.clipAmmo.ToString();

                    reserveAmmoText.text = targetWeapon.reserveAmmo.ToString();
                }
            }
            else
            {
                clipAmmoText.text = "";

                reserveAmmoText.text = "";
            }
        }



        #endregion



    }
}
