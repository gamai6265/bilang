using UnityEngine;
using UnityEditor;
using System.Collections;

public class AddGameObject : MonoBehaviour {
	[MenuItem ("GameObject/Add GameObject")]  
	static void MenuAddChild()  
	{  
		Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);  
		
		foreach(Transform transform in transforms)  
		{  
			GameObject newChild = new GameObject("GameObject");  
			newChild.transform.parent = transform;  
		}  
	}
}
