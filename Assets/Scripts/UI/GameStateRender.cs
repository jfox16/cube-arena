using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.FoxGames.CubeShooter
{
    public class GameStateRender : MonoBehaviour
    {



        #region Serializable Fields ====================================================================================


        
        [SerializeField]
        private List<Global.GameState> gameStatesToRenderOn = new List<Global.GameState>();

        [SerializeField]
        private List<GameObject> elementsToRender = new List<GameObject>();



        #endregion



        #region Private Fields ====================================================================================



        //



        #endregion



        #region MonoBehaviour Callbacks ================================================================================



        private void Awake()
        {
            Global.OnGameStateChange.AddListener(HandleGameStateChange);
        }



        #endregion



        #region Private Methods ========================================================================================



        private void HandleGameStateChange(Global.GameState gameState)
        {
            bool isRendered = gameStatesToRenderOn.Contains(gameState);

            foreach (GameObject element in elementsToRender)
            {
                element.SetActive(isRendered);
            }
        }



        #endregion
    }
}
