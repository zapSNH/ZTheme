
using System.Text.RegularExpressions;

namespace ZUI {
	internal static class Constants {
		internal const string GAMEDATA_FOLDER = "GameData/";
		internal const string ZUI_FOLDER = "999_ZUI/";
		internal const string MOD_FOLDER = GAMEDATA_FOLDER + ZUI_FOLDER;

		internal const string DEBUG_CFG = "debugMode";

		internal const string ZUI_NODE = "ZUI"; // main node
		internal const string ZUISETTINGS_NODE = "ZUISettings"; // global settings
		internal const string ZUINAVBALL_NODE = "ZUINavBall"; // adaptive navball
		internal const string ZUITRANSFORM_NODE = "ZUITransform"; // gameobject transform
		internal const string ZUICONFIG_NODE = "ZUIConfig"; // config that can be enabled/disabled
		internal const string ZUICONFIGOPTIONS_NODE = "ZUIConfigOptions"; // handles ZUIConfig
		internal const string ZUIHUDREPLACER_NODE = "ZUIHUDReplacerConfig"; // ZUIConfig node for hudreplacer
		internal const string ZUIHUDREPLACER_RECOLOR_NODE = "ZUIHUDReplacerRecolorConfig"; // ZUIConfig node for hudreplacer recolor

		internal const string ZUICONFIGNAME_VALUE = "name"; // name of ZUIConfig
		internal const string ZUICONFIGOPTION_ENABLED_CFG = "enabled"; // enabled ZUIConfigs
		internal const string ZUICONFIGOPTION_PRIORITY_CFG = "priority";
		internal const string ZUICONFIGOPTION_REQUIRESCENERELOAD_CFG = "requireSceneReload";
		internal const string ZUICONFIGOPTION_REQUIRERESTART_CFG = "requireRestart";

		internal const string HUDREPLACER_NODE = "HUDReplacer:NEEDS[HUDReplacer]";
		internal const string HUDREPLACER_RECOLOR_NODE = "HUDReplacerRecolor:NEEDS[HUDReplacer]";

		internal const string HUDREPLACER_PRIORITY_CFG = "priority";

		internal const string TARGET_TRANSFORM_CFG = "target";
		internal const string RELATIVE_TRANSFORM_CFG = "relative";
		internal const string MULTI_OBJECT_TRANSFORM_CFG = "multi";

		internal const string TRANSLATE_CFG = "translate";
		internal const string ROTATE_CFG = "rotate";
		//internal const string SCALE_CFG = "scale";

		internal const string ADAPTIVE_NAVBALL_ENABLED_CFG = "adaptiveNavballEnabled";
		internal const string NAVBALL_SURFACE = "navballSurface";
		internal const string NAVBALL_ORBIT = "navballOrbit";
		internal const string NAVBALL_TARGET = "navballTarget";
		internal const string NAVBALL_TEXTURE = "NavBall";

		internal const string TOOLBAR_BUTTON_ENABLED = "toolbarButtonEnabled";
	}
	internal static class Extensions {

		// https://stackoverflow.com/a/4489046
		// https://creativecommons.org/licenses/by-sa/4.0/
		/// <summary>
		/// Converts a Camel-case string format into a spaced-out human readable format.
		/// </summary>
		internal static string CamelCaseToHumanReadable(this string str) {
			return Regex.Replace(
				Regex.Replace(
					str,
					@"(\P{Ll})(\P{Ll}\p{Ll})",
					"$1 $2"
				),
				@"(\p{Ll})(\P{Ll})",
				"$1 $2"
			);
		}
	}
}