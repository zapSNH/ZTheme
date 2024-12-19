using KSP.UI.Screens.Flight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using KSP.Localization;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class GaugeThumbs : MonoBehaviour {
		public static GaugeThumbs Instance { get; private set; }

		private GameObject autopilotModesGObj;
		private GameObject deltaVGaugeGObj;
		private ThrottleGauge throttleGauge;
		private GeeGauge geeGauge;
		private TextMeshProUGUI throttleText;
		private TextMeshProUGUI geeText;
		private float currentSpacingLeft = 0;
		private float currentSpacingRight = 0;

		private float originalThrottleGaugeMaxRot;
		private float originalThrottleGaugeMinRot;
		private float originalGeeGaugeMaxRot;
		private float originalGeeGaugeMinRot;

		private Texture2D navballFrameTexture;
		private string navballFrameOriginalPath = Constants.MOD_FOLDER + "PluginData/NavBall-Bg.png";
		private string navballFrameGaugeThumbsPath = Constants.MOD_FOLDER + "PluginData/NavBall-Bg-GaugeThumbs.png";
		private string navballFrameGaugeThumbsEmbeddedPath = Constants.MOD_FOLDER + "PluginData/NavBall-Bg-FullGauge.png";

		private string gForceSuffix = "G";

		public struct ThumbImage {
			public string name;
			public string path;
			public Vector3 thumbPosition;
			public Vector3 textPosition;
			public Vector3 textRotation;
			public float fontSize;
			public float sidePadding;
			public bool draggable;
			public float thumbScale;
			public float targetMaxRot;
			public float targetMinRot;
			public ThumbImage(string name, string path, Vector3 thumbPosition, Vector3 textPosition, Vector3 textRotation, float fontSize, float sidePadding, bool draggable, float thumbScale, float targetMaxRot, float targetMinRot) {
				this.name = name;
				this.path = path;
				this.thumbPosition = thumbPosition;
				this.textPosition = textPosition;
				this.textRotation = textRotation;
				this.fontSize = fontSize;
				this.sidePadding = sidePadding;
				this.draggable = draggable;
				this.thumbScale = thumbScale;
				this.targetMaxRot = targetMaxRot;
				this.targetMinRot = targetMinRot;
			}
		}

		//private static ThumbImage regularDragThumb = new ThumbImage("Regular", Constants.ZUI_FOLDER + "Assets/throttle-thumb", Vector3.zero, new Vector3(-24, 0, 0), Vector3.zero, 28, 128, true, 1.3f);
		private static ThumbImage compactDragThumb = new ThumbImage("CompactDraggable", Constants.ZUI_FOLDER + "Assets/throttle-thumb-compact-draggable", new Vector3(-2.25f, 0, 0), new Vector3(4, 0, 0), new Vector3(0, 0, 90), 14, 32, true, 1.3f, -32, 32);
		private static ThumbImage compactThumb = new ThumbImage("Compact", Constants.ZUI_FOLDER + "Assets/throttle-thumb-compact", Vector3.zero, Vector3.zero, new Vector3(0, 0, 90), 14, 24, false, 1.3f, -32, 32);
		// nice variable names, loser.
		private static ThumbImage compactDragThumbEmbed = new ThumbImage("CompactDraggableEmbedded", Constants.ZUI_FOLDER + "Assets/throttle-thumb-compact-draggable", new Vector3(-2.25f, 0, 0), new Vector3(4, 0, 0), new Vector3(0, 0, 90), 14, 0, true, 1f, -32, 48);
		private static ThumbImage compactThumbEmbed = new ThumbImage("CompactEmbedded", Constants.ZUI_FOLDER + "Assets/throttle-thumb-compact", Vector3.zero, Vector3.zero, new Vector3(0, 0, 90), 14, 0, false, 1.015f, -32, 48);

		private GameObject throttleThumbObject = null;
		private GameObject geeThumbObject = null;

		public void Awake() {
			if (Instance == null || Instance == this) {
				Instance = this;
			} else {
				Destroy(gameObject);
			}
		}

		public void Start() {
			gForceSuffix = Localizer.Format("#autoLOC_ZUI_GForceSuffix");
			throttleGauge = GameObject.Find(Constants.THROTTLE_GAUGE_GOBJ_NAME).GetComponent<ThrottleGauge>();
			geeGauge = GameObject.Find(Constants.GEE_GAUGE_GOBJ_NAME).GetComponent<GeeGauge>();
			autopilotModesGObj = GameObject.Find(Constants.AUTOPILOT_MODES_GOBJ_NAME);
			deltaVGaugeGObj = GameObject.Find(Constants.DV_GAUGE_GOBJ_NAME);

			originalThrottleGaugeMaxRot = throttleGauge.gauge.maxRot;
			originalThrottleGaugeMinRot = throttleGauge.gauge.minRot;

			originalGeeGaugeMaxRot = geeGauge.gauge.maxRot;
			originalGeeGaugeMinRot = geeGauge.gauge.minRot;

			foreach (Texture2D tex in (Texture2D[])(object)Resources.FindObjectsOfTypeAll(typeof(Texture2D))) {
				if (tex.name == Constants.NAVBALL_FRAME_TEXTURE) {
					navballFrameTexture = tex;
					break;
				}
			}

			if (ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG]) {
				ToggleThrottleThumb(true);
			}
			if (ConfigManager.options[Constants.GEE_THUMB_ENABLED_CFG]) {
				ToggleGeeThumb(true);
			}
		}
		public void SetThrottleThumbDrag(bool draggable) {
			ConfigManager.options[Constants.THROTTLE_THUMB_DRAG_ENABLED_CFG] = draggable;
			if (ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG]) ToggleThrottleThumb(true);
		}
		public void SetThrottleThumbEmbed(bool embedded) {
			ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG] = embedded;
			if (ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG]) ToggleThrottleThumb(true);
			if (ConfigManager.options[Constants.GEE_THUMB_ENABLED_CFG]) ToggleGeeThumb(true);
		}

		public void ToggleThrottleThumb(bool active) {
			// horrible setup, *might* fix later
			ThumbImage thumbImage;
			if (ConfigManager.options[Constants.THROTTLE_THUMB_DRAG_ENABLED_CFG]) {
				if (ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG]) {
					thumbImage = compactDragThumbEmbed;
				} else {
					thumbImage = compactDragThumb;
				}
			} else {
				if (ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG]) {
					thumbImage = compactThumbEmbed;
				} else {
					thumbImage = compactThumb;
				}
			}
			if (throttleThumbObject != null) {
				Destroy(throttleThumbObject);
				autopilotModesGObj.transform.localPosition += new Vector3(currentSpacingLeft, 0, 0);
				currentSpacingLeft = 0;
				throttleGauge.gauge.maxRot = originalThrottleGaugeMaxRot;
				throttleGauge.gauge.minRot = originalThrottleGaugeMinRot;
			}
			if (active) {
				throttleGauge.gauge.maxRot = thumbImage.targetMaxRot;
				throttleGauge.gauge.minRot = thumbImage.targetMinRot;
				throttleThumbObject = CreateGaugeThumb(throttleGauge.gameObject, thumbImage, out throttleText, leftSide: true);
				if (ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG]) {
					ImageConversion.LoadImage(navballFrameTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballFrameGaugeThumbsEmbeddedPath));
				} else {
					ImageConversion.LoadImage(navballFrameTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballFrameGaugeThumbsPath));
				}
			} else {
				ImageConversion.LoadImage(navballFrameTexture, File.ReadAllBytes(KSPUtil.ApplicationRootPath + navballFrameOriginalPath));
			}
		}
		public void ToggleGeeThumb(bool active) {
			if (geeThumbObject != null) {
				Destroy(geeThumbObject);
				deltaVGaugeGObj.transform.localPosition -= new Vector3(currentSpacingRight, 0, 0);
				currentSpacingRight = 0;
			}
			if (active) {
				if (ConfigManager.options[Constants.THROTTLE_THUMB_EMBED_CFG]) {
					geeGauge.gauge.maxRot = 32f;
					geeGauge.gauge.minRot = -76f;
					geeThumbObject = CreateGaugeThumb(geeGauge.gameObject, compactThumbEmbed, out geeText, rightSide: true);
				} else {
					geeGauge.gauge.maxRot = originalGeeGaugeMaxRot;
					geeGauge.gauge.minRot = originalGeeGaugeMinRot;
					geeThumbObject = CreateGaugeThumb(geeGauge.gameObject, compactThumb, out geeText, rightSide: true);
				}
			}
		}

		private GameObject CreateGaugeThumb(GameObject gaugeObject, ThumbImage thumbImage, out TextMeshProUGUI gaugeText, bool leftSide = false, bool rightSide = false) {
			// autopilot modes and dv gauge padding
			if (leftSide) {
				currentSpacingLeft = thumbImage.sidePadding;
				autopilotModesGObj.transform.localPosition -= new Vector3(currentSpacingLeft, 0, 0);
			} else if (rightSide) {
				currentSpacingRight = thumbImage.sidePadding + 8;
				deltaVGaugeGObj.transform.localPosition += new Vector3(currentSpacingRight, 0, 0);
			}

			// instantiate thumb from gauge
			GameObject gaugeThumb = Instantiate(gaugeObject, gaugeObject.transform.parent);
			gaugeThumb.transform.localScale = new Vector3(thumbImage.thumbScale, thumbImage.thumbScale, 1);

			// thumb visual
			GameObject gaugeThumbVisual = gaugeThumb.transform.GetChild(0).gameObject;
			Sprite thumbSprite = CreateThumbImage(thumbImage);
			gaugeThumbVisual.GetComponent<Image>().sprite = thumbSprite;
			gaugeThumbVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(thumbSprite.texture.width, thumbSprite.texture.height);
			
			gaugeThumbVisual.transform.localPosition += thumbImage.thumbPosition;
			gaugeThumbVisual.transform.localScale /= thumbImage.thumbScale;

			// dragging
			if (thumbImage.draggable) {
				gaugeThumbVisual.AddComponent<ThumbDrag>();
				gaugeThumbVisual.AddComponent<Button>(); // adds effects when mouse is hovering/clicking
			}

			// create text
			GameObject gaugeThumbTextObject = new GameObject("GaugeThumbText");

			gaugeText = gaugeThumbTextObject.AddComponent<TextMeshProUGUI>();
			gaugeText.text = "0";
			gaugeText.font = UISkinManager.TMPFont;
			gaugeText.fontSize = thumbImage.fontSize;
			gaugeText.alignment = TextAlignmentOptions.Center;

			gaugeThumbTextObject.transform.SetParent(gaugeThumbVisual.transform);
			gaugeThumbTextObject.transform.localPosition = thumbImage.textPosition;
			gaugeThumbTextObject.transform.localEulerAngles = thumbImage.textRotation;
			gaugeThumbTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(thumbSprite.texture.width, thumbSprite.texture.height);

			return gaugeThumb;
		}
		private Sprite CreateThumbImage(ThumbImage thumbImage) {
			Texture2D imageTexture = GameDatabase.Instance.GetTexture(thumbImage.path, false);
			return Sprite.Create(imageTexture, new Rect(0, 0, imageTexture.width, imageTexture.height), new Vector2(0.5f, 0.5f));
		}
		public void Update() {
			if (throttleThumbObject != null) throttleText.text = Mathf.CeilToInt(throttleGauge.gauge.Value * 100).ToString(); // use ceil to prevent cases where text displays 0 even if throttle is != 0 but > 0.5 
			if (geeThumbObject != null) geeText.text = Mathf.RoundToInt((float)FlightGlobals.ActiveVessel.geeForce).ToString() + gForceSuffix;
		}
	}
}