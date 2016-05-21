namespace MiniLD53 {
    using UnityEngine;
    using System.Collections;

    [System.Serializable]
    public class ResourceBooster {
        private double baseVPS;
        private double baseDPS;
        private uint count;
        internal string dependency = "";
        private string booster = "";
        internal string dependencySymbol;
        internal string ownerSymbol;
        private long startingCost;


        public ResourceBooster(string name, double vps, double dps, long cost,
            string booster, string description,  string ownerSymbol) {
            Name = name;
            baseVPS = vps;
            baseDPS = dps;
            startingCost = cost;
            NextCost = cost;
            count = 0;
            this.booster = booster;
            Description = description;
            this.ownerSymbol = ownerSymbol;
            //setGUIContent();
        }

        public string Description { get; private set; }
        //public GUIContent guiContent { get; private set; }
        internal string Name { get; private set; }
        internal double DPS { get; private set; }
        internal double VPS { get; private set; }
        internal long NextCost { get; set; }

        internal uint Bought {
            get { return count; }
        }

        internal void buy() {
            try {
                NextCost += (long)(NextCost * 0.15);
            } catch (System.OverflowException) {
                NextCost = long.MaxValue;
            }
            ++count;
            VPS = count * baseVPS;
            DPS = count * baseDPS;
            setGUIContent();
        }

        public GUIContent setGUIContent() {
            return new GUIContent(
                Name,
                //Name + " Owned: " + count + " Cost: " + NextCost + " " + dependency,
                "<size=24>" + Description + "</size>\n<size=20><color=green>+" + baseVPS + " " + ownerSymbol +
                "</color>           <color=red>-" + baseDPS + " " + dependencySymbol + "</color></size>"
                );
        }

        internal void reset() {
            DPS = 0;
            VPS = 0;
            NextCost = startingCost;
            count = 0;
            setGUIContent();
        }

    } //class
} //namespace