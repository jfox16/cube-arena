using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



using Photon.Pun;



namespace Com.FoxGames.CubeShooter
{
    public class ClientManager : MonoBehaviour
    {



        #region Static Fields ====================================================================================



        public static ClientManager Instance {get; private set;}



        #endregion



        #region Serializable Fields ====================================================================================


        
        [SerializeField]
        private float cameraAngle = 45.0f;
        
        [SerializeField]
        private float cameraDistance = 10.0f;

        [SerializeField]
        private float respawnTime = 5.0f;



        #endregion



        #region Public Fields ==========================================================================================



        public Global.GameState gameState
        {
            get
            {
                return _gameState;
            }

            set
            {
                _gameState = value;

                Global.OnGameStateChange.Invoke(value);
            }
        }

        [HideInInspector]
        public GameObject target = null;

        [HideInInspector]
        public PlayerCube player = null;

        public bool isPaused
        {
            get
            {
                return _isPaused;
            }

            set
            {
                _isPaused = value;

                Global.PauseEvent.Invoke();
            }
        }

        public Unit.Team playerTeam {get; private set;}

        [HideInInspector]
        public Transform spawnLocation = null;



        #endregion



        #region Private Fields



        private Global.GameState _gameState = Global.GameState.Start;

        private Transform cameraPositionerTransform = null;

        private bool _isPaused = false;

        private bool isRespawning = false;

        private Timer respawnTimer = null;



        #endregion



        #region MonoBehaviour Callbacks ================================================================================



        private void Awake()
        {
            Instance = this;

            cameraPositionerTransform = transform.Find("Camera Positioner");

            respawnTimer = new Timer();
        }



        private void Start()
        {
            gameState = Global.GameState.TeamSelect;
        }



        private void Update()
        {
            if (cameraPositionerTransform && target)
            {
                // Follow target

                transform.position = target.transform.position;

                cameraPositionerTransform.localPosition = new Vector3(
                    0,
                    cameraDistance * Mathf.Sin(Mathf.Deg2Rad * cameraAngle),
                    -cameraDistance * Mathf.Cos(Mathf.Deg2Rad * cameraAngle)
                );

                cameraPositionerTransform.localRotation = Quaternion.Euler( cameraAngle, 0, 0 );
            }

            if (isRespawning)
            {
                if (respawnTimer.GetTime() < 1.0f)
                {
                    transform.position = spawnLocation.position;
                }

                if (respawnTimer.isDone)
                {
                    SpawnPlayer();

                    isRespawning = false;
                }
            }
        }



        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                isPaused = true;
            }
        }



        #endregion



        #region Public Methods =========================================================================================



        public static Quaternion GetCameraRotation()
        {
            return Instance.cameraPositionerTransform.rotation;
        }

        public static void StartRespawn()
        {
            Instance.isRespawning = true;

            Instance.respawnTimer.SetTime(Instance.respawnTime);
        }



        public void HandleJoinTeam(Unit.Team team)
        {
            playerTeam = team;

            spawnLocation = ArenaManager.GetSpawnLocation(playerTeam);

            transform.rotation = spawnLocation.rotation; // rotate clientManager to match spawnLocation

            SpawnPlayer();



            gameState = Global.GameState.Running;
        }



        #endregion



        #region Private Methods ========================================================================================



        private void SpawnPlayer()
        {
            string playerResource = "";

            if (playerTeam == Unit.Team.Red)
            {
                playerResource = "Player Cube";
            }
            else if (playerTeam == Unit.Team.Blue)
            {
                playerResource = "Player Cube Blue";
            }

            

            GameObject playerGo;

            if (PhotonNetwork.IsConnected)
            {
                playerGo = PhotonNetwork.Instantiate(
                    playerResource,
                    spawnLocation.position,
                    Quaternion.identity
                );
            }
            else
            {
                playerGo = Instantiate(
                    Resources.Load<GameObject>(playerResource),
                    spawnLocation.position,
                    Quaternion.identity
                );
            }



            if (playerGo)
            {
                target = playerGo;

                player = playerGo.GetComponent<PlayerCube>();
            }
        }



        #endregion
    }
}
