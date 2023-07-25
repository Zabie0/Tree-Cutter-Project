using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _treePrefab;
    [SerializeField] public bool isCut;
    [SerializeField] private float _treeRegenTime;
    private void Start(){
        Instantiate(Resources.Load<GameObject>("models/Tree_test" + Random.Range(1, 3)), transform);
        _treeRegenTime = 15;
    }
    private void Update(){
        if(isCut){
            _treeRegenTime -= Time.deltaTime;
        }
        if(_treeRegenTime <= 0){
            isCut = false;
            transform.position = new Vector3(Random.Range(-5,8), 0, Random.Range(-5,8));
            _treePrefab = Resources.Load<GameObject>("models/Tree_test" + Random.Range(1, 3));
            Instantiate(_treePrefab, transform);
            _treeRegenTime = 15;
        }
    }
}
