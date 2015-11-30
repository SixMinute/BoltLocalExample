using UnityEngine;
using System.Collections;
using System;

[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class SMBoltClientCallbacks : Bolt.GlobalEventListener
{
	public static event Action<bool> OnConnectResult;

    private bool _continueSearching;
	private bool _shuttingDown;

    public override void BoltStartDone()
    {
        _continueSearching = true;

        StartCoroutine( startSearching() );
    }

	IEnumerator startSearching()
	{
		yield return null;
		int tries = 10;

		while(_continueSearching)
		{
			yield return new WaitForSeconds(1.0f);

			//if we have a session then auto-connect
			if(BoltNetwork.SessionList.Count > 0)
			{
				foreach(var s in BoltNetwork.SessionList)
				{
					BoltNetwork.Connect(s.Value.LanEndPoint);

					_continueSearching = false;
					yield break;
				}
			}

			tries--;

			UnityEngine.Debug.Log("search failed, num tries remaining: " + tries);
			if(0 == tries)
			{
				quitBolt();
				sendResult(false);

				_continueSearching = false;
				yield break;
			}
		}
	}

	public override void Disconnected(BoltConnection connection)
	{
		base.Disconnected(connection);

		quitBolt();
	}

	void quitBolt()
	{
		if(_shuttingDown)
		{
			return;
		}
		_shuttingDown = true;
		StartCoroutine( _quitBolt() );
	}

	IEnumerator _quitBolt()
	{
		yield return new WaitForSeconds(0.5f);

		try
		{
			BoltLauncher.Shutdown();
		}
		catch(Exception e)
		{
			UnityEngine.Debug.LogException(e);
		}
	}

    public override void ConnectFailed(UdpKit.UdpEndPoint endpoint, Bolt.IProtocolToken token)
    {
		base.ConnectFailed(endpoint, token);

        sendResult(false);
    }

    public override void Connected(BoltConnection connection)
	{
		base.Connected(connection);
		_shuttingDown = false;

        sendResult(true);
    }

    void sendResult(bool success)
    {
        if(null != OnConnectResult)
        {
            OnConnectResult.Invoke(success);
            OnConnectResult = null;
        }
    }
}
