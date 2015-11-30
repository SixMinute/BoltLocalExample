using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class SMBoltServerCallbacks : Bolt.GlobalEventListener
{
	public static event Action OnStartDone;

    public override void BoltStartDone()
    {
		BoltNetwork.LoadScene("Lobby");

		sendStartResult();
	}

	void sendStartResult()
	{
		if(null != OnStartDone)
		{
			OnStartDone.Invoke();
			OnStartDone = null;
		}
	}
}
