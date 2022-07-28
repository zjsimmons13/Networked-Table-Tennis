using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DisconnectManager : NetworkBehaviour {
    
    [SerializeField] GameManager _gm;
    [SerializeField] TMPro.TextMeshProUGUI _disconnectMessage;
    [SerializeField] UnityEngine.UI.Image _backdrop;
    private bool _disconnectedPartner;
    
    void Awake() {
        NetworkManager.Singleton.OnClientDisconnectCallback += ((ulong id) => PartnerDisconnectedClientRPC());
        NetworkManager.Singleton.OnClientConnectedCallback += ((ulong id) => PartnerReconnectedClientRPC());
    }

    [ClientRpc]
    public void PartnerDisconnectedClientRPC() {
        _gm.PauseServerRpc();
        _disconnectMessage.text = "Other Player has disconnected.";
        _disconnectedPartner = true;
        _backdrop.enabled = true;
        _disconnectMessage.enabled = true;
    }

    [ClientRpc]
    public void PartnerReconnectedClientRPC() {
        if (!_disconnectedPartner) return;
        _gm.UnPauseServerRpc();
        _disconnectedPartner = false;
    }
}
