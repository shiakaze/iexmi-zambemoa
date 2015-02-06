using UnityEngine;
using System.Collections;

/// <summary>
/// testing object for network
/// </summary>
public class DESTINY_CUBE : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		CustomEventStream.Instance.Subscribe (new CustomEventHandler (CubeEventHandler), "Cube");
		CustomEvent ce = new ActionEvent ("now");
		string save = CustomEvent.ToString (ce);
		CustomEvent newce = CustomEvent.FromString (save);
		Debug.Log (CustomEvent.ToString (newce));
	}

	void CubeEventHandler (CustomEvent ce)
	{
		CustomEventStream.Instance.Unsubscribe(new CustomEventHandler(CubeEventHandler),"Cube");
		Debug.Log("was told to sudoku");
		Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
