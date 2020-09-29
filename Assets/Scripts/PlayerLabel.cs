using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Com.FoxGames.CubeShooter
{
    public class PlayerLabel : MonoBehaviour
    {



        #region Serializable Fields



        //



        #endregion



        #region Private Fields



        private TextMesh textMesh = null;



        #endregion



        #region MonoBehaviour Callbacks



        private void Awake()
        {
            textMesh = GetComponent<TextMesh>();
        }



        private void Update()
        {
            transform.rotation = ClientManager.GetCameraRotation();
        }



        #endregion



        #region Public Methods



        public void SetText(string text)
        {
            if (textMesh)
            {
                textMesh.text = text;
            }
        }



        #endregion
    }
}