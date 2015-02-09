using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour
{

	#region constants
	private const string DefaultNetworkChannel = "Network";
	private const string DefaultIp = "127.0.0.1";
	private const int DefaultPort = 6900;

	#endregion
	#region private members

	private int retryCount = 0;
	private string lastIp = DefaultIp;
	private int lastPort = DefaultPort;
	private CustomEventStream CES;
	#endregion

	#region methods

//	void Update() {
//		float horiz = Input.GetAxis("Horizontal");
//		if (horiz > 0){
//			Debug.Log("telling other guy to kill the cube of destiny");
//			SendOthers(new CustomEvent("Action"),"Cube");
//		}
//	}


	//create the network channel and have get a reference to the custom event stream
	void Start ()
	{
		CES = CustomEventStream.Instance;
		CES.CreateChannel (DefaultNetworkChannel);
		CES.Subscribe (NetEventHandler, DefaultNetworkChannel);

		Debug.Log ("trying hosting");
		NetworkConnectionError nce = Host ();
		if (nce == NetworkConnectionError.CreateSocketOrThreadFailure) {
			Debug.Log ("host failed; assuming already exists. connecting");
			Connect (DefaultIp);
		} else {
			Debug.Log ("success");
		}
	}

	/// <summary>
	/// Host a server at the specified port
	/// </summary>
	/// <param name="port">Port.</param>
	public NetworkConnectionError Host (int port = DefaultPort)
	{
		bool useNat = !Network.HavePublicAddress ();
		return Network.InitializeServer (2, port, useNat);
	}

	/// <summary>
	/// Connect to the specified ip and port.
	/// port defaults to DefaultPort
	/// </summary>
	/// <param name="ip">Ip.</param>
	/// <param name="port">Port.</param>
	public void Connect (string ip, int port = DefaultPort)
	{
		lastPort = port;
		lastIp = ip;
		Network.Connect (lastIp, lastPort);
	}

	/// <summary>
	/// connect using the previous connection parameters
	/// </summary>
	public void Connect ()
	{
		Network.Connect (lastIp, lastPort);
	}

	/// <summary>
	/// Disconnect from the server.
	/// </summary>
	public void Disconnect ()
	{
		Network.Disconnect ();
	}

	/// <summary>
	/// Called by the client when disconnected from the server.
	/// Called by the server when connection is disconnected.
	/// </summary>
	/// <param name="info">if the network was dropped or disconnected</param>
	public void OnDisconnectedFromServer (NetworkDisconnection info)
	{
		if (Network.isServer) {
			//at this point the internet has cut out for the server or the server has shut down
			Debug.Log ("Local server connection disconnected");
		} else {
			if (info == NetworkDisconnection.LostConnection) {
				Debug.Log ("Lost connection to the server, trying to connect now");
				Connect ();
			} else {
				Debug.Log ("Successfully diconnected from the server");
			}
		}
	}

	/// <summary>
	/// Called when unable to connect to server
	/// </summary>
	/// <param name="error">reason why connection failed.</param>
	void OnFailedToConnect (NetworkConnectionError error)
	{
		switch (error) {
		// the internet is cruddy, going to try 3 more times
		case NetworkConnectionError.ConnectionFailed:
			if (retryCount++ < 3) {
				Debug.Log ("connection failed, attempt #" + retryCount + "...");
				Connect ();
			} else {
				Debug.Log ("connection failed too many times");
			}
			break;
		// you are already connected to someone else, d/c then reconnect
		case NetworkConnectionError.AlreadyConnectedToServer:
		case NetworkConnectionError.AlreadyConnectedToAnotherServer:
			Debug.Log ("already connection already in use, disconnecting");
			Disconnect ();
			Debug.Log ("Connecting...");
			Connect ();
			break;
		}
		Debug.Log ("Could not connect to server: " + error);
	}


	/// <summary>
	/// called when the client is connected to the server successfully
	/// </summary>
	void OnConnectedToServer ()
	{
		Debug.Log ("Connected to server");
	}

	/// <summary>
	/// handles events relevent to the network.
	/// </summary>
	/// <param name="ce">event passed in</param>
	public void NetEventHandler (CustomEvent ce)
	{
		if ("Notification".Equals (ce ["Type"])) {
			if ("Disconnect".Equals (ce ["Notification"])) {
				
			}
		}
	}


	//send a message to all connected players
	public void SendAll (CustomEvent evnt, string channel = DefaultNetworkChannel)
	{
		string toSend = CustomEvent.ToString (evnt);
		networkView.RPC ("NetMessage", RPCMode.All, toSend, channel);
	}
	//send a message to all other connected players
	public void SendOthers (CustomEvent evnt, string channel = DefaultNetworkChannel)
	{
		string toSend = CustomEvent.ToString (evnt);
		networkView.RPC ("NetMessage", RPCMode.Others, toSend, channel);
	}
	//network send message
	[RPC]
	public void NetMessage (string evnt, string channel)
	{
		CustomEvent ce = CustomEvent.FromString (evnt);
		CES.Broadcast (ce, channel);
	}
	#endregion
}
