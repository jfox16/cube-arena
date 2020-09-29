using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.FoxGames.CubeShooter
{
    public class MouseInput : MonoBehaviour
    {



        #region Static Fields ==========================================================================================

        

        public static MouseInput Instance;

        public static bool IsOnMouseRayPlane = false;

        public static Vector3 MousePoint = Vector3.zero;



        #endregion



        #region MonoBehaviour Callbacks ================================================================================



        private void Awake()
        {
            Instance = this;
        }



        private void Update()
        {
            RaycastHit hitInfo;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, Global.MouseRayPlaneMask))
            {
                IsOnMouseRayPlane = true;

                MousePoint = hitInfo.point;
            }
            else
            {
                IsOnMouseRayPlane = false;
            }
        }



        #endregion



    }
}
