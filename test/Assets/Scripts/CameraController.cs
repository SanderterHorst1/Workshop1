using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 _offset;

    void Start()
    {
        // calculate the current offset by getting the distance between the camera and the player
        _offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        // set the position of the camera equal to the position of the player, but offset by a fixed amount
        transform.position = player.transform.position + _offset;
    }
}