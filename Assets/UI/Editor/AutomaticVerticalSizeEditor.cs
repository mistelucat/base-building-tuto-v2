using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AutomaticVerticalSize))]
public class AutomaticVerticalSizeEditor : Editor {

	public override void OnInspectorGUI () {

		DrawDefaultInspector ();

		if ( GUILayout.Button ("Recalc the size")) {
			((AutomaticVerticalSize)target).AdjustSize ();
			//qui est équivalent à 
			//AutomaticVerticalSize myScript = ((AutomaticVerticalSize)target);
			//myscript.AdjustSize();

		}
	}

}
