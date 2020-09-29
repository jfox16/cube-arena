using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;



namespace Com.FoxGames.CubeShooter
{
    public class Launcher : MonoBehaviourPunCallbacks
    {



        #region Serializable Fields


        
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so a new room will be created.")]
        [SerializeField]
        private byte maxPlayersPerRoom = 4;

        [SerializeField]
        private InputField usernameInputField = null;

        [SerializeField]
        private Button playButton = null;

        [SerializeField]
        private Text progressLabel = null;



        #endregion



        #region Private Fields



        enum State {
            None,
            Connecting,
            Connected
        };

        private State state = State.None;



        private string errorMessage = "";



        #endregion



        #region MonoBehaviour CallBacks



        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        private void Awake()
        {
            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        } 



        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        private void Start()
        {
        }



        private void OnGUI()
        {
            if (usernameInputField)
            {
                usernameInputField.interactable = (state == State.None);
            }

            if (playButton)
            {
                playButton.interactable = (state == State.None);
            }

            if (progressLabel)
            {
                switch(state)
                {
                    case State.None:
                        progressLabel.text = errorMessage;
                        break;
                    case State.Connecting:
                        progressLabel.text = "Connecting...";
                        break;
                    case State.Connected:
                        progressLabel.text = "Connected!";
                        break;
                }
            }
        }



        #endregion



        #region Public Methods



        /// <summary>
        /// Start the connection process.
        /// - If already connected, we attempt joining a random room
        /// - If not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            // we check if we are connected or not, we join if we are, else we initiate the connection to the server.
            if (PhotonNetwork.IsConnected)
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
                state = State.Connected;
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = Global.GAME_VERSION;
                state = State.Connecting;
            }
        }
    


        #endregion



        #region Private Methods



        #endregion



        #region MonoBehaviourPunCallbacks Callbacks



        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            state = State.Connected;

            // Set nick name

            if (usernameInputField)
            {
                string nickName = usernameInputField.text;

                if (nickName.Length == 0)
                {
                    nickName = "Nobody";
                }

                PhotonNetwork.NickName = nickName;
                
                PlayerPrefs.SetString("NickName", nickName);
            }

            // #Critical: The first thing we try to do is to join a potential existing room. If there is, good.
            // Else, we'll be called back with OnJoinRandomFailed().
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed was called by PUN. No random room available, so we create one. \nCalling: PhotonNetwork.CreateRoom");

            // #Critical: we failed to join a random room, maybe none exist or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

            if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                PhotonNetwork.LoadLevel("Arena");
            }
        }



        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason: {0}", cause);

            state = State.None;

            errorMessage = string.Format("Could not connect to the server.\n{0}", cause);
        }



        #endregion


    }
}