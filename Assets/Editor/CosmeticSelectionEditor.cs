using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor {
	[CustomEditor(typeof(CosmeticShop))]
	public class CosmeticSelectionEditor : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();
			CosmeticsManager manager = FindObjectOfType<CosmeticsManager>();
			if (manager == null || manager.Cosmetics.Count == 0)
				return;
			CosmeticShop shop = (CosmeticShop) this.target;
			string[] elements = new string[manager.Cosmetics.Count];
			for (int i = 0; i < manager.Cosmetics.Count; i++)
				elements[i] = manager.Cosmetics[i].Name;
			shop.CosmeticID = EditorGUILayout.Popup("Cosmetic", shop.CosmeticID, elements);
			shop.Cosmetic = manager.Cosmetics[shop.CosmeticID];
			shop.RefreshModel();
			if (GUI.changed) {
				EditorUtility.SetDirty(shop);
				EditorSceneManager.MarkSceneDirty(shop.gameObject.scene);
			}
		}
	}
}
