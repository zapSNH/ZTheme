using KSP.UI.Screens;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZUI {
	public class ThumbDrag : MonoBehaviour, IDragHandler, IScrollHandler {
		private Transform gaugeObject;
		private RotationalGauge gauge;
		internal static float scrollAmount = 1f;
		public void Start() {
			gaugeObject = transform.parent;
			gauge = gaugeObject.GetComponent<RotationalGauge>();
		}
		public void OnDrag(PointerEventData eventData) {
			// Mathf.Clamp could output a wrong value if minRot is larger than maxRot so we invert minRot and maxRot if that's the case
			float minRot;
			float maxRot;
			if (gauge.minRot > gauge.maxRot) {
				minRot = gauge.maxRot;
				maxRot = gauge.minRot;
			} else {
				minRot = gauge.minRot;
				maxRot = gauge.maxRot;
			}
			eventData.position = new Vector2(eventData.position.x - Screen.width / 2, eventData.position.y - Screen.height / 2);
			float targetAngle = Mathf.Clamp((Mathf.Atan2(gaugeObject.position.y - eventData.position.y, gaugeObject.position.x - eventData.position.x)
				* Mathf.Rad2Deg), minRot, maxRot);
			FlightInputHandler.state.mainThrottle = targetAngle.Remap((minRot, maxRot), (1, 0));
		}
		public void OnScroll(PointerEventData eventData) {
			FlightInputHandler.state.mainThrottle += eventData.scrollDelta.y * scrollAmount * 0.01f;
		}
	}
}