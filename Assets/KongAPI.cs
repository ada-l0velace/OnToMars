namespace kongregate {

    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;

    public class KongAPI : MonoBehaviour {

        private static KongAPI m_instance;

        internal static bool isKongregate = false;
        internal static int userID = 0;
        internal static string username = "Guest";
        internal static string gameAuthToken = "";
        
        internal static bool downloadBegan = false;
        internal static int texturesToDownload = 0;
        internal static int texturesDownloaded = 0;


        //public static bool downloadsCompleted = false;
        //public static int downloadsCount;

        void Start() {
            if (m_instance != null) {
                Debug.LogError("Only one instance of KongAPI is allowed.");
                Destroy(this.gameObject);
            }
                
            m_instance = this;
            //SIGN-IN CODE APPLY LATER OR BE DOOMED TO HATERS
            /*Application.ExternalEval(
                "kongregate.services.addEventListener('login', function(){" +
                "   var services = kongregate.services;" +
                "   var data=[services.getUserId(), services.getUsername(), services.getGameAuthToken()].join('|');" +
                "   kongregateUnitySupport.getUnityObject().SendMessage('KongregateAPI', 'OnKongregateAPILoaded');" +
                "});"
             );*/

            connectToKong();
        }

        /// <summary>
        /// Handles the loaded API, and extracts it
        /// </summary>
        /// <param name="userInfoString">contains API data</param>
        void OnKongregateAPILoaded(string userInfoString) {
            // We now know we're on Kongregate
            isKongregate = true;

            // Split the user info up into tokens
            string[] api = userInfoString.Split("|"[0]);
            userID = int.Parse(api[0]);
            KongAPI.username = api[1];
            gameAuthToken = api[2];
        }


        #region Accessors

        internal static bool isGuest {
            get { return username == "Guest"; }
        }


        #endregion

        internal static void connectToKong() {
            Application.ExternalEval(
                "if(typeof(kongregateUnitySupport) != 'undefined'){" +
                " kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');" +
                "};"
            );
        }

        internal static void Submit(string statName, long value) {
            connectToKong();
            Application.ExternalCall("kongregate.stats.submit", statName, value);
        }

    }//end of class
}