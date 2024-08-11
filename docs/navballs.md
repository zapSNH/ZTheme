---
title: "about:navballs"
---

The pre-alpha KSP2 style navball in the screenshot(s) is not included in this theme but is instead [@Squeaky's navball](https://forum.kerbalspaceprogram.com/topic/200741-112x-navballtexturechanger-updated-now-with-ui/?do=findComment&comment=4290196). There are also many more navballs that look nice with (or without) this theme.

There are three ways of changing the navball

In these examples, I'll be using the Principia navball, which looks pretty nice with this theme (and others maybe).

![image](https://github.com/user-attachments/assets/b7079633-b654-49aa-9d08-e90841c62dd5)

* * *

## 1 - Using [NavBallTextureChanger Updated](https://forum.kerbalspaceprogram.com/topic/200741-112x-navballtexturechanger-updated-now-with-ui/)

[NavBallTextureChanger Updated](https://forum.kerbalspaceprogram.com/topic/200741-112x-navballtexturechanger-updated-now-with-ui/) is a mod for KSP which... well... changes the navball texture.

You can change the navball pretty easily in the UI with this mod.

### I. Install [NavBallTextureChanger Updated](https://forum.kerbalspaceprogram.com/topic/200741-112x-navballtexturechanger-updated-now-with-ui/)
Installing NBTCU is pretty similar to installing ZTheme.
You can use CKAN or you can manually install it like ZTheme.

### II. Copy the texture into GameData/NavBallTextureChanger/PluginData/Skins
I recommend to also copy the navball twice and naming one navballName_em.png if you want to create emissives for it.

### III.  Create the config file
Here's an example of what a config file looks like. This should automatically work if you named the navballs navball_compass.png & navball_compass_em.png

```cs
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

Place the config file in the same folder as the navball texture. (this doesn't affect the functionality but is good for organization)

Make sure the file ends with `.cfg`

### IV. Launch KSP and change the navball through the toolbar icon.

You're done!
 
* * *

## 2 - Using HUDReplacer
This method does not need any more mods as HUDReplacer is a dependency of ZTheme.

This method is much simpler than method 1.

### I. Copy your texture to GameData/ZTheme/PluginData/UI and rename it to NavBall.png
...and you're done!

If all goes well then you should see your magnificent navball that replaces the stinky old one.
 

* * *

## 3 - Using ZUI
If you want your navball to be adaptive just like KSP 2, then you can use ZUI, a supplementary plugin for ZTheme.
Note: ZUI does other thing(s) too, not just an adaptive navball.

### I. Install ZUI
Installing ZUI is identical to installing ZTheme manually.
ZUI isn't currently available in CKAN because I'd like to add some more stuff before adding it to CKAN.

You're done!

If you want to change the navball textures then you can go to the ZUI folder and swap the textures or change the navball paths in config.cfg. 
