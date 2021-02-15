using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviormentScript : MonoBehaviour
{
    public CarRoboControl roboClass; 

    [SerializeField] private float fireSpawnArea;
    [SerializeField] private float maxFires;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private GameObject parentTileRewards;
    [SerializeField] private GameObject fireParent;

    public void InitalizeEnviroment()
    {
        int amountOfFires = Mathf.FloorToInt(Random.Range(6f, maxFires));
        Transform[] fireChildren = fireParent.GetComponentsInChildren<Transform>();
        for(int i = 1;i < fireChildren.Length; i++)
        {
            Destroy(fireChildren[i].gameObject);
        }
        for(int i = 0; i < amountOfFires; i++)
        {
            FireScript newFire = Instantiate(firePrefab, new Vector3(Random.Range(-fireSpawnArea,fireSpawnArea)+ transform.position.x, 2.5f, Random.Range(-fireSpawnArea,fireSpawnArea)+ transform.position.z), Quaternion.identity).GetComponent<FireScript>();
            newFire.transform.parent = fireParent.transform;
            newFire.FireParent = fireParent;
            newFire.envPos = fireParent.transform.position;
        }
        Transform[] childrenInParentTile = parentTileRewards.GetComponentsInChildren<Transform>();
        for(int i = 1;i < childrenInParentTile.Length; i++)
        {
            Destroy(childrenInParentTile[i].gameObject);
        }
        roboClass.train = true;
    }

    public Vector3 InitalizeAgentPos()
    {
        return new Vector3(Random.Range(-fireSpawnArea,fireSpawnArea) + transform.position.x,5f,Random.Range(-fireSpawnArea,fireSpawnArea) + transform.position.z);
    }
    public Vector3 InitalizeAgentRot()
    {
        return new Vector3(0f,Random.Range(0f, 359f),0f);
    }
    

}
