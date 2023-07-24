using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 offset;
    private void Start(){
        offset = new Vector3(0, 6.5f, -4.75f);
    }
    private void Update(){
        transform.position = player.transform.position + offset;
    }
}
