using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour {
    public NetworkVariable<bool> gamePlayable = new NetworkVariable<bool>(false);
    private bool _gameStarted = false;
    private bool _gameWon = false;
    private NetworkVariable<int> _score1 = new NetworkVariable<int>(0);
    private NetworkVariable<int> _score2 = new NetworkVariable<int>(0);
    [SerializeField] private BallController _bc;
    [SerializeField] private TMPro.TextMeshProUGUI _player1;
    [SerializeField] private TMPro.TextMeshProUGUI _player2;
    [SerializeField] private TMPro.TextMeshProUGUI _winScreen;
    [SerializeField] private UnityEngine.UI.Image _backdrop;

    public void Player1Score() {
        _score1.Value++;
        UpdateScoresClientRpc();
        WinCheckClientRpc();
    }

    public void Player2Score() {
        _score2.Value++;
        UpdateScoresClientRpc();
        WinCheckClientRpc();
    }
    [ClientRpc]
    void UpdateScoresClientRpc() {
        _winScreen.enabled = false;
        _backdrop.enabled = false;
        _gameWon = false;
        _player1.text = _score1.Value.ToString();
        _player2.text = _score2.Value.ToString();
    }
    [ClientRpc]
    void WinCheckClientRpc() {
        if (_score1.Value >= 7) {
            _winScreen.text = "Player 1 Wins \n \n Space Bar to Restart";
            _winScreen.enabled = true;
            _backdrop.enabled = true;
            _gameWon = true;
            _gameStarted = false;
        } 
        else if (_score2.Value >= 7) {
            _winScreen.text = "Player 2 Wins \n \n Space Bar to Restart";
            _winScreen.enabled = true;
            _backdrop.enabled = true;
            _gameWon = true;
            _gameStarted = false;
        }
        StartCoroutine(Reset());
    }
    IEnumerator Reset() {
        _bc.sprite.enabled = false;
        if (_gameWon) yield break;
        yield return new WaitForSeconds(0.5f);
        _bc.sprite.enabled = true;
        yield return new WaitForSeconds(1f);
        _bc.GameStartServerRpc();
    }

    public void OnSpace() {
        if(!gamePlayable.Value) return;
        _winScreen.enabled = false;
        _backdrop.enabled = false;
        if (_gameWon) {
            RestartServerRpc();
            _gameStarted = true;
            _gameWon = false;
        }
        else if (!_gameStarted) {
            RestartServerRpc();
            _gameStarted = true;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void RestartServerRpc() {
        _score1.Value = 0;
        _score2.Value = 0;
        UpdateScoresClientRpc();
        _bc.GameStart();
    }

    [ServerRpc(RequireOwnership=false)]
    public void PauseServerRpc() {
        _bc.Pause();
    }

    [ServerRpc(RequireOwnership=false)]
    public void UnPauseServerRpc() {
        _bc.UnPause();
        UpdateScoresClientRpc();
        UnPauseClientRpc();
    }

    [ClientRpc]
    public void UnPauseClientRpc() {
        _winScreen.enabled = false;
        _backdrop.enabled = false;
        _gameStarted = true;
    }

}
