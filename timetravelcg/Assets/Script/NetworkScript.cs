using UnityEngine;
using System.Collections;

public class NetworkScript : MonoBehaviour {

	public Transform testObj;

	void Start() {
		bool useNat = !Network.HavePublicAddress();
		NetworkConnectionError result = Network.InitializeServer(32, 60000, useNat);
		if (result != NetworkConnectionError.NoError) {
			Network.Connect("127.0.0.1", 60000);
			Debug.Log("after connection");
			Debug.Log(Network.connections.ToString());
		}
	}

	void Update() {
		float send = Input.GetAxis("Horizontal");
		if(send != 0) {
			networkView.RPC("moveThatCube", RPCMode.OthersBuffered, send);
			Debug.Log("Moving the other cube");
		}
	}
	
	// test rpc function
	[RPC]
	public void moveThatCube(float movement) {
		testObj.Translate(movement,0,0);
	}
}
