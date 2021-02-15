using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class CarRoboControl : Agent
{

    public EnviormentScript enviorment;
    public bool train;

    [SerializeField] private Rigidbody rb;

    [SerializeField] private WheelCollider wheel1;
    [SerializeField] private WheelCollider wheel2;
    [SerializeField] private WheelCollider wheel3;
    [SerializeField] private WheelCollider wheel4;
    [SerializeField] private WheelCollider wheel5;
    [SerializeField] private WheelCollider wheel6;

    [SerializeField] private KeyCode Forward;
    [SerializeField] private KeyCode Back;
    [SerializeField] private KeyCode TurnLeft;
    [SerializeField] private KeyCode TurnRight;
    
    private Transform[] rayCastChildren;

    [SerializeField] private RewardTileChecker TileChecker;
    
    [SerializeField] private GameObject RayCastParent;

    [SerializeField] private float turnMultiplier;
    [SerializeField] private float forwardMultipler;

    [SerializeField] private float speed;
    [SerializeField] private float chooseTimerIterations;
    [SerializeField] private float tickRate;
    [SerializeField] private float maxDistancesOfRays;
    [SerializeField] private int maxSteps;

    private Vector3 prevPos;

    private float curTimer;

    private bool canChoose;
    private int currentStep;

    public override void Initialize()
    {
        enviorment = transform.parent.gameObject.GetComponent<EnviormentScript>();
        enviorment.roboClass = this;
        rayCastChildren = RayCastParent.GetComponentsInChildren<Transform>();
    }

    public override void OnEpisodeBegin()
    {
        currentStep = 0;
        prevPos = transform.position;
        canChoose = true;
        curTimer = 0f;
        train = false;

        transform.position = enviorment.InitalizeAgentPos();
        transform.rotation = Quaternion.Euler(enviorment.InitalizeAgentRot());
        enviorment.InitalizeEnviroment();
        TileChecker.resetEp();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        RaycastHit hit;
        for(int i = 1; i < rayCastChildren.Length; i++)
        {
            //Debug.DrawRay(rayCastChildren[i].position, rayCastChildren[i].transform.forward * maxDistancesOfRays, Color.red, 0.02f, false);
            if(Physics.Raycast(rayCastChildren[i].position, rayCastChildren[i].transform.forward,out hit, maxDistancesOfRays))
            {
                AddReward(0-((maxDistancesOfRays-hit.distance)/2));
                sensor.AddObservation(hit.distance);
            }
        }
    }

    public override void Heuristic(float[] actions)
    {
        actions[0] = 0f;
        

        if(Input.GetKey(Forward))
        {
            actions[0] = 0f;
        }
        else if(Input.GetKey(Back))
        {
            actions[0] = 1f;
        }
        else if(Input.GetKey(TurnLeft))
        {
            actions[0] = 2f; 
        }
        else if(Input.GetKey(TurnRight))
        {
            actions[0] = 3f;
        }
        else
        {
            actions[0] = 4f;
        }
    }

    public override void OnActionReceived(float[] Actions)
    {
        int choice = Mathf.FloorToInt(Actions[0]);

        switch(choice)
        {
            case 0:
                forwardTank(speed);
                break;
            case 1:
                forwardTank(-speed);
                break;
            case 2:
                TankTurn(speed);
                break;
            case 3:
                TankTurn(-speed);
                break;
            case 4:
                StopMoving(1);
                break;
        }
        // if(choice == 0)
        // {
        //     forwardTank(speed);
        // }
        // else if(choice == 1)
        // {
        //     forwardTank(-speed);
        // }
        // else if(choice == 2)
        // {
        //     TankTurn(speed);
        // }
        // else if(choice == 3)
        // {
        //     TankTurn(-speed);
        // }
        // else
        // {
        //     StopMoving(1);
        //}
         
    }
    public void Moved()
    {
        AddReward(0.5f);
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if(currentStep >=maxSteps)
        {
            EndEpisode();
        }
        currentStep++;
        
        if(!canChoose)
        {
            if(curTimer >= chooseTimerIterations)
            {
                canChoose = true;
                curTimer = 0f;
            }
            else
            {
                curTimer++;
            }
        }
        if(train && canChoose)
        {
            RequestDecision();
        }
        if((prevPos.x -1 <= transform.position.x  && prevPos.z-1 <= transform.position.z) && (prevPos.x +1>= transform.position.x  && prevPos.z+1 >= transform.position.z))
        {
            AddReward(-0.015f);
        }
        else
        {
            
        AddReward(-0.002f);
        }
        prevPos = transform.position;
    
    }
        
    private void OnTriggerEnter(Collider _collider)
    {
        if(_collider.gameObject.tag == "fire")
        {
           AddReward(-1f);
           EndEpisode();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "wall")
        {
            AddReward(-1f);
            EndEpisode();
            Debug.Log("BAM");
        }
    }
    


//         

    private void TankTurn(float _horizontalVel)
    {
        StopMoving(0);
        wheel1.motorTorque = turnMultiplier*_horizontalVel;
        wheel2.motorTorque = turnMultiplier*_horizontalVel;
        wheel5.motorTorque = turnMultiplier*_horizontalVel;
        wheel3.motorTorque = -turnMultiplier*_horizontalVel;
        wheel4.motorTorque = -turnMultiplier*_horizontalVel ;
        wheel6.motorTorque = -turnMultiplier*_horizontalVel;
    }

    private void forwardTank(float _forwardVel)
    {
        StopMoving(0);        
        
        wheel1.motorTorque = forwardMultipler*_forwardVel;
        wheel2.motorTorque = forwardMultipler*_forwardVel;
        wheel3.motorTorque = forwardMultipler*_forwardVel;
        wheel4.motorTorque = forwardMultipler*_forwardVel;
        wheel5.motorTorque = forwardMultipler*_forwardVel;
        wheel6.motorTorque = forwardMultipler*_forwardVel;
    }

    private void stopTorque(float _forwardVel)
    {        
        wheel1.motorTorque = forwardMultipler*_forwardVel;
        wheel2.motorTorque = forwardMultipler*_forwardVel;
        wheel3.motorTorque = forwardMultipler*_forwardVel;
        wheel4.motorTorque = forwardMultipler*_forwardVel;
        wheel5.motorTorque = forwardMultipler*_forwardVel;
        wheel6.motorTorque = forwardMultipler*_forwardVel;
    }

    
    private void StopMoving(int multiplier)
    {
        stopTorque(0);
        wheel1.brakeTorque = 5*speed*multiplier;
        wheel2.brakeTorque = 5*speed*multiplier;
        wheel3.brakeTorque = 5*speed*multiplier;
        wheel4.brakeTorque = 5*speed*multiplier;
        wheel5.brakeTorque = 5*speed*multiplier;
        wheel6.brakeTorque = 5*speed*multiplier; 
    }
}


 // if()
        // {
            
            
        //     if(Input.GetKey(KeyCode.D))
        //     {
                
        //         TankTurn(speed);
        //     } 
        //     else
        //     {
        //         TankTurn(-speed)   ;    
        //     }
        // }
        // else 
        // {
        //     if(Input.GetKey(KeyCode.S))
        //     {
        //         forwardTank(-speed);   
        //     }
        //     else if(Input.GetKey(KeyCode.W))
        //     {
        //         forwardTank(speed);   
        //     }
            
        // } 