using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] bool _enable = true;

    [SerializeField, Range(0, 0.01f)] float _amplitude = 0.003f;
    [SerializeField, Range(0, 30f)] float _frequency = 10.0f;

    [SerializeField] Transform _camera;
    [SerializeField] Transform _cameraHolder;

    float _toggleSpeed = 1.5f;
    Vector3 _startPos;
    CharacterController _controller;

    float _focusPointDistance = 30.0f;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _startPos = _camera.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_enable) return;

        CheckMotion();
        ResetPosition();
        _camera.LookAt(FocusTarget());
    }

    void CheckMotion()
    {
        float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;
        if (speed < _toggleSpeed) return;
        if (!_controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    void PlayMotion(Vector3 motion)
    {
        _camera.localPosition += motion;
    }

    Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency / 2) * _amplitude * 2;
        return pos;
    }

    void ResetPosition()
    {
        if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition, _startPos, 1 * Time.deltaTime);
    }

    Vector3 FocusTarget()
    {
        Vector3 pos = new(transform.position.x, transform.position.y + _cameraHolder.localPosition.y, transform.position.z);
        pos += _cameraHolder.forward * _focusPointDistance;
        return pos;
    }
}
