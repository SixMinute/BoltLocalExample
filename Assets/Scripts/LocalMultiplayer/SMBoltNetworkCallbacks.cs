using UnityEngine;
using System.Collections;
using System;

[BoltGlobalBehaviour]
public class SMBoltNetworkCallbacks : Bolt.GlobalEventListener
{
	public static event Action OnShutdownBegin;
	public static event Action OnShutdown;

	public override void BoltStartDone ()
	{
		base.BoltStartDone();

		UnityEngine.Debug.Log("bolt start done");

		BoltNetwork.EnableLanBroadcast();
	}

	public override void BoltShutdownBegin(Bolt.AddCallback registerDoneCallback)
	{
		base.BoltShutdownBegin(registerDoneCallback);

		UnityEngine.Debug.Log("bolt shutdown begin");

		try
		{
			BoltNetwork.DisableLanBroadcast();
		}
		catch(Exception e)
		{
			UnityEngine.Debug.LogException(e);
		}

		if(null != OnShutdownBegin)
		{
			OnShutdownBegin.Invoke();
			OnShutdownBegin = null;
		}

		registerDoneCallback(onShutdown);
	}

	protected void onShutdown()
	{
		if(null != OnShutdown)
		{
			OnShutdown.Invoke();
			OnShutdown = null;
		}
	}
}
