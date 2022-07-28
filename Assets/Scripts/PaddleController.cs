using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PaddleController : NetworkBehaviour {
    private float _direction;
    private ulong _clientId;

    public void OnMove(InputValue value) {
        _direction = value.Get<float>();
    }

    void FixedUpdate() {
        MoveServerRpc(_direction, _clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveServerRpc(float direction, ulong clientID) {
        Transform _player = NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.transform;
        _player.position = new Vector2(_player.position.x, Mathf.Clamp((_player.position.y + (direction/4)), -4f, 4f));
    }

    public override void OnNetworkSpawn() {
        _clientId = NetworkManager.Singleton.LocalClientId;
    }
}
