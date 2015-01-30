using UnityEngine;
using System.Collections;

public class Card : JayObject
{
	private bool selected;
	public bool Selected {
		get {
			return selected;
		}
		set {
			selected = value;
			if (selected) {
				print (SelectedPosition.z);
			}
		}
	}

	private bool focused;
	public bool Focused {
		get {
			return focused;
		}
		set {
			focused = value;
			Animator.SetBool ("Focused", focused);
		}
	}

	public readonly Vector3 SelectedPosition = new Vector3 ();

	// Use this for initialization
	void Start ()
	{
		CustomEventStream.Instance.Subscribe (CustomEventHandler, Cursor.CursorChannelName);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Selected) {
			ObjectTransform.position = Cursor.GetCursorWorldPosition (SelectedPosition.z);
		}
	}

	#region implemented abstract members of JayObject

	protected override void CustomEventHandler (CustomEvent evnt)
	{
		if (Selected == false && evnt.Contains ("Notification", "Target")) {
			Focused = (((GameObject)evnt ["Target"]) == gameObject);
		}
		if (evnt.Contains ("Notification", "Select")) {
			Selected = Focused;
		}

		#endregion
	}
}
