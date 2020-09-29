using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace Com.FoxGames.CubeShooter
{
    public class TeamSelect : MonoBehaviour
    {



        #region Public Methods =========================================================================================



        public void HandleJoinTeamClick(int teamInt)
        {
            ClientManager.Instance.HandleJoinTeam((Unit.Team)teamInt);
        }



        #endregion



    }
}
