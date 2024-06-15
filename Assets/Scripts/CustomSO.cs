using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor;
#endif
using UnityEngine;

public class ScriptableObjectWithId : ScriptableObject, IEquatable<ScriptableObjectWithId>
{
	[field: ScriptableObjectId][field: SerializeField] public string id { get; private set; }

	public bool Equals(ScriptableObjectWithId other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return id == other.id;
	}

	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((ScriptableObjectWithId)obj);
	}

	public override int GetHashCode()
	{
		// ReSharper disable once NonReadonlyMemberInGetHashCode
		return HashCode.Combine(id);
	}

	public static bool operator ==(ScriptableObjectWithId left, ScriptableObjectWithId right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(ScriptableObjectWithId left, ScriptableObjectWithId right)
	{
		return !Equals(left, right);
	}
}

public class ScriptableObjectIdAttribute : PropertyAttribute { }


#if UNITY_EDITOR


public class SoChecker : BuildPlayerProcessor
{
	public override int callbackOrder => -1;

	public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
	{
		var ids = new HashSet<string>();

		var guids = AssetDatabase.FindAssets("t:" + typeof(ScriptableObjectWithId));
		foreach (var guid in guids)
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			var so = AssetDatabase.LoadAssetAtPath<ScriptableObjectWithId>(path);
			if (string.IsNullOrEmpty(so.id))
			{
				Debug.LogError("SO doesn't have ID", so);
				throw new Exception();
			}
			if (!ids.Add(so.id))
			{
				Debug.LogError("SO has the same ID as some other SO", so);
				throw new Exception();
			}
		}
	}
}

[CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
public class ScriptableObjectIdDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (string.IsNullOrEmpty(property.stringValue))
		{
			property.stringValue = Guid.NewGuid().ToString();
		}

		var propertyRect = new Rect(position);
		propertyRect.xMax -= 100;
		var buttonRect = new Rect(position);
		buttonRect.xMin = position.xMax - 100;

		GUI.enabled = false;
		EditorGUI.PropertyField(propertyRect, property, label, true);
		GUI.enabled = true;

		if (GUI.Button(buttonRect, "Regenerate ID"))
		{
			property.stringValue = Guid.NewGuid().ToString();
		}
	}
}
#endif

