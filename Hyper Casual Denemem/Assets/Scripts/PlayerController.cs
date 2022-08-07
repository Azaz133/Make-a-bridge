using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;


    public float limitX;
    public float runningSpeed;
    public float xSpeed = 2f;
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;


    private bool _spawningBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;

    private bool _finished;
    private float _scoreTimer = 0;

    public Animator animator;

    private float _lastTouchedX;
    private float _dropSoundTimer;

    public AudioSource cylinderAudioSource,triggerAudioSource, itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip, coinAudioClip, buyAudioClip, equipItemAudioClip, unequipItemAudioClip;

    public List<GameObject> wearSpots;


    void Start()
    {
        Current = this;
    }
    
    void Update()
    {
        if (levelController.Current == null || !levelController.Current.gameActive)
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
        if (Input.touchCount > 0)
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;   
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchXDelta = 5 * (_lastTouchedX - Input.GetTouch(0).position.x) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            touchXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if (Input.GetMouseButton(0))
        {
            touchXDelta = Input.GetAxis("Mouse X");
        }
        newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentRunningSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (_spawningBridge)
        {
            PlayDropSound();
            _creatingBridgeTimer -= Time.deltaTime;
            if(_creatingBridgeTimer < 0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);
                GameObject createdBridgePieces = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.EndReference.transform.position - _bridgeSpawner.StartReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePieces.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.StartReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.StartReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePieces.transform.position = newPiecePosition;
                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if(_scoreTimer < 0)
                    {
                        _scoreTimer = 0.3f;
                        levelController.Current.ChangeScore(1);
                    }
                }
                
            }
        }


    }


    public void ChangeSpeed(float value)
    {
        _currentRunningSpeed = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip, 0.1f);
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }
        else if (other.tag == "SpawnBridge")
        {
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag == "StopSpawnBridge")
        {
            StopSpawningBridge();
            if (_finished)
            {
                levelController.Current.FinishGame();
            }
        }
        else if (other.tag == "Finish")
        {
            _finished = true;
            StartSpawningBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if(other.tag == "Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            levelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (levelController.Current.gameActive)
        {
            PlayDropSound();
            IncrementCylinderVolume(-Time.fixedDeltaTime);
        }
        if(other.tag == "Trap")
        {
            other.tag = "Untagged";
            PlayDropSound();
            IncrementCylinderVolume(-Time.fixedDeltaTime);
        }
    }

    public void IncrementCylinderVolume(float value)
    {
        if (cylinders.Count == 0)
        {
            if(value > 0)
            {
                CreateCylinder(value);
            }
            else
            {
                if (_finished)
                {
                    levelController.Current.FinishGame();
                }
                else
                {
                    D�e();
                }
            }
        }
        else 
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);
        }
    }



    public void D�e()
    {
        animator.SetBool("dead",true);
        gameObject.layer = 8;
        Camera.main.transform.SetParent(null);
        levelController.Current.GameOver();
    }

    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);
    }

    public void DestroyCylinder(RidingCylinder cylinder)
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    }

    public void StartSpawningBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;
    }

    public void StopSpawningBridge()
    {
        _spawningBridge = false;
    }

    public void PlayDropSound()
    {
        _dropSoundTimer -= Time.deltaTime;
        if(_dropSoundTimer < 0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);
        }
    }
}