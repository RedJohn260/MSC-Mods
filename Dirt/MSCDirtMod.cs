using MSCLoader;
using UnityEngine;

namespace MSCDirtMod
{
	public class MSCDirtMod : Mod
	{
		private bool m_isLoaded = false;
		public override string ID => "DirtModRevived";
		public override string Name => "DirtModRevived";
		public override string Author => "Zamp, RedJohn260";
		public override string Version => "1.0";
		public override bool UseAssetsFolder => true;

		public static string assetPath;

		public static Keybind keyMoreDirt = new Keybind("moredirt", "More Dirt", KeyCode.KeypadPlus, KeyCode.LeftControl);
		public static Keybind keyLessDirt = new Keybind("lessdirt", "Less Dirt", KeyCode.KeypadMinus, KeyCode.LeftControl);
		public static Settings debugMode = new Settings("debug", "Debug Mode", false);

		public override void OnLoad()
		{
			assetPath = ModLoader.GetModAssetsFolder(this);
			Keybind.Add(this, keyMoreDirt);
			Keybind.Add(this, keyLessDirt);
		}

		public override void Update()
		{
			if (Application.loadedLevelName == "GAME")
			{
				if (!m_isLoaded)
				{
					new GameObject("DirtMod").AddComponent<ModBehaviour>();
					m_isLoaded = true;
				}
			}
			else if (Application.loadedLevelName != "GAME" && m_isLoaded)
			{
				m_isLoaded = false;
			}
		}
		public override void ModSettings()
        {
			Settings.AddCheckBox(this, debugMode);
		}
	}
}
