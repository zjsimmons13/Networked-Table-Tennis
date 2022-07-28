using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BallController : NetworkBehaviour {
    private Rigidbody2D _rb;
    private Transform _ball;
    public SpriteRenderer sprite;
    private bool _gameOccurring = false;
    private Vector2 _pauseVelocity;
    [SerializeField] private GameManager _gm;
    [SerializeField] private float _speed = 3;
    [SerializeField] TMPro.TextMeshProUGUI _winScreen;
    
    void Awake() {
        _rb = GetComponent<Rigidbody2D>();
        _ball = GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void GameStartServerRpc() {
        if (_gameOccurring) return;
        int option = Random.Range(0, 4);
        switch (option){
            case 0:
                _rb.velocity = new Vector2(1, 1) * _speed;
                break;
            case 1:
                _rb.velocity = new Vector2(-1, 1) * _speed;
                break;
            case 2:
                _rb.velocity = new Vector2(1, -1) * _speed;
                break;
            case 3:
                _rb.velocity = new Vector2(-1, -1) * _speed;
                break;
            default:
                Debug.Log("Default case reached.");
                break;
        }
        _gameOccurring = true;
    }

    public void GameStart() {
        if (_gameOccurring) return;
        int option = Random.Range(0, 4);
        switch (option){
            case 0:
                _rb.velocity = new Vector2(1, 1) * _speed;
                break;
            case 1:
                _rb.velocity = new Vector2(-1, 1) * _speed;
                break;
            case 2:
                _rb.velocity = new Vector2(1, -1) * _speed;
                break;
            case 3:
                _rb.velocity = new Vector2(-1, -1) * _speed;
                break;
            default:
                Debug.Log("Default case reached.");
                break;
        }
        _gameOccurring = true;
    }
    void OnTriggerEnter2D(Collider2D col) {
        if (NetworkManager.Singleton.IsClient) return;
        string whichGoal = col.gameObject.name;
        if (whichGoal == "Player1 Goal") _gm.Player1Score();
        else _gm.Player2Score();
        _gameOccurring = false;
        _rb.velocity = Vector2.zero;
        sprite.enabled = false;
        _rb.position = Vector2.zero;
    }

    public void Pause() {
        _pauseVelocity = _rb.velocity;
        _rb.velocity = Vector2.zero;
    }

    public void UnPause() {
        _rb.velocity = _pauseVelocity;
        if (_pauseVelocity == Vector2.zero) GameStart();
    }
}
