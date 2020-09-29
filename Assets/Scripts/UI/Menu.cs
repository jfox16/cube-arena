using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;



namespace Com.FoxGames.CubeShooter
{
    public class Menu : MonoBehaviour
    {



        #region Serializable Fields =========================================================================================



        [SerializeField]
        private List<GameObject> elements = new List<GameObject>();

        [SerializeField]
        private Dropdown windowOptionDropdown = null;



        #endregion



        #region Public Fields =========================================================================================



        public enum WindowOption {Fullscreen, Windowed}



        #endregion



        #region MonoBehaviour Callbacks ================================================================================



        private void Awake()
        {
            windowOptionDropdown = transform.Find("Menu Elements/Panel/Window Option Dropdown").GetComponent<Dropdown>();
        }



        private void Start()
        {
            Global.PauseEvent.AddListener(HandlePauseEvent);

            if (PlayerPrefs.HasKey("WindowOption"))
            {
                windowOptionDropdown.value = (int)PlayerPrefs.GetInt("WindowOption");
            }
        }



        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ClientManager.Instance.isPaused = !ClientManager.Instance.isPaused;
            }
        }



        #endregion



        #region Public Methods =========================================================================================4



        public void SetPaused(bool isPaused)
        {
            ClientManager.Instance.isPaused = isPaused;
        }



        public void HandleWindowOptionChange(Int32 value)
        {
            switch(value)
            {
                case (Int32)WindowOption.Fullscreen:
                    Screen.fullScreen = true;
                    break;

                case (Int32)WindowOption.Windowed:
                    Screen.fullScreen = false;
                    break;
            }

            PlayerPrefs.SetInt("WindowOption", (int)value);
        }



        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();

            PhotonNetwork.Disconnect();
        }



        public void QuitGame()
        {
            LeaveRoom();

            Application.Quit();
        }



        #endregion



        #region Private Methods ========================================================================================



        private void HandlePauseEvent()
        {
            bool showElements = ClientManager.Instance.isPaused;

            foreach (GameObject element in elements)
            {
                element.SetActive(showElements);
            }
        }



        #endregion



    }
}
