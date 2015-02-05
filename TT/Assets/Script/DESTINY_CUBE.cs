using UnityEngine;
using System.Collections;

public class DESTINY_CUBE : MonoBehaviour {

	// Use this for initialization
	void Start () {
		CustomEventStream.Instance.Subscribe(new CustomEventHandler(CubeEventHandler),"Cube");
	}

	void CubeEventHandler(CustomEvent ce) {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
