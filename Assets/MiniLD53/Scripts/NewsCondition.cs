namespace MiniLD53 {
    using System.Collections;
	using SimpleJSON;

    public class NewsCondition {

        internal string resourceName;
        internal string boosterName;
        internal string text;
        internal int status = 0;
        internal ConditionType type;


		public NewsCondition() : base() {
			/*Empty*/
		}

		public NewsCondition(JSONNode item) : this() {
			switch (item["type"]) {
				case "hasBooster":
					type = ConditionType.hasBooster;
					resourceName = item["resource"];
					boosterName = item["booster"];
					text = item["text"];
					break;
			}
		}
        internal void update() {
            switch (type) {
                case ConditionType.hasBooster:
                    if (Engine.instance.resources.Find(x => x.Name == resourceName).boosters.Find(x => x.Name == boosterName).Bought > 0) {
                        status = 1;
                    } else
                        status = 0;
                    break;
            }
        }

    } //class

    enum ConditionType {
        hasBooster
    }


} //namespace

