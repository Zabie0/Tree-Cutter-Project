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
    [SerializeField] private float _activationTime, _moveSpeed;
    [SerializeField] private GameObject _tree, _tempTree, _cuttedTree, _bullet, _wood, _log;
    [SerializeField] private Transform _treePos;
    [SerializeField] private bool _woodSpawned;
    [SerializeField] private ParticleSystem _particles, _dust;
    [SerializeField] private int _woodCount, _timeNeeded = 0, _cutTime;
    [SerializeField] private TextMeshProUGUI _woodText;
    [SerializeField] private GameObject[] allDusts;
    [SerializeField] private AudioSource _step, _chop, _pickup;
    [SerializeField] private List<GameObject> _branchList;
    [SerializeField] private List<Transform> _hitList;

    private void Start(){
        _cutTime = 0;
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
            if(_cutTime == _timeNeeded){
                _animator.SetBool("isCutting", false);
                _cutTime = 0;
                _timeNeeded = 0;
                _woodSpawned = true;
                _cuttedTree.transform.parent.GetComponent<treeSpawner>().isCut = true;
                Instantiate(_wood, new Vector3(_cuttedTree.transform.position.x, 2, _cuttedTree.transform.position.z), Quaternion.Euler(0, 0, 0), null);
            }
        }
        if(_woodSpawned && _activationTime > 0){
            _activationTime -= Time.deltaTime;
        }
    }

    private void TreeHit(){
        _particles.Play();
        _bullet.transform.localPosition = _hitList[_cutTime].localPosition;
        Debug.Log(_hitList[_cutTime].name);
    }

    private void OnTriggerEnter(Collider obj){
        if(obj.gameObject.tag == "Tree Host"){
            _cuttedTree = obj.transform.gameObject;
            countTime();
            _cuttedTree.GetComponent<Collider>().enabled = false;
            //obj.tag = "Untagged";
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
        _step.clip = Resources.Load<AudioClip>("sounds/step" + stepType);
        _step.Play();
    }

    private void PlayChopSound(){
        int chopType = Random.Range(1, 5);
        _chop.clip = Resources.Load<AudioClip>("sounds/chop" + chopType);
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

    private void countTime(){
        _branchList = new List<GameObject>();
        _hitList = new List<Transform>();
        Transform t = _cuttedTree.transform;
        foreach(Transform tr in t){
            if(tr.tag == "branch"){
                _branchList.Add(tr.gameObject);
                _timeNeeded += 1;
            }
            else if(tr.tag == "hit"){
                _hitList.Add(tr);
            }
        }
    }
}