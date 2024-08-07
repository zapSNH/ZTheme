using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Partially based on wheeeUI and Visual Studio's autocomplete feature
namespace ZUI {
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class Transformer : MonoBehaviour {
		internal static Transformer Instance { get; private set; }
		internal ConfigNode[] transforms;
		internal List<GameObject> transformObjects = new List<GameObject>();
		internal List<bool> relativeTransform = new List<bool>();
		public List<Vector3> translateAmounts = new List<Vector3>();
		public List<Vector3> rotateAmounts = new List<Vector3>(); // rotation will always non-relative
		public List<Vector3> scaleAmounts = new List<Vector3>();

		private bool debugMode = false;

		public void Start() {
			if (Instance == null) {
				Instance = this;
			} else {
				Destroy(gameObject);
				return;
			}
			GetTransform();
			SetTransform();
		}
		internal void GetTransform() {
			UrlDir.UrlConfig[] ZUINodes = GameDatabase.Instance.GetConfigs(Constants.ZUI_NODE);
			foreach (UrlDir.UrlConfig node in ZUINodes) {
				transforms = node.config.GetNodes(Constants.ZUITRANSFORM_NODE);
				foreach (ConfigNode config in transforms) {

					if (!config.HasValue(Constants.TARGET_TRANSFORM_CFG)) {
						Debug.Log("[ZUI] Node does not have a transform target!");
						continue;
					}

					string target = config.GetValue(Constants.TARGET_TRANSFORM_CFG);
					bool isMulti = false;

					if (config.HasValue(Constants.MULTI_OBJECT_TRANSFORM_CFG)) {
						bool.TryParse(config.GetValue(Constants.MULTI_OBJECT_TRANSFORM_CFG), out isMulti);
					}

					List<GameObject> gameObjects = new List<GameObject>();

					if (isMulti) {
						gameObjects = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == target) as List<GameObject>;
					} else {
						gameObjects.Add(GameObject.Find(target));
					}

					foreach (var gameObject in gameObjects) {
						if (!TryAddTransform(gameObject, config))
							Debug.Log($"[ZUI] Invalid transform target! ({target})");
					}
				}

				ConfigNode[] ZUISettings = node.config.GetNodes(Constants.ZUISETTINGS_NODE);
				foreach (ConfigNode config in ZUISettings) {
					config.TryGetValue(Constants.DEBUG_CFG, ref debugMode);
				}
			}
		}
		private bool TryAddTransform(GameObject gameObject, ConfigNode config) {
			if (gameObject != null) {
				Vector3 translate = Vector3.negativeInfinity;
				Vector3 rotate = Vector3.negativeInfinity;
				Vector3 scale = Vector3.negativeInfinity;
				bool isRelative = false;

				if (config.HasValue(Constants.RELATIVE_TRANSFORM_CFG)) {
					bool.TryParse(config.GetValue(Constants.RELATIVE_TRANSFORM_CFG), out isRelative);
				}
				if (config.HasValue(Constants.TRANSLATE_CFG)) {
					config.TryGetValue(Constants.TRANSLATE_CFG, ref translate);
				}
				if (config.HasValue(Constants.ROTATE_CFG)) {
					config.TryGetValue(Constants.ROTATE_CFG, ref rotate);
				}
				if (config.HasValue(Constants.SCALE_CFG)) {
					config.TryGetValue(Constants.SCALE_CFG, ref scale);
				}

				transformObjects.Add(gameObject);
				rotateAmounts.Add(rotate);
				translateAmounts.Add(translate);
				relativeTransform.Add(isRelative);
				scaleAmounts.Add(scale);
				Debug.Log($"[ZUI] target: {gameObject.name} | translate: {translate} | rotate: {rotate}" /* | scale: {scale}"*/);
				return true;
			} else {
				return false;
			}
		}
		internal void SetTransform() {
			int i = 0;
			foreach (GameObject gameObject in transformObjects) {
				SetGameObjectTransform(gameObject, relativeTransform[i], translateAmounts[i], rotateAmounts[i], scaleAmounts[i]);
				i++;
			}
		}
		public void SetGameObjectTransform(GameObject gameObject, bool relative, Vector3 translate, Vector3 rotate, Vector3 scale) {
			if (gameObject != null) {
				if (relative) {
					if (!translate.Equals(Vector3.negativeInfinity)) {
						gameObject.transform.localPosition += translate;
					}
					if (!scale.Equals(Vector3.negativeInfinity)) {
						gameObject.transform.localScale = Vector3.Scale(scale, gameObject.transform.localScale);
					}
				} else {
					if (!translate.Equals(Vector3.negativeInfinity)) {
						gameObject.transform.localPosition = translate;
					}
					if (!scale.Equals(Vector3.negativeInfinity)) {
						gameObject.transform.localScale = scale;
					}
				}
				gameObject.transform.localEulerAngles = rotate;
			}
		}
	}
}