namespace MiniLD53 {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class NewsReport : MonoBehaviour {

        internal GUIStyle guiStyle;
        internal List<NewsCondition> reports = new List<NewsCondition>();
        internal List<NewsCondition> readyReports = new List<NewsCondition>();
        internal NewsCondition reporting;
        internal float timer;
        internal float maxTime = 7.5f;

        void Start() {
            guiStyle = Engine.instance.skin.GetStyle("newsReport");
            setReport();
            enabled = false;
        }

        internal Rect drawArea = new Rect(150, 10, 500, 55);
        void OnGUI() {
            timer += Time.deltaTime;
            if (timer > maxTime) {
                timer = 0f;
                setReport();
            }
            GUI.Box(drawArea, (reporting != null) ? reporting.text : "Scientists prepare for Mars.", guiStyle);
        }

        private void setReport() {
            int c = 0;
            for (int i = 0; i < reports.Count; i++) {
                c = reports[i].status;
                reports[i].update();
                if (c != reports[i].status) {
                    if (reports[i].status == 1)
                        readyReports.Add(reports[i]);
                    else
                        readyReports.Remove(reports[i]);
                }
            }
            if (readyReports.Count > 0)
                reporting = readyReports[Random.Range(0, readyReports.Count - 1)];
            else
                reporting = null;
        }



        internal void reset() {
            for (int i = 0; i < readyReports.Count; i++) {
                readyReports[i].status = 0;
            }
            readyReports.Clear();
            reporting = null;
        }
    } //class
}//namespace