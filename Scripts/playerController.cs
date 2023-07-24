using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScoredProductions.Global;
using TMPro;
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]

public class playerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private FixedJoystick _joystick;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _treeTime, _cutTime, _activationTime, _moveSpeed;
    [SerializeField] private GameObject _tree, _tempTree, _cuttedTree, _bullet, _wood, _log;
    [SerializeField] private Transform _treePos;
    [SerializeField] private bool _woodSpawned, _treeCut;
    [SerializeField] private ParticleSystem _particles, _dust;
    [SerializeField] private int _woodCount, _time = 0;
    [SerializeField] private TextMeshProUGUI _woodText;
    [SerializeField] private GameObject[] allDusts;
    [SerializeField] private AudioSource _step, _chop, _pickup;

    private void Start(){
        _treeTime = 15;
        _cutTime = 0;
        _treeCut = false;
        _activationTime = 3;
    }
    private void FixedUpdate(){
        if(!_animator.GetBool("isCutting") && !_animator.GetBool("isGathering")){
            if(_joystick.Vertical > 0.05f || _joystick.Horizontal > 0.05f || _joystick.Vertical < -0.05f || _joystick.Horizontal < -0.05f){
                _rigidbody.velocity = new Vector3(_joystick.Horizontal * _moveSpeed, _rigidbody.velocity.y, _joystick.Vertical * _moveSpeed);
                transform.rotation = Quaternion.LookRotation(_rigidbody.velocity);
                _animator.SetBool("isRunning", true);
            }
            else{
                _animator.SetBool("isRunning", false);
            }
        }
        if(_animator.GetBool("isCutting")){
            if(_cutTime == 3){
                _animator.SetBool("isCutting", false);
                _cutTime = 0;
                _woodSpawned = true;
                _treeCut = true;
            }
        }
        if(_treeCut){
            _treeTime -= Time.deltaTime;
        }
        if(_treeTime <= 0){
            _treePos.position = new Vector3(Random.Range(-5,8), 0.1665f, Random.Range(-5,8));
            _tempTree = Instantiate(_tree, _treePos);
            _treeTime = 15;
            _treeCut = false;
        }
        if(_woodSpawned && _activationTime > 0){
            _activationTime -= Time.deltaTime;
        }
    }
    private void FirstHit(){
        if(_time == 0){
            _particles.Play();
            foreach(GameObject dust in allDusts){
                dust.GetComponent<ParticleSystem>().Play();
            }
            _bullet.transform.localPosition = new Vector3(-1, 9, 0);
            _time += 1;
        }
        else if(_time == 1){
            _particles.Play();
            allDusts[1].GetComponent<ParticleSystem>().Stop();
            _bullet.transform.localPosition = new Vector3(1, 9, 0);
            _time += 1;
        }
        else{
            _particles.Play();
            allDusts[0].GetComponent<ParticleSystem>().Stop();
            allDusts[2].GetComponent<ParticleSystem>().Stop();
            _time = 0;
            _bullet.transform.localPosition = new Vector3(0, 2, 0);
            Instantiate(_wood, new Vector3(_cuttedTree.transform.position.x, 2, _cuttedTree.transform.position.z), Quaternion.Euler(0, 0, 0), null);
        }
    }

    private void OnTriggerEnter(Collider obj){
        if(obj.gameObject.tag == "Tree Host"){
            _cuttedTree = obj.transform.parent.gameObject;
            obj.tag = "Untagged";
            _bullet = GameObject.FindGameObjectWithTag("Bullet");
            allDusts = GameObject.FindGameObjectsWithTag("Dust");
            _particles = _cuttedTree.GetComponentInChildren<ParticleSystem>();
            _chop = _cuttedTree.GetComponentInChildren<AudioSource>();
            _animator.SetBool("isCutting", true);
            if(_cuttedTree) transform.LookAt(new Vector3(_cuttedTree.transform.position.x, 0,_cuttedTree.transform.position.z));
        }
        if(obj.gameObject.tag == "Finish"){
            transform.localPosition = new Vector3(0, 1, 0);
        }
    }
    private void OnCollisionEnter(Collision obj){
        if(obj.gameObject.tag == "log" && _activationTime <= 0){
            _animator.SetBool("isGathering", true);
            _log = obj.gameObject;
            transform.LookAt(new Vector3(_log.transform.position.x, 0, _log.transform.position.z));
        }
    }

    private void PlayStepSound(){
        int stepType = Random.Range(1, 5);
        switch(stepType){
            case 1:
                _step.clip = Resources.Load<AudioClip>("sounds/step1");
                break;
            case 2:
                _step.clip = Resources.Load<AudioClip>("sounds/step2");
                break;
            case 3:
                _step.clip = Resources.Load<AudioClip>("sounds/step3");
                break;
            case 4:
                _step.clip = Resources.Load<AudioClip>("sounds/step4");
                break;
        }
        _step.Play();
    }

    private void PlayChopSound(){
        int chopType = Random.Range(1, 5);
        switch(chopType){
            case 1:
                _chop.clip = Resources.Load<AudioClip>("sounds/chop1");
                break;
            case 2:
                _chop.clip = Resources.Load<AudioClip>("sounds/chop2");
                break;
            case 3:
                _chop.clip = Resources.Load<AudioClip>("sounds/chop3");
                break;
            case 4:
                _chop.clip = Resources.Load<AudioClip>("sounds/chop4");
                break;
        }
        _chop.Play();
    }

    private void ChopEnd(){
        _cutTime += 1;
    }

    private void gathered(){
        _pickup.Play();
        _woodCount += 1;
        _woodText.SetText("Wood: " + _woodCount);
        _activationTime = 3;
        _woodSpawned = false;
        Destroy(_log);
        _animator.SetBool("isGathering", false);
    }
}