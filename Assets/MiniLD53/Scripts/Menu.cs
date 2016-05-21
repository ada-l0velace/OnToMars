namespace MiniLD53 {
    using UnityEngine;
    using System.Collections;

    public class Menu : MonoBehaviour {

        public Rect continueBTN;
        public Rect newBTN;
        public Rect title;
        private GUIStyle buttonStyle;
        private GUIStyle titleStyle;

        void OnGUI() {
            if (titleStyle == null || buttonStyle == null) {
                GUISkin s = Engine.instance.skin;
                titleStyle = s.GetStyle("Title");
                buttonStyle = s.GetStyle("MenuBtn");
            }

            GUI.Label(title, "Onto Mars!", Engine.instance.skin.GetStyle("Title"));
            if (PPSerialization.HasSave("gametag") && GUI.Button(continueBTN, "Continue", buttonStyle)) {
            //if (GUI.Button(continueBTN, "Continue", buttonStyle)) {
                Engine.instance.loadOld();
                Destroy(this);
            }
            if (GUI.Button(newBTN, "New Game", buttonStyle)) {
                Engine.instance.newGame();
                Destroy(this);
            }
        }

    } //class
} //namespace