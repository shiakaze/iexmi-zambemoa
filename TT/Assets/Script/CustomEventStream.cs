using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

public class CustomEvent : Dictionary<string, object>
{
	public string Type {
		get {
			return (string)this ["Type"];
		}
		set {
			this ["Type"] = value;
		}
	}

	public CustomEvent (string type)
	{
		Type = type;
	}

	public bool Contains (params string[] keys)
	{
		for (int i = 0, max = keys.Length; i < max; ++i) {
			if (this.ContainsKey (keys [i]) == false) {
				return false;
			}
		}
		return true;
	}
	/// <summary>
	/// convert the string to a custom event.
	/// </summary>
	/// <returns>The custom event.</returns>
	/// <param name="str">string to convert.</param>
	public static CustomEvent FromString (string str)
	{
		int strindex = 0;
		char now = str [strindex];
		StringBuilder sb = new StringBuilder ();
		CustomEvent ce = new CustomEvent ("Unknown");
		string key = null;

		while (now != ']') {
			if (now == ';') {
				ce [key] = sb.ToString ();
				sb = new StringBuilder ();
			} else if (now == ':') {
				key = sb.ToString ();
				sb = new StringBuilder ();
			} else {
				sb.Append (str [strindex]);
			}
			now = str [++strindex];
		}
		return ce;
	}
	/// <summary>
	/// Connvert the event to a string
	/// : to split key value pair
	/// ; to denotate the end of the key value pair
	/// ] to denotate the end of the string
	/// </summary>
	/// <returns>The string in the format of "key:value;key:value]"</returns>
	/// <param name="ce">custom event to convert.</param>
	public static string ToString (CustomEvent ce)
	{
		StringBuilder sb = new StringBuilder ();
		foreach (KeyValuePair<string,object>kvp in ce) {
			sb.Append (kvp.Key);
			sb.Append (':');
			sb.Append (kvp.Value);
			sb.Append (';');
		}
		sb.Append ("]");
		return sb.ToString ();
	}
}

public class ActionEvent : CustomEvent
{
	public string Action {
		get {
			return (string)this ["Action"];
		}
		set {
			this ["Action"] = value;
		}
	}
	
	public ActionEvent (string action)
		: base("Action")
	{
		Action = action;
	}
}

public class NotificationEvent : CustomEvent
{
	public string Notification {
		get {
			return (string)this ["Notification"];
		}
		set {
			this ["Notification"] = value;
		}
	}
	
	public NotificationEvent (string notification)
		: base("Notification")
	{
		Notification = notification;
	}
}

public delegate void CustomEventHandler (CustomEvent evnt);

public class CustomEventStream : MonoBehaviour
{
	public static CustomEventStream Instance;
	public bool LogEvents = true;
	private Dictionary<string, CustomEventHandler> Channels;
	private const string DefaultChannel = "Main";

	private void Awake ()
	{
		if (Instance != null) {
			Destroy (this);
		} else {
			Instance = this;
			DontDestroyOnLoad (this);

			Initiate ();
		}
	}

	private void Initiate ()
	{
		Channels = new Dictionary<string, CustomEventHandler> ();
		CreateChannel (DefaultChannel);
		Channels [DefaultChannel] += MainEventHandler;
	}

	private void MainEventHandler (CustomEvent evnt)
	{
		CustomEvent.Enumerator enumerator = evnt.GetEnumerator ();

		StringBuilder builder = new StringBuilder ();
		while (enumerator.MoveNext()) {
			KeyValuePair<string, object> current = enumerator.Current;
			string key = current.Key;
			object obj = current.Value;

			builder.Append ("[").Append (key).Append (": ").Append (obj.ToString ()).Append ("]\n");
		}
		if (LogEvents) {
			Debug.Log (builder.ToString ());
		}
	}

	public void Broadcast (CustomEvent evnt, string channelName = DefaultChannel)
	{
#if UNITY_EDITOR
		if (!Channels.ContainsKey (channelName)) {
			throw new UnityException ("EventStream does not contain channel " + channelName);
		}
#endif
		if (Channels [channelName] != null) {
			Channels [channelName] (evnt);
		}
	}

	/// <summary>
	/// Subscribe the specified handler and channelName.
	/// 
	/// This can be improved by having custom option like subscribe only to action event or notification event, 
	/// so that other ends don't need to do any checking (like type == action those boring stuffs)
	/// </summary>
	/// <param name="handler">Handler.</param>
	/// <param name="channelName">Channel name.</param>
	public bool Subscribe (CustomEventHandler handler, string channelName = DefaultChannel, bool createIfNotExist = false)
	{
		if (createIfNotExist) {
			bool result = (Channels.ContainsKey (channelName));
			if (result) {
				Channels [channelName] += handler;
			}
			return result;
		} else {
			bool result = (Channels.ContainsKey (channelName));
			if (result == false) {
				CreateChannel (channelName);
			}
			Channels [channelName] += handler;

			return true;
		}
	}

	public bool Unsubscribe (CustomEventHandler handler, string channelName = DefaultChannel)
	{
		bool result = (Channels.ContainsKey (channelName));
		if (result) {
			Channels [channelName] -= handler;
		}
		return result;
	}

	public void CreateChannel (string name)
	{
		if (!Channels.ContainsKey (name)) {
			Channels [name] = null;
		}
	}

	public void CloseChannel (string name)
	{
		if (Channels.ContainsKey (name)) {
			Channels.Remove (name);
		}
	}
}
