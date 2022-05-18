using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Editor {
	[CustomEditor(typeof(MonoBehaviour), true)]
	public class DraggableVector3Editor : UnityEditor.Editor {
		private readonly GUIStyle style = new GUIStyle();

		private void OnEnable() {
			this.style.fontStyle = FontStyle.Bold;
			this.style.normal.textColor = Color.white;
		}

		private void OnSceneGUI() {
			SerializedProperty property = this.serializedObject.GetIterator();
			while (property.Next(true)) {
				if (property.propertyType == SerializedPropertyType.Vector3) {
					FieldInfo field = this.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic);
					if (field == null)
						continue;

					object[] attributes = field.GetCustomAttributes(typeof(DraggableVector3Attribute), false);
					if (attributes.Length > 0) {
						Handles.Label(property.vector3Value, property.name);
						property.vector3Value = Handles.PositionHandle(property.vector3Value, Quaternion.identity);
						this.serializedObject.ApplyModifiedProperties();
					}
				} else if (property.propertyType == SerializedPropertyType.Generic) {
					FieldInfo field = this.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic);
					if (field == null)
						continue;

					object[] attributes = field.GetCustomAttributes(typeof(DraggableVector3Attribute), false);
					if (attributes.Length > 0 && typeof(IList).IsAssignableFrom(field.FieldType)) {
						IList list = (IList) field.GetValue(this.serializedObject.targetObject);
						for (int i = 0; i < list.Count; i++) {
							if (list[i] is Vector3) {
								Vector3 vector3 = (Vector3) list[i];
								Handles.Label(vector3, property.name + "[" + i + "]");
								list[i] = Handles.PositionHandle(vector3, Quaternion.identity);
								this.serializedObject.ApplyModifiedProperties();
							}
						}
					}
				}
			}
		}
	}
}
