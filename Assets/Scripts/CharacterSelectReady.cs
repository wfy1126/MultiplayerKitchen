using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{

    public static CharacterSelectReady Instance {  get; private set; }

    private Dictionary<ulong, bool> playerReadyDictory;

    public event EventHandler OnReadyChanged;

    private void Awake()
    {
        Instance = this;
        playerReadyDictory = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictory[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictory.ContainsKey(clientId) || playerReadyDictory[clientId] == false)
            {
                allClientReady = false;
                break;
            }
        }

        if (allClientReady)
        {
            KitchenGameLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }

    }


    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientID)
    {
        playerReadyDictory[clientID] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID)
    {
        return playerReadyDictory.ContainsKey(clientID) && playerReadyDictory[clientID];
    }
}
