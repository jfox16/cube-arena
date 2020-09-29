using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;



namespace Com.FoxGames.CubeShooter
{
    public class Projectile : MonoBehaviour
    {



        #region Serializable Fields ====================================================================================

        

        [SerializeField]
        private float moveSpeed = 1.0f;

        [SerializeField]
        private int damage = 10;

        [SerializeField]
        private float lifeTime = 10.0f;

        [SerializeField]
        private Vector3 knockbackVector = Vector3.one;


        #endregion



        #region Public Fields



        public Unit.Team ownerTeam;



        #endregion



        #region Private Fields =========================================================================================

        

        private new Rigidbody rigidbody;

        private PhotonView photonView;

        private bool isLaunched;

        private Timer lifeTimer = new Timer();



        #endregion



        #region MonoBehaviour Callbacks ================================================================================

        

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();

            photonView = GetComponent<PhotonView>();

            if (!PhotonNetwork.IsConnected || photonView.IsMine)
            {
                lifeTimer.SetTime(lifeTime);
            }
        }



        private void FixedUpdate()
        {
            if (!PhotonNetwork.IsConnected || photonView.IsMine)
            {
                if (isLaunched)
                {
                    rigidbody.velocity = transform.forward * moveSpeed;
                }

                if (lifeTimer.isDone)
                {
                    Die();
                }
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            if (!PhotonNetwork.IsConnected || photonView.IsMine)
            {
                if (other.tag == "Player")
                {
                    PlayerCube player = other.GetComponent<PlayerCube>();

                    if (player && player.team != ownerTeam)
                    {
                        player.Hurt(damage);

                        player.KnockBack(transform.rotation * knockbackVector);

                        Die();
                    }
                }
                else
                {
                    Die();
                }
            }
        }



        #endregion



        #region Public Methods =========================================================================================



        public void Launch()
        {
            isLaunched = true;
        }



        #endregion



        #region Private Methods =========================================================================================

        

        private void Die()
        {
            PhotonNetwork.Destroy(gameObject);
        }



        #endregion



    }
}
