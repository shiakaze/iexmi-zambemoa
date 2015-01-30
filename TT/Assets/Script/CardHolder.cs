using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class CardHolder : JayObject
{
	public float HandWidth;
	private Card[] cards;

	// Use this for initialization
	void Start ()
	{
		cards = GetComponentsInChildren<Card> ();
		for (int i = 0, max = cards.Length; i < max; ++i) {
			cards [i].Position = Position + new Vector3 (i * (HandWidth / max), 0, i);
			cards [i].name = name + i;
		}

		collider.enabled = false;
		CustomEventStream.Instance.Subscribe (CustomEventHandler, Cursor.CursorChannelName);
	}

	// Update is called once per frame
	#region implemented abstract members of JayObject
	
	protected override void CustomEventHandler (CustomEvent evnt)
	{
		if (evnt.Contains ("Notification", "Select")) {
			collider.enabled = true;
		}
		if (evnt.Contains ("Notification") && ((string)evnt ["Notification"]) == "Deselect") {
			collider.enabled = false;

		}
		
		#endregion
	}


}
