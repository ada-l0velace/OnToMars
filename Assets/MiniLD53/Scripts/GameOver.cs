namespace MiniLD53 {
	using UnityEngine;
	using System.Collections;

	public class GameOver : MonoBehaviour {

        internal static GameOver instance;
        public string reason;
        public GUISkin skin;

		void Awake() {
            instance = this;
            enabled = false;
		}

        void OnGUI() {
            GUI.skin = skin;
            GUI.Box(new Rect(0,0,Screen.width, Screen.height), reason, skin.GetStyle("gameOver"));
            if (GUI.Button(new Rect(360, 500, 80, 40), "Restart")) {
                Engine.instance.restart();
                enabled = false;
            }
        }

        internal static void gameOver(string reason) {
            Engine.instance.enabled = false;
            Engine.report.enabled = false;
            instance.reason = reason;
            instance.enabled = true;
        }

	} //class
} //namespace