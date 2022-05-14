using System;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType{
    Base,
    Mage,
    Baracks,
    Earthbreaker,
    Archer
}


public class Tower : MonoBehaviour
{

    public TowerType type;

    [Range(1, 100)]
    public int AttackSpeed;
    public Vector2 Damage;
    public float Range { get { return rangeCollider.radius; } set { rangeCollider.radius = value; } }

    protected List<EntityAI> inRangeEntities = new List<EntityAI>();     //Entities that are in range

    [System.NonSerialized]
    public EntityAI currentTarget;

    [System.NonSerialized]
    public Tile plot; //The plot the tower is on

    //Prefabs
    [SerializeField] private SphereCollider rangeCollider;
    [SerializeField] private GameObject ProjectilePrefab;

    //ETC.
    protected float AttackWaitTime { get { return Mathf.Lerp(References.minAttackWaitTime, References.maxAttackWaitTime, 1 - ((AttackSpeed - 1) / (References.maxAttackSpeed - 1))); } }
    protected float currentWaitTime = 2;

    // Start is called before the first frame update
    void Start()
    {
        //BeatNotifier.instance.AddSpectrumBeatListener(1, Attack);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Refresh();
    }

    /// <summary>
    /// Refreshes the tower.
    /// </summary>
    protected virtual void Refresh()
    {
        currentWaitTime += Time.fixedDeltaTime;

        if (currentTarget != null)
        {
            if (currentTarget.Health <= 0)
            {
                inRangeEntities.Remove(currentTarget);
                SelectTarget();
            }
            else
            {
                WaitToAttack();
            }
        }
        else
        {
            SelectTarget();
        }
    }

    /// <summary>
    /// Selects a target entity to attack from the entities that are in range.
    /// </summary>
    protected void SelectTarget()
    {
        if (inRangeEntities.Count > 0)
        {
            float smallestRange = float.MaxValue;
            int index = 0;

            for (int i = 0; i < inRangeEntities.Count; i++)
            {
                Vector3 pos = inRangeEntities[i].transform.position;
                Vector3 distVector = pos - this.transform.position;

                if (distVector.sqrMagnitude < smallestRange)
                {
                    smallestRange = distVector.sqrMagnitude;
                    index = i;
                }
            }
            currentTarget = inRangeEntities[index];
        }
    }

    /// <summary>
    /// Waits for the attack wait time to pass, and then attack.
    /// </summary>
    protected void WaitToAttack()
    {
        if(currentWaitTime >= AttackWaitTime)
        {
            Attack();
            currentWaitTime = 0;
        }
    }

    /// <summary>
    /// Attack the target entity.
    /// </summary>
    /// <param name="entity"></param>
    protected virtual void Attack()
    {
        if (currentTarget != null)
        {
            int damage = UnityEngine.Random.Range((int)Damage.x, (int)Damage.y + 1);

            GameObject obj = Instantiate(ProjectilePrefab, transform.GetChild(0).position, Quaternion.identity);
            //Projectile projectile = obj.GetComponent<Projectile>();

            //projectile.damage = damage;
            //projectile.currentTarget = currentTarget;
        }
    }

    //Triggered when something enters the tower range
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Entity"))
        {
            inRangeEntities.Add(collider.GetComponent<EntityAI>());
        }
    }

    //Triggered when something exits the tower range
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag.Equals("Entity"))
        {
            if(currentTarget == collider.GetComponent<EntityAI>())
            {
                currentTarget = null;
            }

            inRangeEntities.Remove(collider.GetComponent<EntityAI>());
        }
    }


}
