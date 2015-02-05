using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour
{
	public static string CursorChannelName = "Cursor";
	private GameObject previousTarget;
	private GameObject Target {
		get {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit info;
			Physics.Raycast (ray, out info);
			Collider collider = info.collider;

			return collider == null ? null : collider.gameObject; 
		}
	}

	public static Vector2 CursorPosition {
		get {
			return Input.mousePosition;
		}
	}

	public static Vector3 GetCursorWorldPosition (float z)
	{
		return Camera.main.ScreenToWorldPoint (new Vector3 (CursorPosition.x, CursorPosition.y, z));
	}

	private void Start ()
	{
		CustomEventStream.Instance.CreateChannel (CursorChannelName);
	}

	private void Update ()
	{
		GameObject target = Target;
		if (previousTarget != target) {
			if (target != null) {
				print (target.name);
			}
			previousTarget = target;
			RaiseTargetChangeEvent (target);
		}

		if (Input.GetMouseButtonDown (0)) {
			RaiseCursorSelectEvent (target);
		} else if (Input.GetMouseButtonUp (0)) {
			RaiseCursorDeselectEvent ();
		}
	}

	private void RaiseTargetChangeEvent (GameObject target)
	{
		NotificationEvent notice = new NotificationEvent ("Target");
		notice ["Target"] = target;
		CustomEventStream.Instance.Broadcast (notice, CursorChannelName);
	}

	private void RaiseCursorSelectEvent (GameObject target)
	{
		NotificationEvent notice = new NotificationEvent ("Select");
		notice ["Select"] = target;
		CustomEventStream.Instance.Broadcast (notice, CursorChannelName);
	}
	private void RaiseCursorDeselectEvent ()
	{
		NotificationEvent notice = new NotificationEvent ("Deselect");
		CustomEventStream.Instance.Broadcast (notice, CursorChannelName);
		print ("deselected");
	}
}
