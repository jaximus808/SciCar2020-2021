using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTileChecker : MonoBehaviour
{
    [SerializeField] private GameObject tileReward;
    [SerializeField] private GameObject RewardParent;

    private CarRoboControl agentScript;

    private bool hitTrig;
    private bool onTile;

    private void Awake()
    {
        onTile = false;
        agentScript = transform.parent.gameObject.GetComponent<CarRoboControl>();
    }

    private void FixedUpdate()
    {
        
        if(!hitTrig && agentScript.train)
        {
            NotOnTile();
        }
        hitTrig = false; 
       
    }
    public void resetEp()
    {
        hitTrig = true;
        NotOnTile();
        onTile = true;
    }
    

    // private void OnTriggerStay(Collider collider)
    // {
    //     if(agentScript.train )
    //     {
    //         onTile = false;
    //     }
    // }

    private void OnTriggerStay(Collider collider)
    {
        hitTrig = true; 
        // if(agentScript.train)
        // {
        //     onTile = true; 
        // }
    }

    private void NotOnTile()
    {
        agentScript.Moved();
        GameObject newTile = Instantiate(tileReward, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity);
        newTile.transform.parent = RewardParent.transform;
    }
}
