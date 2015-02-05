using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class CardHolder : JayObject
{
	public float HandWidth;
	private List<Card> cards;
	
	// Use this for initialization
	void Start ()
	{
		cards = new List<Card> (GetComponentsInChildren<Card> ());
		OrganizeCards ();
		collider.enabled = false;
		CustomEventStream.Instance.Subscribe (CustomEventHandler, Cursor.CursorChannelName);
	}
	
	private void OrganizeCards ()
	{
		for (int i = 0, max = cards.Count; i < max; ++i) {
			cards [i].Position = Position + new Vector3 (i * (HandWidth / max), 0, i);
			cards [i].name = name + i;
		}
	}
	
	public void InsertCard (Card c)
	{
		print ("insert: " + c.name);
		cards.Add (c);
		OrganizeCards ();
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
