using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MachineGun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Gun settings")]
    public bool isWorking = true;
    [Tooltip("Will determine accuracy of the turret")]
    public float trackerOffsetSecs = 0.25f;
    [Tooltip("Just to smooth out movement of the turret head")]
    public float rotationSmoothness = 1f; 
    private float _currentTrackerValue = 0;
    private float _memoryAngle = 0f;
    public float detectionRadius = 2f;
    private int _trackableLayerId; 
    
    public Transform shootingTransform;
    public Transform laserShootingTransform;
    public Transform shellEjectTransform; 
    
    public LineRenderer laserRenderer;
    private Vector3 _lastKnownDirection; 
    
    public float defaultDistance = 10;
    private bool hasIdentified = false;

    public float lockInTimeSecs = 3f;
    private float _currLockingTime = 0;

    private float _originalEndWidth;
    private Color _originalEndColor;

    private Vector3 _hitPoint;

    [Tooltip("Determins the colision box for boxcasting")]
    public Vector2 ultraSonicRadarSize = new Vector2(0.5f, 0.5f);

    [Header("Fire settings")] 
    public int numberOfBurstRounds = 3;
    public float delayBetweenRoundsSecs = 0.5f;
    public float bulletSpeed = 200; 

    private bool _isShooting = false;

    private static GameObject _prefabbedShellCasing;
    private static GameObject _prefabbedBulletTip;
    private static GameObject _muzzleFlash; 
    
    [Header("Shell casing forces")]
    public float casingForceMin = 1f; 
    public float casingForceMax = 5f; 
    public float rotationSpeedMin = 50f; 
    public float rotationSpeedMax = 200f; 
    
    [Header("Muzzle flash lifetime")]
    public float muzzleFlashLifetime = 0.1f;
    public Vector2 muzzleFlashScaleRange = new Vector2(0.7f, 1.3f);
    
    private void Awake()
    {
        if (!_prefabbedShellCasing) _prefabbedShellCasing = Resources.Load<GameObject>("Prefabs/MachineGun/ShellCasing");
        if (!_prefabbedBulletTip) _prefabbedBulletTip = Resources.Load<GameObject>("Prefabs/MachineGun/BulletTip"); 
        if(!_muzzleFlash) _muzzleFlash = Resources.Load<GameObject>("Prefabs/MachineGun/MuzzleFlashPrefab");
    }

    void Start()
    {
        
        laserRenderer = GetComponent<LineRenderer>();
        laserRenderer.enabled = true; 
        _trackableLayerId = LayerMask.GetMask("Trackable");
        _originalEndWidth = laserRenderer.endWidth;
        _originalEndColor = laserRenderer.endColor; 
    }

    // Update is called once per frame
    void Update()
    {

        if (_isShooting) return; 
        
        _currentTrackerValue += Time.deltaTime;
        // render laser towards player to let them know // about 5 seconds before the laser fully locks in 
        if (hasIdentified)
        {
            SpanLaser(laserShootingTransform.position, _hitPoint);  
            
        }
        else ClearLaserRender();
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, _memoryAngle, Time.deltaTime * rotationSmoothness);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);

    }

    private void ClearLaserRender()
    {
        laserRenderer.SetPosition(0, Vector3.zero);
        laserRenderer.SetPosition(1, Vector3.zero);
    }

    private void FixedUpdate()
    {


        if (_isShooting) return; 
        
        Collider2D detectedMovement = Physics2D.OverlapCircle(transform.position, detectionRadius, _trackableLayerId);
        
        if (detectedMovement && detectedMovement.CompareTag("Player") &&  isWorking && _currentTrackerValue >= trackerOffsetSecs)
        {
            
            transform.localScale = transform.position.x >= detectedMovement.transform.position.x ? new Vector3(-1, -1, 1) : new Vector3(-1, 1, 1);
            Vector3 direction = (detectedMovement.transform.position - transform.position).normalized;
            _lastKnownDirection = direction; 
            
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _memoryAngle = targetAngle; 
            

            _currentTrackerValue = 0; 
        }

        RaycastHit2D[] foundImpactPoints =  Physics2D.BoxCastAll(laserShootingTransform.position, ultraSonicRadarSize, 0, _lastKnownDirection);

        hasIdentified = false; 
        
        foreach (RaycastHit2D hit in foundImpactPoints)
        {
            if (hit.transform.CompareTag("Shield")) break;  
            if (hit.transform.name == "MainConnectionBone")
            {
                _hitPoint = hit.collider.transform.position;
                hasIdentified = true;
                _currLockingTime += Time.fixedDeltaTime;
                if (_currLockingTime >= lockInTimeSecs && !_isShooting)
                {
                    StartCoroutine(OpenFire()); 
                }
            }
            
        }

        if (!hasIdentified)
        {
            _currLockingTime = 0; 
        }

        laserRenderer.endWidth = _originalEndWidth * (1f - _currLockingTime / lockInTimeSecs); 
        
    }

    private IEnumerator OpenFire()
    {
        _isShooting = true;
        ClearLaserRender();
        hasIdentified = false; 

        for(int i = 0; i < numberOfBurstRounds; i++)
        {
            GameObject bullet = Instantiate(_prefabbedBulletTip, shootingTransform.position, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = (_hitPoint - shootingTransform.position).normalized;
                rb.linearVelocity = direction * bulletSpeed;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
            SpawnMuzzleFlash();
            
            GameObject casing = Instantiate(_prefabbedShellCasing, laserShootingTransform.position, Quaternion.identity);
            Rigidbody2D shellRb = casing.GetComponent<Rigidbody2D>();

            if (shellRb != null)
            {
                Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                float force = Random.Range(casingForceMin, casingForceMax);
                shellRb.AddForce(randomDirection * force, ForceMode2D.Impulse);

                float rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
                shellRb.angularVelocity = rotationSpeed;

            }
            
            yield return new WaitForSeconds(delayBetweenRoundsSecs);
        }
        
        _currLockingTime = 0;
        _isShooting = false;
    }


    private void SpanLaser(Vector3 startPos, Vector3 endPos)
    {
        laserRenderer.SetPosition(0, startPos);
        laserRenderer.SetPosition(1, endPos);
    }

    private void SpawnMuzzleFlash()
    {
        GameObject flash = Instantiate(_muzzleFlash, shellEjectTransform.position, transform.rotation);
        //float randomScale = Random.Range(muzzleFlashScaleRange.x, muzzleFlashScaleRange.y);
        //flash.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        Destroy(flash, muzzleFlashLifetime);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


}
