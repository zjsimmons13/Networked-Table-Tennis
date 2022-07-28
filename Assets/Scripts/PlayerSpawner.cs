using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode{
    public class PlayerSpawner : NetworkBehaviour {
        [SerializeField] private GameObject playerPrefabA;
        [SerializeField] private GameObject playerPrefabB;
        [SerializeField] private GameManager _gm;
        private ulong _user1 = 0;
        private ulong _user2 = 0;

        void Awake() {
            NetworkManager.Singleton.OnClientDisconnectCallback += ((ulong id) => UnRegisterUser(id));
        }
    
        [ServerRpc(RequireOwnership=false)]
        public void SpawnPlayerServerRpc(ulong clientId) {
            GameObject newPlayer;
            if (_user1 == clientId){
                newPlayer=(GameObject)Instantiate(playerPrefabA);
                //_gm.gamePlayable.Value = true;
                NetworkObject netObj=newPlayer.GetComponent<NetworkObject>();
                newPlayer.SetActive(true);
                netObj.SpawnAsPlayerObject(clientId,true);
            }
            else if (_user2 == clientId) {
                newPlayer=(GameObject)Instantiate(playerPrefabB);
                NetworkObject netObj=newPlayer.GetComponent<NetworkObject>();
                newPlayer.SetActive(true);
                netObj.SpawnAsPlayerObject(clientId,true);
                _gm.gamePlayable.Value = true;
            }
        }

        [ServerRpc(RequireOwnership=false)]
        public void RegisterUserServerRpc(ulong clientID) {
            Debug.Log(clientID);
            Debug.Log("Register");
            if (_user1 == 0) _user1 = clientID;
            else if (_user2 == 0) _user2 = clientID;
        }

        
        public void UnRegisterUser(ulong clientID) {
            Debug.Log(clientID);
            Debug.Log("UnRegister");
            if (_user1 == clientID) _user1 = 0;
            else if (_user2 == clientID) _user2 = 0;
        }
        public override void OnNetworkSpawn() {
            RegisterUserServerRpc(NetworkManager.Singleton.LocalClientId);
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
}