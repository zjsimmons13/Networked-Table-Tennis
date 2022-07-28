using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour {
    public NetworkVariable<bool> gamePlayable = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> _gameStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<int> _score1 = new NetworkVariable<int>(0);
    private NetworkVariable<int> _score2 = new NetworkVariable<int>(0);
    private bool _gameWon = false;
    private Coroutine _reset;
    [SerializeField] private BallController _bc;
    [SerializeField] private TMPro.TextMeshProUGUI _player1;
    [SerializeField] private TMPro.TextMeshProUGUI _player2;
    [SerializeField] private TMPro.TextMeshProUGUI _winScreen;
    [SerializeField] private UnityEngine.UI.Image _backdrop;

    void Start() {
        _score1.OnValueChanged += ((int prev, int curr) => UpdateScores());
        _score2.OnValueChanged += ((int prev, int curr) => UpdateScores());
        _gameStarted.OnValueChanged += ((bool prev, bool curr) => BeginGame());
    }

    public void Player1Score() {
        _score1.Value++;
        WinCheck();
    }

    public void Player2Score() {
        _score2.Value++;
        WinCheck();
    }
    public void UpdateScores() {
        if (NetworkManager.Singleton.IsServer) return;
        _player1.text = _score1.Value.ToString();
        _player2.text = _score2.Value.ToString();
    }


    void WinCheck() {
        if (_score1.Value >= 7) {
            _gameStarted.Value = false;
            WinClientRpc(true);
            return;
        } 
        else if (_score2.Value >= 7) {
            _gameStarted.Value = false;
            WinClientRpc(false);
            return;
        }
        ContinueClientRpc();
    }

    [ClientRpc]
    public void WinClientRpc(bool player1) {
        if (player1) {
            _winScreen.text = "Player 1 Wins \n \n Space Bar to Restart";
            _winScreen.enabled = true;
            _backdrop.enabled = true;
            _gameWon = true;
        }
        else {
            _winScreen.text = "Player 2 Wins \n \n Space Bar to Restart";
            _winScreen.enabled = true;
            _backdrop.enabled = true;
            _gameWon = true;
        }
    }
    [ClientRpc]
    public void ContinueClientRpc() {
        _reset = StartCoroutine(Reset());
    }
    public void BeginGame() {
        if (NetworkManager.Singleton.IsServer) return;
        if (!_gameStarted.Value) return;
        _reset = StartCoroutine(Reset());
    }
    IEnumerator Reset() {
        if (!_gameStarted.Value) yield break;
        _winScreen.enabled = false;
        _backdrop.enabled = false;
        _gameWon = false;
        _bc.sprite.enabled = false;
        yield return new WaitForSeconds(0.5f);
        _bc.sprite.enabled = true;
        yield return new WaitForSeconds(1f);
        _bc.PointStartServerRpc();
    }

    [ClientRpc]
    public void StopResetClientRpc() {
        StopCoroutine(_reset);
    }

    public void OnSpace() {
        if(!gamePlayable.Value) return;
        _winScreen.enabled = false;
        _backdrop.enabled = false;
        if (_gameWon) {
            RestartServerRpc();
            UpdateScores();
            StartGameServerRpc();
            _gameWon = false;
        }
        else if (!_gameStarted.Value) StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void StartGameServerRpc() {
        _gameStarted.Value = true;
    }

    [ServerRpc(RequireOwnership=false)]
    public void RestartServerRpc() {
        RestartServer();
    }
    public void RestartServer() {
        _score1.Value = 0;
        _score2.Value = 0;
        _gameStarted.Value = false;
        _bc.Reset();
    }

    [ServerRpc(RequireOwnership=false)]
    public void PauseServerRpc() {
        _bc.Pause();
        StopResetClientRpc();
    }

    [ServerRpc(RequireOwnership=false)]
    public void UnPauseServerRpc() {
        if (NetworkManager.Singleton.ConnectedClients.Count < 2) return;
        _bc.UnPause();
        UnPauseClientRpc();
    }

    [ClientRpc]
    public void UnPauseClientRpc() {
        _winScreen.enabled = false;
        _backdrop.enabled = false;
    }

}
