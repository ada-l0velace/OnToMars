namespace MiniLD53 {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using SimpleJSON;

    public class Engine : MonoBehaviour {

        internal static Engine instance;
        internal static float CLICK_BUTTON_HEIGHT = 60.0f;
        internal static float CLICK_BUTTON_WIDTH = 100.0f;
        internal static NewsReport report;
        internal GameData GameData = new GameData();

		internal List<Resource> resources = new List<Resource>();
        
		internal Resource lastHovered;
        internal Rect upgradesDrawArea;
        internal List<Rect> arrows = new List<Rect>();
        public List<Texture2D> arrowsGfx;

        internal List<LoseCondition> loseConditions = new List<LoseCondition>();

        public GUISkin skin;
        public Texture2D happyFace;
        public Texture2D unhappyFace;
        public Texture2D sadFace;
        public Rect victoryDrawArea;

        internal float timeSpent = 0f;
        internal float saveTimer = 0f;

        internal long clickCount = 0;
		internal int konamiCode = 0;
        internal List<KeyCode> konamiCodeKey = new List<KeyCode>() { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.A, KeyCode.B };
        
        internal Dictionary<string, Rect> resourceRects = new Dictionary<string,Rect>();

        #region loading shenenigans
        void Awake() {
            instance = this;
            enabled = false;
            GameData.resources = resources;

            JSONNode data = JSON.Parse(Resources.Load("data").ToString());
            JSONNode d = data["upgradesDA"];
            upgradesDrawArea = new Rect(d[0].AsFloat, d[1].AsFloat, d[2].AsFloat, d[3].AsFloat);


            d = data["resources"];
            JSONNode h, b, hh;

            Resource r;
            for (int i = 0; i < d.Count; i++) {
                h = d[i];
                r = new Resource(
                    h["name"], h["link"].Value, h["description"],
                    long.Parse(h["victory"].Value), h["symbol"].Value
                );
                resourceRects.Add(r.Name, createRect(h["button"]));
                b = h["boosters"];
                for (int j = 0; j < b.Count; j++) {
                    hh = b[j];
                    r.boosters.Add(
                        new ResourceBooster(
                            hh["name"], hh["vps"].AsDouble, hh["dps"].AsDouble,
                            long.Parse(hh["cost"].Value), h["name"],
                            hh["description"], r.Symbol
                            )
                        );
                }
                resources.Add(r);
            }

            string n;
            Resource rr;
            for (int i = 0; i < d.Count; i++) {
                r = resources[i];
                n = r.dependencyName;
                rr = resources.Find(x => x.Name == n);
                if (rr == null)
                    Debug.LogError("Resource [" + r.Name + "] dependency not found: " + n);
                r.dependency = rr;
                for (int j = 0; j < r.boosters.Count; j++) {
                    r.boosters[j].dependency = rr.Description;
                    r.boosters[j].dependencySymbol = rr.Symbol;
                    r.boosters[j].setGUIContent();
                }
            }
            loadLoseConditions(data["loseConditions"]);
            loadArrows(data["arrows"]);
            loadNews(data["news"]);

            GameData.resources = resources;
            GameData.loseConditions = loseConditions;
        }

		public void loadNews(JSONNode h) {
            report = GetComponent<NewsReport>();
            NewsCondition cond;
            foreach (JSONNode item in h.Childs) {
                cond = new NewsCondition(item);
                report.reports.Add(cond);
            }
        }

		public void loadArrows(JSONNode h) {
            foreach (JSONNode item in h.Childs) {
                arrows.Add(new Rect(
                    item[0].AsFloat, item[1].AsFloat,
                    item[2].AsFloat, item[3].AsFloat
                ));
            }
        }

		public void loadLoseConditions(JSONNode h) {
            LoseCondition lc;
            foreach (JSONNode item in h.Childs) {
				lc = new LoseCondition(item);
                loseConditions.Add(lc);
            }
        }

		public Resource findResource(string name) {
            return resources.Find(x => x.Name == name || x.Description == name);
        }

		public Rect createRect(JSONNode data) {
            return new Rect(data[0].AsFloat, data[1].AsFloat,
                    CLICK_BUTTON_WIDTH, CLICK_BUTTON_HEIGHT);
        }
        #endregion
        
        
        void Update() {
            float time = Time.deltaTime;
            timeSpent += time;
            saveTimer += time;
            for (int i = resources.Count - 1; i >= 0; --i)
                resources[i].update(time);
            updateLoseCondition();
            updateWinCondition();
            if (saveTimer >= 2f) {
                saveTimer = 0f;
                kongregate.KongAPI.Submit("TimesClicked", clickCount);
                clickCount = 0;
                PlayerPrefs.SetFloat("timeplayed", timeSpent);
                PPSerialization.Save("gametag", GameData);
            }
        }


        void OnGUI() {
            GUI.skin = skin;
            Vector2 mousePos = Event.current.mousePosition;

            for (int i = 0; i < arrows.Count; i++) 
                GUI.DrawTexture(arrows[i], arrowsGfx[i]);


            string s = "";
            Resource res;
            List<ResourceBooster> bos;
            ResourceBooster b;
            Rect rect;
            for (int i = resources.Count - 1; i >= 0; --i) {
                res = resources[i];
                rect = resourceRects[res.Name];
                s += res.ToGUI() + "\n";
                if (rect.Contains(mousePos))
                    lastHovered = res;
                if (GUI.Button(rect, res.ToGUIButton(), skin.GetStyle("ButtonClicker"))) {
                    res.Click();
                }

                if (lastHovered == res) {
                    bos = res.boosters;
                    GUI.Box(upgradesDrawArea, "");
                    GUI.BeginGroup(upgradesDrawArea);
                    GUI.Label(new Rect(0,0,upgradesDrawArea.width, 30), res.Description, skin.GetStyle("UpsTitle"));
                    for (int j = 0; j < bos.Count; j++) {
                        b = bos[j];
                        Rect r = new Rect(3, 33 + j * 28, upgradesDrawArea.width - 6, 25);
                        if (GUI.Button(r, b.setGUIContent(), skin.button)) {
                            res.buy(b);
                        }
                        GUI.Label(r, b.NextCost + b.dependencySymbol, skin.GetStyle("UpBtnMoney"));
                    }
                    GUI.EndGroup();
                }
            }

            winConditionGUI();
            loseConditionGUI();

            if (!string.IsNullOrEmpty(GUI.tooltip))
                GUI.Box(tooltipRect, GUI.tooltip, skin.box);
        }
        internal Rect tooltipRect = new Rect(100, 460, 600, 110);


        #region Lose Conditions
		public void updateLoseCondition() {
            Resource r;
            foreach (LoseCondition item in loseConditions) {
                //test for raising values
                r = item.raising;
                if (r != null) {
                    if (r.VPS > r.DPS) {
                        r = item.mustHave;
                        if (r.Total <= 0) {
                            item.currentRate -= item.dropRate * Time.deltaTime;
                            if (item.currentRate <= 0f) {
                                GameOver.gameOver(item.getDeathReason());
                                break;
                            }
                        }
                        continue;
                    }
                }

                //test for 100% must have
                r = item.mustHave;
                if (r.Total <= 0) {
                    item.currentRate -= item.dropRate * Time.deltaTime;
                    if (item.currentRate <= 0f) {
                        GameOver.gameOver(item.getDeathReason());
                        break;
                    }
                    continue;
                }
            }
        }

		public void loseConditionGUI() {
            GUI.Box(new Rect(655, 69, 141, loseConditions.Count * 36), "");
            Texture2D h;
            for (int i = 0; i < loseConditions.Count; i++) {
                if (loseConditions[i].currentRate >= 75f)
                    h = happyFace;
                else if (loseConditions[i].currentRate >= 25f)
                    h = unhappyFace;
                else
                    h = sadFace;
                GUI.DrawTexture(new Rect(658, i * 32 + 71, 32, 32), h);
                GUI.Label(new Rect(700, i * 32 + 72, 200, 30), loseConditions[i].ToGUI());
            }
        }
        #endregion

        #region Win Conditions
		public void updateWinCondition() {
            foreach (Resource item in resources) 
                if (item.Total < item.Victory)
                    return;
            report.enabled = false;
            kongregate.KongAPI.Submit("TimeToVictory", (long)timeSpent);
            GameOver.gameOver("You have enough resources, knowledge and money to head on to Mars.\nWhat are you waiting for?\nGo!");
        }

		public void winConditionGUI() {
            string s = "Resources left to head onto Mars\n";
            for (int i = 0; i < resources.Count; i++) 
                s += resources[i].ToVictoryGUI();
            GUI.Box(victoryDrawArea, s);
        }
        #endregion

        internal void restart() {
            enabled = true;
            report.enabled = true;
            timeSpent = 0f;
            report.reset();
            for (int i = 0; i < resources.Count; i++)
                resources[i].reset();
            for (int i = 0; i < loseConditions.Count; i++)
                loseConditions[i].reset();
        }

        internal void newGame() {
            restart();
        }

        internal void loadOld() {
            setLoadedData((GameData)PPSerialization.Load("gametag"));
            enabled = true;
            report.enabled = true;
            report.reset();
        }



        internal void setLoadedData(GameData gameData) {
            GameData = gameData;
            resources = gameData.resources;
            loseConditions = gameData.loseConditions;
            timeSpent = PlayerPrefs.GetFloat("timeplayed");
        }
    } //class
} //namespace