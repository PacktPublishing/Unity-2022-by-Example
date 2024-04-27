/* ****************************************************************************************
 * This script is only to test a bug in 2022.1.0b12 where PlayerSettings WebGL
 * Scripting Define Symbols are not appearing after adding.
 * ***************************************************************************************/

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;

public class DefineSymbolsWindow : EditorWindow
{

	[MenuItem("Window/Define Symbols")]
	public static void ShowWindow()
	{
		GetWindow<DefineSymbolsWindow>("Define Symbols");
	}

	void OnGUI()
	{
		GUILayout.Label(PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.WebGL), EditorStyles.boldLabel);

		if (GUILayout.Button("SET DEFINE SYMBOLS"))
		{
			Debug.Log("test");
		}
	}
}