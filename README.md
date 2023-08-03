# ZTheme
A very cool and dark KSP Theme based on HUDReplacer's example theme
[![very neat](https://github.com/zapSNH/ZTheme-Files/blob/main/banner.png?raw=true "very neat")](https://github.com/zapSNH/ZTheme-Files/blob/main/banner.png?raw=true "very neat")
Navball Not Included

## Dependencies
[HUDReplacer](https://github.com/UltraJohn/HUDReplacer/releases "HUDReplacer") + its dependencies

[Module Manager](https://github.com/sarbian/ModuleManager "Module Manager")

## (maybe) Frequently Asked Questions
### How Do I Get The Navball?
The navball in the screenshot(s) is not included in this theme. There are also many more navballs that look nice with (or without) this theme.

There are two ways of changing the navball

In these examples, I'll be using the [Principia navball](https://github.com/mockingbirdnest/Principia/blob/master/ksp_plugin_adapter/assets/navball_compass.png)


### 1 - Using NavBallTextureChanger Updated

NavBallTextureChanger Updated is a mod for KSP which... well... changes the navball texture.
You can change the navball pretty easily in the UI with this mod.

#### I. Install [NavBallTextureChanger](https://forum.kerbalspaceprogram.com/topic/200741-112x-navballtexturechanger-updated-now-with-ui/) Updated

Installing NBTCU is pretty similar to installing ZTheme.

#### II. Copy the texture into `GameData/NavBallTextureChanger/PluginData/Skins`

I recommend to also copy the navball twice and naming one navballName_em.png if you want to create emissives for it.

#### III.  Create the config file
Here's an example of what a config file looks like. 

If you're using the Principia navball then this should automatically work if you named the navballs `navball_compass.png` & `navball_compass_em.png`
```js
NavballTextureChanger
{
	FILE_EMISSIVE
	{
		file = PluginData/Skins/navball_compass.png
		emissive = PluginData/Skins/navball_compass_em.png
		descr = NavballCompass
	}
}
```
Make sure the file ends with .cfg

#### IV. Launch KSP and change the navball through the toolbar icon


### 2 - Using HUDReplacer
This method does not need any more mods as HUDReplacer is a dependency of ZTheme.

This method also is pretty simplified so that people who don't know how to do method 1 can easily do this.

#### I. Copy your texture to `GameData/ZTheme/PluginData` and rename it to `NavBall.png`
...and you're done!

If all goes well then you should see your magnificent navball that replaces the stinky old one. 

### I don't like what you did with X! Can you change it back?
- The older variants can always be downloaded through older releases and it also may be in the [ZThemeExtras](https://github.com/zapSNH/ZThemeExtras "zthemeExtras").

## Known Issues
 - Some textures are missing icons [#1](https://github.com/zapSNH/ZTheme/issues/1 "#1")


## Special Thanks
UltraJohn for creating HUDReplacer


Combatpigeon96 for being the one who made a [Reddit post](https://www.reddit.com/r/KerbalSpaceProgram/comments/12et06i/are_there_any_mods_that_change_the_look_of_the_ui/ "Reddit Post") which showed me HUDReplacer


Nazalassa for being inspiration and also being the first to create a working dark theme (Go check out [Naztheme!](https://forum.kerbalspaceprogram.com/index.php?/topic/216234-112x-naztheme-an-alternate-theme-for-ksp/ "Naztheme!"))


MrF0X and beik for being inspiration
- Karbon UI: https://forum.kerbalspaceprogram.com/topic/195594-new-interface-ksp-karbon-ui/
- beik's Flight HUD Redesign: https://forum.kerbalspaceprogram.com/topic/187152-experiment-on-a-possible-flight-hud-redesign-result/


You for reading this README

## Licensing
GNU GPL v3.0
Except for some textures that are from the example theme. You are not allowed to use those unless you get permission from UltraJohn.
