using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class AdaptiveNavball : MonoBehaviour {
		internal static AdaptiveNavball Instance { get; private set; }
		private string[] navballPaths = new string[] { null, null, null };
		private bool[] navballExists = new bool[3];
		private Texture2D navballTexture;
		private string originalNavballTextureLocation = Constants.MOD_FOLDER + "PluginData/navball_original.png";

		private bool adaptiveBehaviorEnabled = false;

		public void Start() {
			if (Instance == null || Instance == this) {
				Instance = this;
			} else {
				Destroy(gameObject);
				return;
			}
			LoadConfigs();
			adaptiveBehaviorEnabled = true;
			foreach (Texture2D tex in (Texture2D[])(object)Resources.FindObjectsOfTypeAll(typeof(Texture2D))) {
				if (tex.name == Constants.NAVBALL_TEXTURE) {
					navballTexture = tex;
					Debug.Log("[ZUI] Found NavBall texture!");
					break;
				}
			}
			Debug.Log("[ZUI] NavBall Texture Paths: Surface: " + (navballPaths[0] ?? "None") + " | Orbit: " + (navballPaths[1] ?? "None") + " | Target:" + (navballPaths[2] ?? "None"));
			if (!ConfigManager.options[Constants.ADAPTIVE_NAVBALL_ENABLED_CFG]) return;
			ChangeNavball(new FlightGlobals.SpeedDisplayModes());
			GameEvents.onSetSpeedMode.Add(ChangeNavball);
		}
		private void LoadConfigs() {
			UrlDir.UrlConfig[] ZUINodes = GameDatabase.Instance.GetConfigs(Constants.ZUI_NODE);
			List<ConfigNode> allNavballConfigs = new List<ConfigNode>();
			foreach (UrlDir.UrlConfig node in ZUINodes) {
				if (!node.config.HasNode(Constants.ZUINAVBALL_NODE)) continue;
				allNavballConfigs.AddRange(node.config.GetNodes(Constants.ZUINAVBALL_NODE));
			}
			allNavballConfigs = allNavballConfigs.OrderBy(c => int.Parse(c.GetValue(Constants.ADAPTIVE_NAVBALL_PRIORITY_CFG))).ToList();
			foreach (ConfigNode config in allNavballConfigs) {
				if (config.HasValue(Constants.NAVBALL_SURFACE)) {
					navballPaths[0] = config.GetValue(Constants.NAVBALL_SURFACE);
					navballExists[0] = true;
				}
				if (config.HasValue(Constants.NAVBALL_ORBIT)) {
					navballPaths[1] = config.GetValue(Constants.NAVBALL_ORBIT);
					navballExists[1] = true;
				}
				if (config.HasValue(Constants.NAVBALL_TARGET)) {
					navballPaths[2] = config.GetValue(Constants.NAVBALL_TARGET);
					navballExists[2] = true;
				}
			}
		}

		internal void ChangeNavball(FlightGlobals.SpeedDisplayModes speedMode) {
			Debug.Log("[ZUI] Switching Navball mode to " + FlightGlobals.speedDisplayMode);
			switch (FlightGlobals.speedDisplayMode) {
				case FlightGlobals.SpeedDisplayModes.Surface:
					if (navballExists[0]) {
						ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[0]));
					}
					break;
				case FlightGlobals.SpeedDisplayModes.Orbit:
					if (navballExists[1]) {
						ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[1]));
					}
					break;
				case FlightGlobals.SpeedDisplayModes.Target:
					if (navballExists[2]) {
						ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballPaths[2]));
					}
					break;
			}
		}

		public void OnDisable() {
			DisableAdaptiveNavball();
		}

		public void DisableAdaptiveNavball() {
			if (!adaptiveBehaviorEnabled) return;
			adaptiveBehaviorEnabled = false;
			try { GameEvents.onSetSpeedMode.Remove(ChangeNavball); } catch { Debug.Log("[ZUI] Unable to unsubscribe to 'ChangeNavball'"); }
			ImageConversion.LoadImage(navballTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + originalNavballTextureLocation));
		}
		public void EnableAdaptiveNavball() {
			if (adaptiveBehaviorEnabled) return;
			adaptiveBehaviorEnabled = true;
			ChangeNavball(new FlightGlobals.SpeedDisplayModes());
			GameEvents.onSetSpeedMode.Add(ChangeNavball);
		}
	}
}
