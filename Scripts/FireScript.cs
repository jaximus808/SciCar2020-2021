using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    public GameObject FireParent;

    public Vector3 envPos;

    [SerializeField] private GameObject firePrefabs;
    [SerializeField] private GameObject fireColliders;

    [SerializeField] private float maxSpreadRange;

    [SerializeField] private float maxSpreadRate;
    [SerializeField] private float minSpreadRate;
    [SerializeField] private float minLifeTime;
    [SerializeField] private float maxLifeTime;
    [SerializeField] private float amountOfFirePrefabs;

    [TextArea] public string Notes = "Comment Here."; 
    

    private bool spread = false;
    private float spreadRate;
    private float lifeTime;
    private float curernt;
    private float currentLife;

    private void Awake()
    {
        ChooseState();
    }

    private void ChooseState()
    {
        
        int randomFire = Mathf.FloorToInt(Random.Range(0f, amountOfFirePrefabs-1f));
        firePrefabs.transform.GetChild(randomFire).gameObject.SetActive(true); 
        fireColliders.transform.GetChild(randomFire).gameObject.SetActive(true); 
        createSpread();
    }

    private void createSpread()
    {
        curernt = 0f;
        currentLife = 0f;
        spreadRate = Random.Range(minSpreadRate, maxSpreadRate);
        lifeTime = Random.Range(minLifeTime, maxLifeTime);
    }

    private void FixedUpdate()
    {
        if(!spread)
        {
            if(curernt >= spreadRate)
            {
                createNewFire();
            }
            else
            {
                curernt += 0.02f; 
                
            }
        }
        if(currentLife < lifeTime)
        {
            currentLife += 0.02f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void createNewFire()
    {
        spread = true;
        Vector3 newPost = transform.position + new Vector3(Random.Range(-maxSpreadRange,maxSpreadRange)+envPos.x, 0f,Random.Range(-maxSpreadRange,maxSpreadRange) + envPos.z); 
        FireScript newFire = Instantiate(gameObject,newPost, Quaternion.identity).GetComponent<FireScript>();
        newFire.transform.parent = FireParent.transform;
        newFire.FireParent = FireParent;
    }




}
