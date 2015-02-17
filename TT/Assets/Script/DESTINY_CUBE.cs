using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// testing object for network
/// </summary>
public class DESTINY_CUBE : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		CustomEventStream.Instance.Subscribe (new CustomEventHandler (CubeEventHandler), "Void");
	}

	void CubeEventHandler (CustomEvent ce)
	{
		Debug.Log("ce['Action']=" + ce["Action"]);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
