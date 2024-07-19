using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class ConfigManager : MonoBehaviour {
		internal static ConfigManager Instance { get; private set; }

		private static List<ZUIConfig> currentConfigs = new List<ZUIConfig>(); // all 'ZUIConfig' configs
		private static List<ZUIConfig> enabledConfigs = new List<ZUIConfig>(); // enabled configs specified in 'ZUIConfigOptions'
		private static List<ConfigNode> currentConfigNodes = new List<ConfigNode>(); // resulting config nodes from enabledConfigs

		private const string USER_OVERRIDE_SAVE_LOCATION = Constants.MOD_FOLDER + "Config/override.cfg"; // user overrides set in ui

		private static int overridePriority = 16384;

		internal static bool enableAdaptiveNavball = true;

		public void Awake() {
			if (Instance == null || Instance == this) {
				Instance = this;
				DontDestroyOnLoad(gameObject);
			} else {
				Destroy(gameObject);
				return;
			}

			LoadConfigs();
			SetConfigs();
		}
		private void LoadConfigs() {
			UrlDir.UrlConfig[] ZUINodes = GameDatabase.Instance.GetConfigs(Constants.ZUI_NODE);
			List<ConfigNode> configOptions = new List<ConfigNode>();
			foreach (UrlDir.UrlConfig URLConfig in ZUINodes) {
				// load all configs
				Debug.Log($"[ZUI] Loading configs from {URLConfig.url}");
				ConfigNode[] ZUIConfigs = URLConfig.config.GetNodes(Constants.ZUICONFIG_NODE);
				foreach (ConfigNode ZUIURLConfig in ZUIConfigs) {
					if (!ZUIURLConfig.HasValue(Constants.ZUICONFIGNAME_VALUE)) {
						Debug.Log("[ZUI] Config does not have a name!");
						continue;
					}

					ZUIConfig config = new ZUIConfig(ZUIURLConfig);
					if (currentConfigs.Exists(c => c.name == config.name)) {
						Debug.Log($"[ZUI] Discarding duplicate of the config '{config.name}'. Please make sure that all configs have unique names.");
						continue;
					}
					currentConfigs.Add(config);
				}
				ConfigNode[] configNodes = URLConfig.config.GetNodes(Constants.ZUICONFIGOPTIONS_NODE);
				if (configNodes.Count() != 0) configOptions.AddRange(configNodes);
			}

			bool alreadyHasOptionsNode = false;
			configOptions = configOptions.OrderByDescending(c => int.Parse(c.GetValue(Constants.ZUICONFIGOPTION_PRIORITY_CFG))).ToList();
			// load config options (whether or not a config is enabled)
			// this is a separate loop so that all the configs are loaded before we apply the options
			foreach (ConfigNode configOption in configOptions) {
				if (alreadyHasOptionsNode) break;
				alreadyHasOptionsNode = true;
				Debug.Log($"[ZUI] priority: {configOption.GetValue(Constants.ZUICONFIGOPTION_PRIORITY_CFG)}");
				if (!configOption.HasValue(Constants.ZUICONFIGOPTION_ENABLED_CFG)) {
					Debug.Log($"[ZUI] Config option does not have '{Constants.ZUICONFIGOPTION_ENABLED_CFG}'. There is nothing to enable.");
					continue;
				}
				if (configOption.HasValue(Constants.ADAPTIVE_NAVBALL_ENABLED_CFG)) {
					enableAdaptiveNavball = bool.Parse(configOption.GetValue(Constants.ADAPTIVE_NAVBALL_ENABLED_CFG));
				}
				string[] ZUIConfigOptionValues = configOption.GetValue(Constants.ZUICONFIGOPTION_ENABLED_CFG).Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string configValue in ZUIConfigOptionValues) {
					if (currentConfigs.Exists(c => c.name == configValue)) {
						EnableConfig(currentConfigs.Find(c => c.name == configValue));
					} else {
						Debug.Log($"[ZUI] '{configValue}' does not exist.");
					}
				}
			}
		}
		internal static void EnableConfig(ZUIConfig config) {
			if (!enabledConfigs.Exists(c => c.name == config.name)) {
				enabledConfigs.Add(config);
			}
		}
		internal static bool DisableConfig(ZUIConfig config) {
			return enabledConfigs.Remove(config);
		}
		private static void AddConfig(ZUIConfig config) {
			if (config.HasHUDReplacerNode) {
				ConfigNode[] ConfigNodes = config.GetConfigNodesAsHUDReplacerNodes();
				foreach (var configNode in ConfigNodes) {
					currentConfigNodes.Add(configNode);
				}
			}
			if (config.HasRecolorNode) {
				ConfigNode[] recolorConfigNodes = config.GetRecolorConfigNodesAsHUDReplacerRecolorNodes();
				foreach (var configNode in recolorConfigNodes) {
					currentConfigNodes.Add(configNode);
				}
			}
		}
		internal static void SetConfigs() {
			currentConfigNodes.Clear();
			HUDReplacer.HUDReplacer.additionalConfigNodes.Clear();
			HUDReplacer.HUDReplacer.additionalRecolorNodes.Clear();
			foreach (ZUIConfig config in enabledConfigs) { 
				AddConfig(config);
			}
			foreach (ConfigNode configNode in currentConfigNodes) {
				if (configNode.name == Constants.HUDREPLACER_NODE) {
					HUDReplacer.HUDReplacer.additionalConfigNodes.Add(configNode);
				} else if (configNode.name == Constants.HUDREPLACER_RECOLOR_NODE) {
					HUDReplacer.HUDReplacer.additionalRecolorNodes.Add(configNode);
				}
			}
			if (HUDReplacer.HUDReplacer.Instance != null) {
				HUDReplacer.HUDReplacer.Instance.GetTextures();
				HUDReplacer.HUDReplacer.Instance.ReplaceTextures();
			}
		}
		internal static void SaveConfigOverrides() {
			ConfigNode overridesFile = new ConfigNode();
			ConfigNode ZUINode = new ConfigNode(Constants.ZUI_NODE);
			ConfigNode ZUIConfigOptionsNode = new ConfigNode(Constants.ZUICONFIGOPTIONS_NODE);
			List<string> enabledConfigs = new List<string>();
			foreach (ZUIConfig config in ConfigManager.enabledConfigs) {
				enabledConfigs.Add(config.name);
			}
			ZUIConfigOptionsNode.AddValue(Constants.ZUICONFIGOPTION_ENABLED_CFG, string.Join(", ", enabledConfigs));
			ZUIConfigOptionsNode.AddValue(Constants.ZUICONFIGOPTION_PRIORITY_CFG, overridePriority);
			ZUIConfigOptionsNode.AddValue(Constants.ADAPTIVE_NAVBALL_ENABLED_CFG, enableAdaptiveNavball);
			ZUINode.AddNode(ZUIConfigOptionsNode);
			overridesFile.AddNode(ZUINode);
			overridesFile.Save(KSPUtil.ApplicationRootPath + USER_OVERRIDE_SAVE_LOCATION, "Config overrides set in-game. Delete this file to remove overrides.");
		}
		internal static List<ZUIConfig> GetConfigs() {
			return currentConfigs;
		}
		internal static List<ZUIConfig> GetEnabledConfigs() {
			return enabledConfigs;
		}
	}
}