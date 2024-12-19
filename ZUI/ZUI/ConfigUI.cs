using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

namespace ZUI {
	// TODO: l10n
	internal static class ConfigUI {
		internal static PopupDialog popupDialog;
		internal static PopupDialog applyNotice = null;
		internal static DialogGUIBase configPage;
		internal static DialogGUIBase settingsPage;

		internal enum ZUITab {
			Configuration,
			OtherSettings
		}

		internal static ZUITab currentTab = ZUITab.OtherSettings;

		private static float windowWidth = 250;
		private static float windowHeight = 250;
		private static float buttonHeight = 24;

		private static float paddingBase = 3;
		private static float paddingXSmall = 0.5f * paddingBase;
		private static float paddingSmall = 1 * paddingBase;
		private static float paddingRegular = 2 * paddingBase;
		private static float paddingLarge = 3 * paddingBase;
		private static float paddingWindow = 4 * paddingBase;

		private static List<ZUIConfig> changedConfigs = new List<ZUIConfig>();

		internal static void TogglePopup() {
			if (popupDialog == null) {
				ShowPopup();
			} else {
				popupDialog.Dismiss();
				popupDialog = null;
			}
		}

		// some gui stuff from https://github.com/neuoy/KSPTrajectories/blob/master/src/Plugin/Display/MainGUI.cs
		internal static void ShowPopup() {
			// create dialog to attach elements to
			List<DialogGUIBase> dialog = new List<DialogGUIBase>();

			// Config Page
			// add zuiconfigs
			List<DialogGUIBase> buttons = new List<DialogGUIBase>();
			List<ZUIConfig> configs = ConfigManager.GetConfigs();
			buttons.Add(new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true));

			if (Constants.ENABLE_HUDREPLACER_FUNCTIONALITY) {
				foreach (ZUIConfig config in configs) {
					Debug.Log("[ZUI] " + config.name + ": " + ConfigManager.GetEnabledConfigs().Contains(config));
					DialogGUIToggleButton button = new DialogGUIToggleButton(ConfigManager.GetEnabledConfigs().Contains(config),
						config.localizedName,
						delegate (bool selected) {
							ToggleConfig(selected, config);
						},
						windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), buttonHeight);
					buttons.Add(button);
				}
			}

