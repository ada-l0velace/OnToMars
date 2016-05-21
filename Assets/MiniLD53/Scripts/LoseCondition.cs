namespace MiniLD53 {
    using GUIContent = UnityEngine.GUIContent;

    [System.Serializable]
    public class LoseCondition {
        internal string name;
        internal Resource mustHave;
        internal string mustHaveText;
        internal string raisingText;
        internal Resource raising;
        internal float dropRate;
        internal float currentRate;

        internal string getDeathReason() {
            if (raising != null) {
                return "Too much " + raisingText + " for so little " + mustHaveText + ".";
            } else {
                return "Didn't have enough " + mustHaveText + ".";
            }
        }

        internal GUIContent ToGUI() {
            return new GUIContent(
                name + " " + currentRate.ToString("F0"),
                (raising != null)?
                    " Needs " + mustHaveText + " while " + raisingText + " above 0. " :
                    "Requires " + mustHaveText + " above 0."
                );
        }

        internal void reset() {
            currentRate = 100;
        }
    } //class
} //namespace