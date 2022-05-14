using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed;
    
    [System.NonSerialized]
    public EntityAI currentTarget;

    [System.NonSerialized]
    public int damage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (currentTarget != null)
        {
            transform.LookAt(currentTarget.transform);
            MoveTowardsTarget();
        }
    }

    protected void MoveTowardsTarget()
    {
        Vector3 moveVector = (currentTarget.transform.position - transform.position).normalized * speed * Time.fixedDeltaTime;
        transform.position = transform.position + moveVector;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Entity"))
        {
            EntityAI entity = collider.GetComponent<EntityAI>();

            if(entity == currentTarget)
            {
                entity.RemoveHealth(damage);
            }

            Destroy(gameObject);
        }
    }
}
