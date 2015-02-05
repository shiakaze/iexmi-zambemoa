using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildIndexer : MonoBehaviour
{
	public bool ExclusiveIndexing = true;

	private int currentIndex;
	public int CurrentIndex {
		get {
			return currentIndex;
		}
		set {
			if (value >= 0 && ((ExclusiveIndexing == true && value < ChildCount) || (ExclusiveIndexing == false && value <= ChildCount))) {
				currentIndex = value;
				Refresh ();
			}
		}
	}

	private int ChildCount {
		get {
			return HierarchyHelper.GetChildrensInGenerationCount (gameObject);
		}
	}


	public Vector3 ChildrenIndexVector;
	public Vector3 InitialPosition;

	private void Start ()
	{
		CurrentIndex = 0;
	}

	private void Refresh ()
	{
		HierarchyHelper.MutateChildrenInGeneration ((childTransform) => {
			int index = childTransform.GetSiblingIndex ();
			childTransform.localPosition = InitialPosition + ChildrenIndexVector * (index - CurrentIndex);
		}, transform, 0);
	}

	public void IncrementIndex ()
	{
		CurrentIndex++;
	}

	public void DecrementIndex ()
	{
		CurrentIndex--;
	}
}
