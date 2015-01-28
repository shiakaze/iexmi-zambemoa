using UnityEngine;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	public void Update() {
		float send = Input.GetAxis("Horizontal");
		if(send != 0) {
			networkView.RPC("printText", RPCMode.All, send.ToString());
		}
	}

	// test rpc function
	[RPC]
	public void printText(string text) {
		Debug.Log(text);
	}
}
