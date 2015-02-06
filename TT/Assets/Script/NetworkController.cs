using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour
{

	#region constants
	private const string DefaultNetworkChannel = "Network";
	private const int DefaultPort = 6900;

	#endregion
	#region private members

	private CustomEventStream CES;
	#endregion

	#region methods

	void Update() {
		float horiz = Input.GetAxis("Horizontal");
		if (horiz > 0){
			Debug.Log("telling other guy to kill the cube of destiny");
			SendOthers(new CustomEvent("Action"),"Cube");
		}
	}
	//create the network channel and have get a reference to the custom event stream
	void Start ()
	{
		CES = CustomEventStream.Instance;
		CES.CreateChannel (DefaultNetworkChannel);
		CES.Subscribe (NetEventHandler, DefaultNetworkChannel);

		Debug.Log("trying hosting");
		NetworkConnectionError nce = Host();
		if(nce == NetworkConnectionError.CreateSocketOrThreadFailure){
			Debug.Log("host failed; assuming already exists. connecting");
			Connect("192.168.0.103");
		} else {
			Debug.Log("success");
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
	/// </summary>
	/// <param name="ip">Ip.</param>
	/// <param name="port">Port.</param>
	public void Connect (string ip, int port = DefaultPort)
	{
		Network.Connect (ip, port);
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
			Debug.Log ("Local server connection disconnected");
		} else {
			if (info == NetworkDisconnection.LostConnection) {
				Debug.Log ("Lost connection to the server");
			} else {
				Debug.Log ("Successfully diconnected from the server");
			}
		}
	
	}

	/// <summary>
	/// Called when unable to connect to server
	/// </summary>
	/// <param name="error">reason why connection failed.</param>
	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log("Could not connect to server: " + error);
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
		string toSend = CustomEvent.ToString(evnt);
		networkView.RPC ("NetMessage", RPCMode.All, toSend, channel);
	}
	//send a message to all other connected players
	public void SendOthers (CustomEvent evnt, string channel = DefaultNetworkChannel)
	{
		string toSend = CustomEvent.ToString(evnt);
		networkView.RPC ("NetMessage", RPCMode.Others, toSend, channel);
	}
	//network send message
	[RPC]
	public void NetMessage (string evnt, string channel)
	{
		CustomEvent ce = CustomEvent.FromString(evnt);
		CES.Broadcast (ce, channel);
	}
	#endregion
}
