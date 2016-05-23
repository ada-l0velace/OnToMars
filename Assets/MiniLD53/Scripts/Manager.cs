using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace MiniLD53 {
	public class Manager {
		private static Manager _instance;

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

		public Manager () {
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
		public static Manager getInstance {
			get {
				if (_instance == null) {
					_instance = new Manager();
				}
				return _instance;
			}
		}
		Rect createRect (JSONNode jSONNode) {
			throw new NotImplementedException ();
		}

		void loadLoseConditions (JSONNode jSONNode) {
			throw new NotImplementedException ();
		}

		void loadArrows (JSONNode jSONNode) {
			throw new NotImplementedException ();
		}

		void loadNews (JSONNode jSONNode) {
			throw new NotImplementedException ();
		}
	}
}

