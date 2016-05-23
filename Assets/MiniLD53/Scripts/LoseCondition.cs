namespace MiniLD53 {
	using SimpleJSON;
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

		public LoseCondition() : base() {
			/*Empty*/
		}

		public LoseCondition(JSONNode item) : this() {
			this.name = item["name"].Value;
			this.dropRate = item["dropRate"].AsFloat;
			this.currentRate = 100.0f;
			this.mustHaveText = item["mustHave"].Value;
			this.mustHave = Engine.instance.findResource(this.mustHaveText);
			if (item["raising"] != null) {
				this.raisingText = item["raising"].Value;
				this.raising = Engine.instance.findResource(this.raisingText);
			}
		}


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


