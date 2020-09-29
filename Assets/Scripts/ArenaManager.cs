using System;
using System.Collections;
using System.Collections.Generic;



using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



using Photon.Pun;
using Photon.Realtime;



namespace Com.FoxGames.CubeShooter
{
    public class ArenaManager : MonoBehaviourPunCallbacks
    {



        #region Static Fields



        public static ArenaManager Instance;



        #endregion



        #region Serializable Fields



        [SerializeField]
        private List<Transform> spawnLocations = new List<Transform>();

        [SerializeField]
        private Text playerLabel = null;



        #endregion



        #region Private Fields



        private ClientManager clientManager;



        #endregion



        #region MonoBehaviour Callbacks



        private void Awake()
        {
            Instance = this;
            
            clientManager = ClientManager.Instance;
        }



        private void Start()
        {
        }

        private void OnGUI()
        {
            playerLabel.text = string.Format(
                "P{0}: {1}",
                PhotonNetwork.LocalPlayer.ActorNumber,
                PhotonNetwork.LocalPlayer.NickName
            );
        }



        #endregion


        
        #region Photon Callbacks



        /// <summary>
        /// Called after the local player leaves a room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }



        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
        }



        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
        }



        #endregion



        #region Public Methods =========================================================================================



        public static Transform GetSpawnLocation(Unit.Team team)
        {
            switch (team)
            {
                case Unit.Team.Blue:
                    return Instance.spawnLocations[1];

                default:
                    return Instance.spawnLocations[0];
            }
        }



        #endregion



    }
}