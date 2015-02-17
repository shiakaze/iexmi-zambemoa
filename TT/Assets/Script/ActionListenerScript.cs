using UnityEngine;
using System.Collections;

public class ActionListenerScript : MonoBehaviour
{

	public string InputChannel = "Input";

	public string NetworkOutChannel {
		get { return NetworkController.DefaultNetworkOutChannel;	}
	}

	public string NetworkInChannel {
		get { return NetworkController.DefaultNetworkInChannel; }
	}

	private CustomEventStream CES { 
		get { return CustomEventStream.Instance; }
	}
	
	public static ActionListenerScript Instance;

	/// <summary>
	/// Legendary Rule
	/// </summary>
	void Awake ()
	{
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy (this);
		}
	}

	void Start ()
	{

		CustomEventStream.Instance.Spy (CustomEventMessageHandler);
	}

	void CustomEventMessageHandler (CustomEventMessage evnt)
	{

		string targetChannel = evnt.channelName;
		CustomEvent ce = evnt.content;
		if (targetChannel.Equals (InputChannel)) {
			HandleInputMessage (ce);
		}
		if (targetChannel.Equals (NetworkOutChannel)) {
			HandleNetworkOutMessage (ce);
		}
	}

	private void HandleInputMessage (CustomEvent ce)
	{
		CustomEventMessage action = InterpretInputEvent (ce);
		CustomEvent networkMessage = ToNetworkMessage (action);
		CES.Broadcast (networkMessage, NetworkInChannel);
	}

	private CustomEventMessage InterpretInputEvent (CustomEvent ce)
	{
		// Do some fancy stuff here
		return new CustomEventMessage (ce, "FANCYSTUFFS");
	}

	private CustomEvent ToNetworkMessage (CustomEventMessage cem)
	{
		CustomEvent mail = new ActionEvent ("Broadcast");
		mail ["Broadcast"] = cem;
		return mail;
	}


	/// <summary>
	/// Handles the network message.
	/// if message was sent over the network
	/// that means the input has already been processed and this is a rpc call to affect the game state
	/// </summary>
	/// <param name="ce">Ce.</param>
	private void HandleNetworkOutMessage (CustomEvent ce)
	{
		CustomEventMessage cem = (CustomEventMessage)ce ["NetworkMessage"];
		CustomEvent realMessage = cem.content;

		string targetChannel = cem.channelName;
		CES.Broadcast (realMessage, targetChannel);

	}
}
