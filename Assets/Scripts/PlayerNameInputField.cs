using UnityEngine;
using UnityEngine.UI;



using Photon.Pun;
using Photon.Realtime;



using System.Collections;



namespace Com.FoxGames.CubeShooter
{
    /// <summary>
    /// Player name input field. Let the user input their name, will appear above the player in the game.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants



        #endregion



        #region MonoBehaviour Callbacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start () {



            string defaultName = string.Empty;

            InputField _inputField = this.GetComponent<InputField>();
            
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey("NickName"))
                {
                    defaultName = PlayerPrefs.GetString("NickName");
                    _inputField.text = defaultName;
                }
            }
        }



        #endregion
    }
}