			// attach configs to list
			DialogGUIScrollList configsScrollList = new DialogGUIScrollList(Vector2.one,
				false,
				true,
				new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), 64,
				paddingXSmall,
				new RectOffset((int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall),
				TextAnchor.MiddleLeft,
				buttons.ToArray()));

			// apply button
			DialogGUIButton applyButton = new DialogGUIButton(Localizer.Format("#autoLOC_ZUI_ConfigUI_Apply"),
				ApplyConfigWindow,
				windowWidth - (2 * paddingRegular), buttonHeight, false);

			configPage = new DialogGUIVerticalLayout(true, false, paddingSmall, new RectOffset(), TextAnchor.UpperCenter, configsScrollList, applyButton);

			// Other Settings Page
			// adaptive navball
			List<DialogGUIBase> settingsSections = new List<DialogGUIBase> {
				new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true),
				CreateSettingsHeader(Localizer.Format("#autoLOC_ZUI_ConfigUI_AdaptiveNavball_header")),
			};
			DialogGUIToggle adaptiveNavballToggle = new DialogGUIToggle(() => ConfigManager.options[Constants.ADAPTIVE_NAVBALL_ENABLED_CFG], Localizer.Format("#autoLOC_ZUI_ConfigUI_EnableAdaptiveNavball"), ToggleAdaptiveNavball);
			adaptiveNavballToggle.tooltipText = Localizer.Format("#autoLOC_ZUI_ConfigUI_EnableAdaptiveNavball_tooltip");
			settingsSections.Add(adaptiveNavballToggle);

			settingsSections.Add(CreateSettingsHeader(Localizer.Format("#autoLOC_ZUI_ConfigUI_GaugeThumbs_header")));
			DialogGUIToggle throttleThumbToggle = new DialogGUIToggle(() => ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG], Localizer.Format("#autoLOC_ZUI_ConfigUI_EnableThrottleThumb"), ToggleThrottleThumb);
			throttleThumbToggle.tooltipText = Localizer.Format("#autoLOC_ZUI_ConfigUI_EnableThrottleThumb_tooltip");
			settingsSections.Add(throttleThumbToggle);
			DialogGUIToggle geeThumbToggle = new DialogGUIToggle(() => ConfigManager.options[Constants.GEE_THUMB_ENABLED_CFG], Localizer.Format("#autoLOC_ZUI_ConfigUI_EnableGForceThumb"), ToggleGeeThumb);
			geeThumbToggle.tooltipText = Localizer.Format("#autoLOC_ZUI_ConfigUI_EnableGForceThumb_tooltip");
			settingsSections.Add(geeThumbToggle);
			DialogGUIToggle throttleThumbDragToggle = new DialogGUIToggle(() => ConfigManager.options[Constants.THROTTLE_THUMB_DRAG_ENABLED_CFG], Localizer.Format("#autoLOC_ZUI_ConfigUI_AllowThrottleThumbDrag"), ToggleThrottleThumbDrag);
			throttleThumbDragToggle.tooltipText = Localizer.Format("#autoLOC_ZUI_ConfigUI_AllowThrottleThumbDrag_tooltip");
			settingsSections.Add(throttleThumbDragToggle);
			DialogGUIToggle throttleEmbedToggle = new DialogGUIToggle(() => ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG], Localizer.Format("#autoLOC_ZUI_ConfigUI_EmbedThrottleThumb"), ToggleThrottleEmbed);
			throttleThumbDragToggle.tooltipText = Localizer.Format("#autoLOC_ZUI_ConfigUI_EmbedThrottleThumb_tooltip");
			settingsSections.Add(throttleEmbedToggle);

			DialogGUIScrollList optionsScrollList = new DialogGUIScrollList(Vector2.one,
				false,
				true,
				new DialogGUIVerticalLayout(windowWidth - (2 * paddingWindow) - (2 * paddingXSmall), 64,
				paddingXSmall,
				new RectOffset((int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall, (int)paddingXSmall),
				TextAnchor.MiddleLeft,
				settingsSections.ToArray()));

			settingsPage = new DialogGUIVerticalLayout(true, false, paddingSmall, new RectOffset((int)paddingRegular, (int)paddingRegular, (int)paddingRegular, (int)paddingRegular), TextAnchor.UpperCenter, optionsScrollList);

			// tab buttons
			DialogGUIToggleButton configTab = new DialogGUIToggleButton(() => currentTab == ZUITab.Configuration,
				Localizer.Format("#autoLOC_ZUI_ConfigUI_ConfigTab"),
				delegate (bool selected) {
					if (selected) SetTab(ZUITab.Configuration);
				},
				(windowWidth / 2) - (2 * paddingRegular), buttonHeight
			);
			DialogGUIToggleButton otherSettingsTab = new DialogGUIToggleButton(() => currentTab == ZUITab.OtherSettings,
				Localizer.Format("#autoLOC_ZUI_ConfigUI_OtherSettingsTab"),
				delegate (bool selected) {
					if (selected) SetTab(ZUITab.OtherSettings);
				},
				(windowWidth / 2) - (2 * paddingRegular), buttonHeight
			); 

			// tab container
			DialogGUIHorizontalLayout tabContainer = new DialogGUIHorizontalLayout(TextAnchor.MiddleCenter, configTab, otherSettingsTab);

			DialogGUIBox configPageBox = new DialogGUIBox(null, -1, -1, () => currentTab == ZUITab.Configuration, configPage);
			DialogGUIBox settingsPageBox = new DialogGUIBox(null, -1, -1, () => currentTab == ZUITab.OtherSettings, settingsPage);

			popupDialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
				new MultiOptionDialog("ZUIConfigWindow",
					"",
					Localizer.Format("#autoLOC_ZUI_ConfigUI_title"),
					HighLogic.UISkin,
					new Rect(0.5f, 0.5f, windowWidth, windowHeight),
					new DialogGUIBase[] { tabContainer, configPageBox, settingsPageBox } ),
				false, HighLogic.UISkin, false);
			if (!Constants.ENABLE_HUDREPLACER_FUNCTIONALITY) {
				// make it 'inactive'
				// idk (and idc) if this isn't how you're supposed to do it
				CanvasGroup canvasGroup = configTab.toggle.gameObject.AddComponent<CanvasGroup>();
				canvasGroup.alpha = 0.5f;
				canvasGroup.interactable = false;
			}
		}
		private static void ToggleConfig(bool selected, ZUIConfig config) {
			Debug.Log($"[ZUI] toggling {config.name} to {selected}");
			changedConfigs.Add(config);
			if (selected) {
				ConfigManager.EnableConfig(config);
			} else {
				ConfigManager.DisableConfig(config);
			}
		}
		private static void ToggleAdaptiveNavball(bool active) {
			ConfigManager.options[Constants.ADAPTIVE_NAVBALL_ENABLED_CFG] = active;
			if (AdaptiveNavball.Instance != null) {
				if (active) {
					AdaptiveNavball.Instance.EnableAdaptiveNavball();
				} else {
					AdaptiveNavball.Instance.DisableAdaptiveNavball();
				}
			}
			ConfigManager.SaveConfigOverrides();
		}
		// RAHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH
		private static void ToggleThrottleThumb(bool active) {
			ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG] = active;
			if (GaugeThumbs.Instance != null) GaugeThumbs.Instance.ToggleThrottleThumb(active);
			ConfigManager.SaveConfigOverrides();
		}
		private static void ToggleGeeThumb(bool active) {
			ConfigManager.options[Constants.GEE_THUMB_ENABLED_CFG] = active;
			if (GaugeThumbs.Instance != null) GaugeThumbs.Instance.ToggleGeeThumb(active);
			ConfigManager.SaveConfigOverrides();
		}
		private static void ToggleThrottleThumbDrag(bool active) {
			ConfigManager.options[Constants.THROTTLE_THUMB_DRAG_ENABLED_CFG] = active;
			if (GaugeThumbs.Instance != null) GaugeThumbs.Instance.SetThrottleThumbDrag(active);
			ConfigManager.SaveConfigOverrides();
		}
		private static void ToggleThrottleEmbed(bool active) {
			ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG] = active;
			if (GaugeThumbs.Instance != null) GaugeThumbs.Instance.SetThrottleThumbEmbed(active);
			ConfigManager.SaveConfigOverrides();
		}
		private static void ApplyConfigWindow() {
			if (changedConfigs.Exists(c => c.requireSceneReload || c.requireRestart)) {
				List<string> requireSceneReload = changedConfigs.Where(c => c.requireSceneReload).ToList().ConvertAll(c => c.name.CamelCaseToHumanReadable());
				List<string> requireRestart = changedConfigs.Where(c => c.requireRestart).ToList().ConvertAll(c => c.name.CamelCaseToHumanReadable());
				List<DialogGUIBase> container = new List<DialogGUIBase>();
				DialogGUIButton okButton = new DialogGUIButton(Localizer.Format("#autoLOC_ZUI_ConfigUI_ok"), ApplyConfigs, windowWidth - (2 * paddingRegular), buttonHeight, true);
				if (requireSceneReload.Count != 0) {
					DialogGUILabel needSceneReloadBox = new DialogGUILabel(Localizer.Format("#autoLOC_ZUI_ConfigUI_NeedSceneReload") + "\n\t" + string.Join("\n\t", requireSceneReload), windowWidth - (2 * paddingWindow) - (2 * paddingRegular));
					container.Add(needSceneReloadBox);
				}
				if (requireRestart.Count != 0) {
					DialogGUILabel needRestartBox = new DialogGUILabel(Localizer.Format("#autoLOC_ZUI_ConfigUI_NeedGameRestart") + "\n\t" + string.Join("\n\t", requireRestart), windowWidth - (2 * paddingWindow) - (2 * paddingRegular));
					container.Add(needRestartBox);
				}
				container.Add(okButton);
				DialogGUIVerticalLayout dialog = new DialogGUIVerticalLayout(container.ToArray());
				if (applyNotice != null) {
					applyNotice.Dismiss();
					applyNotice = null;
				}
				changedConfigs.Clear();
				applyNotice = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
					new MultiOptionDialog("ApplyConfigWindow",
						Localizer.Format("#autoLOC_ZUI_ConfigUI_ApplyConfigPretext"),
						null,
						HighLogic.UISkin,
						new Rect(0.5f, 0.5f, windowWidth, -1),
						new DialogGUIContentSizer(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize, true),
						dialog),
					false, HighLogic.UISkin, true);
				applyNotice.SetDraggable(false);
			} else {
				ApplyConfigs();
			}
		}
		private static void ApplyConfigs() {
			if (applyNotice != null) {
				applyNotice.Dismiss();
				applyNotice = null;
			}
			ConfigManager.SaveConfigOverrides();
			ConfigManager.SetConfigs();
		}
		private static void SetTab(ZUITab tab) {
			if (tab == currentTab) return;
			currentTab = tab;
			Debug.Log($"[ZUI] current tab: {tab}");
		}
		private static DialogGUIBox CreateSettingsHeader(string title) {
			return new DialogGUIBox(title, -1f, 18f);
		}
	}
}