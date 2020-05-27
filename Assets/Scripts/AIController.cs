using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    Transform target;
    //NavMeshAgent agent;

    public float MoveSpeed = 4f;
    public float MaxDist = 8f;
    public float distance;
    public float MinDist = 2f;
    private int damage;

    void Start() 
    {
    	//agent = GetComponent<NavMeshAgent>();
    	target = PlayerManager.instance.player.transform;
    	EnemyManager.instance.numEnemies++;
    	damage = EnemyManager.instance.damage;
    }
 
    void Update() 
    {
        //transform.LookAt(Player);
        distance = Vector3.Distance(target.position, transform.position);

 		if (distance <= MinDist)
 		{
 			Vector3 direction = (target.position - transform.position).normalized;
 			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);

			// do an attack
 		}
        else if (distance <= MaxDist) {
 			//agent.SetDestination(target.position);
			// need to figure out how to make navmeshes

			Vector3 direction = (target.position - transform.position).normalized;
			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 8f);

            transform.position += transform.forward * MoveSpeed * Time.deltaTime;

        }
    }

    void onTriggerEnter(Collider other)
    {
    	if (other.CompareTag("Player"))
    	{
    		PlayerManager.instance.hp -= damage;
    	}
    }

    void OnDrawGizmosSelected()
    {
    	Gizmos.color = Color.red;
    	Gizmos.DrawWireSphere(transform.position, MaxDist);
    	Gizmos.DrawWireSphere(transform.position,MinDist);
    }
}
