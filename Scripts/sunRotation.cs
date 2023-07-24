using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sunRotation : MonoBehaviour
{
    [SerializeField] private Vector3 _rotation;
    [SerializeField] private AudioSource _fire;
    [SerializeField] private bool _isNight;
    [SerializeField] private ParticleSystem _torch;
    void Start(){
        _rotation = new Vector3(10, 1, 1);
        _isNight = true;
    }
    void Update(){
        transform.Rotate(_rotation * Time.deltaTime);
        if(transform.localRotation.eulerAngles.x > 180 || transform.localRotation.eulerAngles.x < 0){
            if (!_isNight){
                _fire.Play();
                _torch.Play();
                _isNight = true;
            }
        }
        else if(_isNight){
            _fire.Stop();
            _torch.Stop();
            _isNight = false;
        }
    }
}
