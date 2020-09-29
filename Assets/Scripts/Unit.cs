using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Com.FoxGames.CubeShooter
{
    public abstract class Unit : MonoBehaviour
    {



        #region Public Fields ====================================================================================



        public enum Team { Neutral, Red, Blue }

        public Team team = Team.Neutral;

        public bool isAlive = true;



        #endregion



        #region Public Methods



        public abstract void Hurt(int damage);

        public abstract void KnockBack(Vector3 knockbackVector);



        #endregion



    }
}
