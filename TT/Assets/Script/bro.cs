using UnityEngine;
using System.Collections;

public enum BroPriorityAttackType
{
	PlayerCharacter,
	FastestBroFirst,
	SlowestBroFirst
}

public class Bro : MonoBehaviour
{
	#region private members

	public int op;
	public int speed;
	private BroPriorityAttackType pbat;
	private int id;
	#endregion

	#region proporties
	/// <summary>
	/// Gets or sets the Id.
	/// id identifies the message recipient
	/// TODO: need to figure out a way to automatically do this and maybe have a hash function for these guys
	/// </summary>
	/// <value>The Id.</value>
	public int Id {
		get { return id; }
		set { id = value;}
	}

	/// <summary>
	/// Gets or sets the OPness.
	/// OPness is used to determine the damage this bro does.
	/// Taking damage will lower the OPness of the creature
	/// </summary>
	/// <value>The OPness.</value>
	public int OPness {
		get { return op; }
		set { op = value; }
	}

	/// <summary>
	/// Gets or sets the speed.
	/// Speed is used to determine who attacks first.
	/// In the case of a tied speed stat, the round master's bro goes first, then, lower OPness
	/// </summary>
	/// <value>The speed.</value>
	public int Speed {
		get { return speed; }
		set { speed = value; }
	}
	
	/// <summary>
	/// Gets or sets the attack priority.
	/// attack priority is used to determine what the bro will attack first, the fastest bro, the slowest bro, or the player character
	/// </summary>
	/// <value>The attack priority.</value>
	public BroPriorityAttackType AttackPriority {
		get { return pbat; }
		set { pbat = value; }
	}
	#endregion
	
	#region methods
	/// <summary>
	/// Send an attack message to specified targetId
	/// </summary>
	/// <param name="targetId">Target identifier.</param>
	public void Attack (int targetId)
	{
		ActionEvent dmg = new ActionEvent ("Damage");
		dmg ["TargetId"] = targetId;
		dmg ["DamageValue"] = OPness;
		CustomEventStream.Instance.Broadcast (dmg, "Bro");
	}

	public void Start ()
	{
		CustomEventStream.Instance.Subscribe (new CustomEventHandler (BroEventHandler), "Bro");
	}
	
	public void BroEventHandler (CustomEvent ce)
	{

		if ("Action".Equals(ce ["Type"]) && "Damage".Equals(ce ["Action"])) {
			if (id.Equals (ce ["TargetId"])) {
				damage ((int)ce ["DamageValue"]);
			}
		}
	}
	#endregion
	#region helper methods

	private void damage (int damageValue)
	{
		OPness -= damageValue;
	}
	#endregion
}



