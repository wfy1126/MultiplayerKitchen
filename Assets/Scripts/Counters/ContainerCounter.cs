using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter {


    public event EventHandler OnPlayerGrabbedObject;


    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) {
            // Player is not carrying anything
            KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            InteractLogicServerRpcInteractLogicServerRpc();


        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpcInteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }


    [ClientRpc]
    private void InteractLogicClientRpc() {
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }

}