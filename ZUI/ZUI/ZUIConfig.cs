using System.Collections.Generic;
using UnityEngine;

namespace ZUI
{
	public class ZUIConfig
	{
		[Persistent] public string name;
		[Persistent] public string localizedName;
		[Persistent] public bool requireSceneReload;
		[Persistent] public bool requireRestart;
		public bool HasRecolorNode { 
			get {
				if (configNode != null) {
					return configNode.HasNode(Constants.ZUIHUDREPLACER_RECOLOR_NODE);
				} else {
					return false;
				}
			}
		}
		public bool HasHUDReplacerNode {
			get {
				if (configNode != null) {
					return configNode.HasNode(Constants.ZUIHUDREPLACER_NODE);
				} else {
					return false;
				}
			}
		}

		public ConfigNode configNode;

		public ZUIConfig(ConfigNode configNode) {
			this.configNode = configNode;
			ConfigNode.LoadObjectFromConfig(this, configNode);
		}

		public ConfigNode[] GetConfigNodesAsHUDReplacerNodes() {
			ConfigNode[] configNodes = configNode.GetNodes(Constants.ZUIHUDREPLACER_NODE);
			List<ConfigNode> hrNodes = new List<ConfigNode>();
			foreach (ConfigNode configNode in configNodes) {
				ConfigNode hrNode = new ConfigNode(Constants.HUDREPLACER_NODE);
				int priority = 1;
				foreach (ConfigNode.Value value in configNode.values) {
					if (value.name == Constants.HUDREPLACER_PRIORITY_CFG) {
						int.TryParse(configNode.GetValue(Constants.HUDREPLACER_PRIORITY_CFG), out priority);
						continue;
					}
					hrNode.AddValue(value.name, value.value);
				}
				hrNode.AddValue(Constants.HUDREPLACER_PRIORITY_CFG, priority);
				hrNodes.Add(hrNode);
			}
			return hrNodes.ToArray();
		}

		public ConfigNode[] GetRecolorConfigNodesAsHUDReplacerRecolorNodes() {
			ConfigNode[] recolorNodes = configNode.GetNodes(Constants.ZUIHUDREPLACER_RECOLOR_NODE);
			List<ConfigNode> hrRecolorNodes	= new List<ConfigNode>();
			foreach (ConfigNode recolorNode in recolorNodes) {
				ConfigNode hrRecolorNode = new ConfigNode(Constants.HUDREPLACER_RECOLOR_NODE);
				int priority = 1;
				foreach (ConfigNode.Value value in recolorNode.values) {
					if (value.name == Constants.HUDREPLACER_PRIORITY_CFG) {
						int.TryParse(recolorNode.GetValue(Constants.HUDREPLACER_PRIORITY_CFG), out priority);
						continue;
					}
					string[] colorStr = value.value.Replace(" ", "").Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
					if (colorStr.Length < 4) {
						Debug.Log("[ZUI] Invalid color!");
						continue;
					}
					string recolorTarget = value.name;
					Color color = new Color(float.Parse(colorStr[0]), float.Parse(colorStr[1]), float.Parse(colorStr[2]), float.Parse(colorStr[3]));
					hrRecolorNode.AddValue(recolorTarget, color);
				}
				hrRecolorNode.AddValue(Constants.HUDREPLACER_PRIORITY_CFG, priority);
				hrRecolorNodes.Add(hrRecolorNode);
			}
			return hrRecolorNodes.ToArray();
		}
	}
}