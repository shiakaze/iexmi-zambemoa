using UnityEngine;
using System.Collections;

public class NetworkController : MonoBehaviour
{

	#region constants
	private const string DefaultNetworkChannel = "Network";

	#endregion
	#region private members

	private CustomEventStream CES;
	#endregion

	#region methods

	//create the network channel and have get a reference to the custom event stream
	void Start ()
	{
		CES = CustomEventStream.Instance;
		CES.CreateChannel ("Network");
	}
	//host a server
	public void Host (int port)
	{
		bool useNat = !Network.HavePublicAddress ();
		Network.InitializeServer (2, port, useNat);
	}
	//connect to an existing server
	public void Connect (string ip, int port = 69000)
	{
		Network.Connect (ip, port);
	}
	//send a message to all connected players
	public void SendAll (CustomEvent evnt, string channel = DefaultNetworkChannel)
	{
		networkView.RPC ("NetMessage", RPCMode.All, channel, evnt);
	}
	//send a message to all other connected players
	public void SendOthers (CustomEvent evnt, string channel = DefaultNetworkChannel)
	{
		networkView.RPC ("NetMessage", RPCMode.Others, channel, evnt);
	}
	//network send message
	[RPC]
	public void NetMessage (CustomEvent evnt, string channel)
	{
		CES.Broadcast (evnt, channel);
	}
	#endregion
}
