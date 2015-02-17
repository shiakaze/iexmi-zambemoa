using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomEvent
{
	public object this [string key] {
		get {
			foreach (KeyValuePair<string,object> kvp in kvplist) {
				if (key.Equals (kvp.Key)) {
					return kvp.Value;
				}
			}
			return null;
		}
		set {
			KeyValuePair<string,object> existingkvp = new KeyValuePair<string, object> (null, null);
			foreach (KeyValuePair<string,object> kvp in kvplist) {
				if (key.Equals (kvp.Key)) {
					existingkvp = kvp;
					break;
				}
			}
			if (existingkvp.Key != null) {
				kvplist.Remove (existingkvp);
			}
			kvplist.Add (new KeyValuePair<string, object> (key, value));
		}
	}
	
	public IEnumerator<KeyValuePair<string,object>> GetEnumerator ()
	{
		return kvplist.GetEnumerator ();
	}

	public List<KeyValuePair<string,object>> kvplist = new List<KeyValuePair<string,object>> ();

	public string Type {
		get {
			return (string)this ["Type"];
		}
		set {
			this ["Type"] = value;
		}
	}

	public CustomEvent ()
	{
		Type = "EVENT";
	}

	public CustomEvent (string type)
	{
		Type = type;
	}

	public bool ContainsKey (string key)
	{
		foreach (KeyValuePair<string, object> kvp in kvplist) {
			if (key.Equals (kvp.Key)) {
				return true;
			}
		}
		return false;
	}

	public bool Contains (params string[] keys)
	{
		for (int i = 0, max = keys.Length; i < max; ++i) {	
			if (!this.ContainsKey (keys [i])) {
				return false;
			}
		}
		return true;
	}
}

[Serializable]
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
	public ActionEvent():base("Action"){Action = "ACTION";}
}

[Serializable]
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
	public NotificationEvent():base("Notification"){Notification = "NOTIFICATION";}
}

[Serializable]
public class CustomEventMessage
{
	public CustomEvent content;
	public string channelName;

	public CustomEventMessage (CustomEvent content, string channelName)
	{
		this.content = content;
		this.channelName = channelName;
	}

	public CustomEventMessage ()
	{
		content = new CustomEvent();
		channelName = "";
	}
}

public delegate void CustomEventHandler (CustomEvent evnt);

public delegate void CustomEventMessageHandler (CustomEventMessage evnt);

public class CustomEventStream : MonoBehaviour
{
	public static CustomEventStream Instance;
	private CustomEventMessageHandler SpyNetwork;
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
		IEnumerator<KeyValuePair<string,object>> enumerator = evnt.GetEnumerator ();

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
		if (SpyNetwork != null) {
			SpyNetwork (new CustomEventMessage (evnt, channelName));
		}

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

	public void Spy (CustomEventMessageHandler handler)
	{
		SpyNetwork += handler;
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
