using UnityEngine;
using System.Collections;

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
	}
}
