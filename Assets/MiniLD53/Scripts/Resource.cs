namespace MiniLD53 {
    using System.Collections.Generic;
    using Rect = UnityEngine.Rect;
    using GUIContent = UnityEngine.GUIContent;
    using SM = System.Math;

    [System.Serializable]
    public class Resource {

        private double clickValue;
        private long total = 0;

        internal Resource dependency;
        internal string dependencyName;
        internal List<ResourceBooster> boosters = new List<ResourceBooster>();

        public long Victory { get; private set; }
        internal string Symbol { get; private set; }
        internal string Description { get; private set; }
        internal string Name { get; private set; }
        internal double VPS { get; private set; }
        internal double DPS { get; private set; }
        private double accumulated { get; set; }
        internal long Total { get; private set; }

        internal double ClickValue {
            get { return clickValue; }
            private set {
                if (value < 1)
                    clickValue = 1;
                else
                    clickValue = value;
            }
        }

        public Resource(string name, string dependency, string description,
            long victory, string symbol) {

            if (boosters == null)
                boosters = new List<ResourceBooster>();
            Name = name;
            dependencyName = dependency;
            VPS = 0;
            clickValue = 1;
            Symbol = symbol;
            Description = description;
            Victory = victory;
            Total = 1;
        }


        internal void update(float deltaTime) {
            add((VPS - DPS) * deltaTime);
        }

        private void add(double d) {
            long value = 0;

            if (d > 0)
                value = (long)SM.Floor(d);
            else
                value = (long)SM.Ceiling(d);
            accumulated += (d - value);

            if (accumulated >= 1) {
                ++value;
                --accumulated;
            } else if (accumulated <= -1) {
                --value;
                ++accumulated;
            }
            Total += value;
        }

        internal void Click() {
            add(clickValue);
        }

        public override string ToString() {
            return Name + " VPS: " + VPS + " DPS: " + DPS + " Total: " + Total;
        }

        public GUIContent ToGUIButton() {
            double d = (VPS - DPS);
            if (d > 0)
                return new GUIContent(
                    "<b>" + Total + "</b>\n" + Name + " (" + Symbol + ")\n<color=green>" + d.ToString("F2") + "</color>",
                    "<size=22>Click to generate " + Description + "</size>\n<size=20><color=green>+" + VPS + " " + Symbol +
                "</color>           <color=red>-" + DPS + " " + dependency.Symbol + "</color></size>"
                    );
            else
                return new GUIContent(
                    "<size=20><b>" + Total + "</b></size>\n" + Name + " (" + Symbol + ")\n<color=red>" + d.ToString("F2") + "</color>",
                    "<size=22>Click to generate " + Description + "</size>\n<size=20><color=green>+" + VPS + " " + Symbol +
                "</color>           <color=red>-" + DPS + " " + dependency.Symbol + "</color></size>"
                    );
        }

        public string ToGUI() {
            return Description + ": " + Total;
        }

        internal void buy(ResourceBooster b) {
            if (boosters.Contains(b)) {
                if (dependency.Total < b.NextCost)
                    return;
                dependency.Total -= b.NextCost;
                b.buy();
                VPS = 0;
                double dps = 0;
                foreach (ResourceBooster i in boosters) {
                    VPS += i.VPS;
                    dps += i.DPS;
                }
                clickValue = VPS * 0.1 + 1;
                dependency.DPS = dps;
            }
        }


        internal string ToVictoryGUI() {
            return Description + ": " + ((Total > Victory) ? 0 : (Victory - Total)) + "\n";
        }

        internal void reset() {
            Total = 1;
            VPS = 0;
            clickValue = 1;
            DPS = 0;
            for (int i = 0; i < boosters.Count; i++)
                boosters[i].reset();
        }
    } //class
} //namespace