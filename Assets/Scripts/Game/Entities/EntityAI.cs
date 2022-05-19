using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Entities are controllable units on the road.
/// </summary>
public class EntityAI : MonoBehaviour
{

    /// <summary>
    /// How fast the entity travels on the road in Units/Second
    /// </summary>
    public float movementSpeed;
    public int baseHealth;


    public int Health { get; protected set; }

    protected Road targetRoad;
    protected Vector3 targetPosition;
    protected float OffsetOnRoad;

    private void Start()
    {
        Health = baseHealth;
    }

    public void FixedUpdate()
    {

        if (Health <= 0)
        {
            Health = 0;
            return;
        }

        //If there is no target then set one
        if (targetRoad == null)
        {
            targetRoad = References.map.roadPoints[0];
            //Setting target pos
            targetPosition = new Vector3(targetRoad.Position.x, 1, targetRoad.Position.y);

        }

        //If the destination is reached, set a new destination
        if (MoveToTargetPosition())
        {
            if (targetRoad.type == RoadType.End)//If last road node have been reached
            {
                OnEndReached();
            }
            else
            {
                targetRoad = targetRoad.Next[0];
            }

            //Setting target pos
            targetPosition = new Vector3(targetRoad.Position.x, 1, targetRoad.Position.y);
        }
    }

    public void RemoveHealth(int amount)
    {
        Health -= amount;
    }

    /// <summary>
    /// Moves the unit to the target position. (This method should be used in FixedUpdate)
    /// <para>Returns true when it has reached the desired position.</para>
    /// </summary>
    protected bool MoveToTargetPosition()
    {
        if (transform.position != targetPosition)
        {
            //the direction vector
            Vector3 dVector = (targetPosition - transform.position);
            float movement = movementSpeed * Time.fixedDeltaTime;//units to move under one frame

            //if the target position is closer than the units it would move, set position to target and return
            if (dVector.magnitude <= movement)
            {
                transform.position = new Vector3(targetPosition.x, 1, targetPosition.z);
                return true;
            }

            //Otherwise move the desired amount of units
            dVector.Normalize();

            transform.position = transform.position + dVector * movement;


            return false;

        }
       
            return true;
        
    }

    /// <summary>
    /// Sets the position on the map. 
    /// <para>One map tile equals 1 unit in map coordinates.</para>
    /// </summary>
    /// <param name="posOnMap"></param>
    /// <returns></returns>
    protected void SetPositionOnMap(Vector2 posOnMap)
    {
        transform.position = new Vector3(targetRoad.Position.x, 1, targetRoad.Position.y);
    }

    protected virtual void OnEndReached()
    {
        SetPositionOnMap(References.map.roadPoints[0].Position);
        targetRoad = References.map.roadPoints[0];
    }

}
