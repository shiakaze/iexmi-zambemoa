using UnityEngine;
using System.Collections;

public class CUBE_OF_DESTINY : MonoBehaviour {

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
