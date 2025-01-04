using UnityEngine;
using KSP.UI.Screens;
using System;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
	public class ToolbarButton : MonoBehaviour {
		private Texture configTexture;
		private ApplicationLauncherButton toolbarButton;

		private const string CONFIG_TEXTURE_PATH = Constants.ZUI_FOLDER + "Assets/zui_config"; // icon

		public void Awake() {
			if (!ToolbarButtonEnabled()) return;
			configTexture = GameDatabase.Instance.GetTexture(CONFIG_TEXTURE_PATH, false);
			toolbarButton = ApplicationLauncher.Instance.AddModApplication(ConfigUI.TogglePopup, ConfigUI.TogglePopup, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, configTexture);
		}
		private bool ToolbarButtonEnabled() {
			bool enabled = false;
			UrlDir.UrlConfig[] ZUINodes = GameDatabase.Instance.GetConfigs(Constants.ZUI_NODE);
			foreach (UrlDir.UrlConfig URLConfig in ZUINodes) {
				ConfigNode[] ZUIConfigOptions = URLConfig.config.GetNodes(Constants.ZUISETTINGS_NODE);
				foreach (ConfigNode ZUIConfigOption in ZUIConfigOptions) {
					ZUIConfigOption.TryGetValue(Constants.TOOLBAR_BUTTON_ENABLED, ref enabled);
				}
			}
			return enabled;
		}
	}
}