using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HierarchyHelper
{
	public delegate void Mutator (Transform child);

	/// <summary>
	/// Gets the childrens in generation.
	/// 
	/// generation 0 means first level of children. 
	/// You can't go below 0
	/// </summary>
	/// <returns>The childrens in generation.</returns>
	/// <param name="parent">Parent.</param>
	/// <param name="layer">Layer.</param>
	public static List<GameObject> GetChildrensInGeneration (GameObject parent, int layer = 0)
	{
		return GetChildrensInGeneration (parent, layer);
	}

	/// <summary>
	/// Gets the childrens in generation.
	/// 
	/// generation 0 means first level of children. 
	/// You can't go below 0
	/// </summary>
	/// <returns>The childrens in generation.</returns>
	/// <param name="parent">Parent.</param>
	/// <param name="layer">Layer.</param>
	public static List<GameObject> GetChildrensInGeneration (Transform parent, int layer)
	{
		List<GameObject> children = new List<GameObject> ();
		foreach (Transform child in parent) {
			if (layer == 0) {
				children.Add (child.gameObject);
			} else {
				children.AddRange (GetChildrensInGeneration (child, layer - 1));
			}
		}

		return children;
	}



	public static void MutateChildrenInGeneration (Mutator mutator, Transform parent, int layer)
	{
		foreach (Transform child in parent) {
			if (layer == 0) {
				mutator (child);
			} else {
				MutateChildrenInGeneration (mutator, child, layer - 1);
			}
		}
	}

	public static int GetChildrensInGenerationCount (GameObject parent, int layer = 0)
	{
		return GetChildrensInGenerationCount (parent.transform, layer);
	}

	public static int GetChildrensInGenerationCount (Transform parent, int layer)
	{
		return GetChildrensInGeneration (parent, layer).Count;
	}
}