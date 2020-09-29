using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



namespace Com.FoxGames.CubeShooter
{
    public class Global : MonoBehaviour
    {


        #region Static Fields ==========================================================================================



        [System.Serializable]
        public class GameStateEvent : UnityEvent<GameState> {}

        

        public enum GameState {Start, TeamSelect, Running, Over}

        public const string GAME_VERSION = "1.01";

        public static UnityEvent PauseEvent = new UnityEvent();

        public static GameStateEvent OnGameStateChange = new GameStateEvent();

        public static int TerrainMask {get; private set;}

        public static int MouseRayPlaneMask {get; private set;}



        #endregion



        #region MonoBehaviour Callbacks ================================================================================



        private void Awake()
        {
            TerrainMask = LayerMask.GetMask("Terrain");
            
            MouseRayPlaneMask = LayerMask.GetMask("Mouse Ray Plane");
        }



        #endregion
    }
}