using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform _orientation;
    public Transform _playerObj;
    private Rigidbody _rigidbody;
    private MovementScript _movementScript;
    [Header("Sliding")]
    public float _maxSlideTime;
    public float _slideForce;
    private float _slideTimer;
    public float _slideYScale;
    public float _startYScale;
    [Header("Input")]
    public KeyCode _slideKey = KeyCode.LeftControl;
    private float _horizontalInput;
    private float _verticalInput;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _movementScript = GetComponent<MovementScript>();
        _startYScale = _playerObj.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slideKey) && (_horizontalInput != 0 || _verticalInput != 0)) StartSlide();
        if (Input.GetKeyUp(_slideKey) && _movementScript.sliding) StopSlide();

    }
    private void FixedUpdate()
    {
        if (_movementScript.sliding) SlidingMovement();
    }
    private void StartSlide()
    {
        _movementScript.sliding = true;
        _playerObj.localScale = new Vector3(_playerObj.localScale.x, _slideYScale, _playerObj.localScale.z);
        _rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = _maxSlideTime * _movementScript.healthMultiplier;
        Debug.Log(_slideTimer);
    }
    private void SlidingMovement()
    {
        Vector3 inputDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        if(!_movementScript.OnSlope() || _rigidbody.velocity.y > -0.1f)
        {
            _rigidbody.AddForce(inputDirection.normalized * _slideForce * (_movementScript.healthMultiplier / (_movementScript.healthMultiplier * 10)), ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        else
        {
            _rigidbody.AddForce(_movementScript.GetSlopeMoveDirection(inputDirection).normalized * _slideForce * (_movementScript.healthMultiplier / (_movementScript.healthMultiplier * 10)), ForceMode.Force);

        }

        if (_slideTimer <= 0) StopSlide();
    }
    private void StopSlide()
    {
        _movementScript.sliding = false;
        _playerObj.localScale = new Vector3(_playerObj.localScale.x, _startYScale, _playerObj.localScale.z);
    }
}
