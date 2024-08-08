using KSP.UI.Screens.Flight;
using SaveUpgradePipeline;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZUI {
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class GaugeThumbs : MonoBehaviour {
		public static GaugeThumbs Instance { get; private set; }

		private GameObject autopilotModesGObj;
		private ThrottleGauge throttleGauge;
		private GeeGauge geeGauge;
		private TextMeshProUGUI throttleText;
		private TextMeshProUGUI geeText;

		public struct ThumbImage {
			public string name;
			public string path;
			public Vector3 thumbPosition;
			public Vector3 textPosition;
			public Vector3 textRotation;
			public float fontSize;
			public float sidePadding;
			public bool draggable;
			public ThumbImage(string name, string path, Vector3 thumbPosition, Vector3 textPosition, Vector3 textRotation, float fontSize, float sidePadding, bool draggable) {
				this.name = name;
				this.path = path;
				this.thumbPosition = thumbPosition;
				this.textPosition = textPosition;
				this.textRotation = textRotation;
				this.fontSize = fontSize;
				this.sidePadding = sidePadding;
				this.draggable = draggable;
			}
		}

		//private static ThumbImage regularDragThumb = new ThumbImage("Regular", Constants.ZUI_FOLDER + "Assets/throttle-thumb", Vector3.zero, new Vector3(-24, 0, 0), Vector3.zero, 28, 128, true);
		private static ThumbImage compactDragThumb = new ThumbImage("Compact", Constants.ZUI_FOLDER + "Assets/throttle-thumb-compact-draggable", new Vector3(-2.5f, 0, 0), new Vector3(4, 0, 0), new Vector3(0, 0, 90), 14, 32, true);
		private static ThumbImage compactThumb = new ThumbImage("Compact (no-drag)", Constants.ZUI_FOLDER + "Assets/throttle-thumb-compact", Vector3.zero, Vector3.zero, new Vector3(0, 0, 90), 14, 24, false);

		private GameObject throttleThumbObject = null;
		private GameObject geeThumbObject = null;

		private float thumbScale = 1.3f;

		public void Awake() {
			if (Instance == null || Instance == this) {
				Instance = this;
			} else {
				Destroy(gameObject);
			}
		}

		public void Start() {
			throttleGauge = GameObject.Find(Constants.THROTTLE_GAUGE_GOBJ_NAME).GetComponent<ThrottleGauge>();
			geeGauge = GameObject.Find(Constants.GEE_GAUGE_GOBJ_NAME).GetComponent<GeeGauge>();
			autopilotModesGObj = GameObject.Find(Constants.AUTOPILOT_MODES_GOBJ_NAME);

			// set custom constraints
			throttleGauge.gauge.maxRot = -32;
			throttleGauge.gauge.minRot = 32;

			if (ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG]) {
				ToggleThrottleThumb(true);
			}
			if (ConfigManager.options[Constants.GEE_THUMB_ENABLED_CFG]) {
				ToggleGeeThumb(true);
			}
		}
		public void SetThrottleThumbDrag(bool draggable) {
			ConfigManager.options[Constants.THROTTLE_THUMB_DRAG_ENABLED_CFG] = draggable;
			ToggleThrottleThumb(ConfigManager.options[Constants.THROTTLE_THUMB_ENABLED_CFG]);
		}

		public void ToggleThrottleThumb(bool active) {
			if (throttleThumbObject != null) {
				Destroy(throttleThumbObject);
			}
			if (active) {
				throttleThumbObject = CreateGaugeThumb(throttleGauge.gameObject, ConfigManager.options[Constants.THROTTLE_THUMB_DRAG_ENABLED_CFG] ? compactDragThumb : compactThumb, out throttleText, true);
			}
		}
		public void ToggleGeeThumb(bool active) {
			if (geeThumbObject != null) {
				Destroy(geeThumbObject);
			}
			if (active) {
				geeThumbObject = CreateGaugeThumb(geeGauge.gameObject, compactThumb, out geeText);
			}
		}

		private GameObject CreateGaugeThumb(GameObject gaugeObject, ThumbImage thumbImage, out TextMeshProUGUI gaugeText, bool leftSide = false) {
			// autopilot modes padding
			if (leftSide) autopilotModesGObj.transform.localPosition -= new Vector3(thumbImage.sidePadding, 0, 0);

			// instantiate thumb from gauge
			GameObject gaugeThumb = Instantiate(gaugeObject, gaugeObject.transform.parent);
			gaugeThumb.transform.localScale = new Vector3(thumbScale, thumbScale, 1);

			// thumb visual
			GameObject gaugeThumbVisual = gaugeThumb.transform.GetChild(0).gameObject;
			Sprite thumbSprite = CreateThumbImage(thumbImage);
			gaugeThumbVisual.GetComponent<Image>().sprite = thumbSprite;
			gaugeThumbVisual.GetComponent<RectTransform>().sizeDelta = new Vector2(thumbSprite.texture.width, thumbSprite.texture.height);
			
			gaugeThumbVisual.transform.localPosition += thumbImage.thumbPosition;
			gaugeThumbVisual.transform.localScale /= thumbScale;

			// dragging
			if (thumbImage.draggable) {
				gaugeThumbVisual.AddComponent<ThumbDrag>();
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
			if (throttleThumbObject != null) throttleText.text = Mathf.RoundToInt(throttleGauge.gauge.Value * 100).ToString();
			if (geeThumbObject != null) geeText.text = Mathf.RoundToInt(geeGauge.gauge.Value).ToString() + "G";
		}
	}
}