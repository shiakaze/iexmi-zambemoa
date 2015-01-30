using UnityEngine;
using System.Collections;

public abstract class JayObject : MonoBehaviour
{
	private Transform objectTransform;
	public Transform ObjectTransform {
		get {
			if (objectTransform == null) {
				objectTransform = transform;
			}

			return objectTransform;
		}
	}

	public Vector3 LocalPosition {
		get {
			return ObjectTransform.localPosition;
		}
		set {
			ObjectTransform.localPosition = value;
		}
	}

	public Vector3 Position {
		get {
			return ObjectTransform.position;
		}
		set {
			ObjectTransform.position = value;
		}
	}

	public Quaternion Rotation {
		get {
			return ObjectTransform.rotation;
		}
		set {
			ObjectTransform.rotation = value;
		}
	}

	private Animator animator;
	public Animator Animator {
		get {
			if (animator == null) {
				animator = GetComponentInChildren<Animator> ();
			}

			return animator;
		}
	}

	protected virtual void CustomEventHandler (CustomEvent evnt)
	{
		//
	}

}
