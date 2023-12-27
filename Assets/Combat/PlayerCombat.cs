using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private bool canAttack = true;
    public LayerMask attackableLayers;
    public float moveSpeed = 0.2f;
    private GameObject closestEnemy;

    public GameObject playerMain;

    void Awake()
    {
        
    }
    public void OnAttack()
    {
        closestEnemy = GetNearestEnemy(2);

        if(closestEnemy != null)
        {
            Transform moveToPosition = closestEnemy.GetComponent<EnemyAi>().moveToPosition;
            Vector3 direction = moveToPosition.transform.position - playerMain.transform.position;
            direction.Normalize();
            float moveToFactor = closestEnemy.GetComponent<EnemyAi>().moveToFactor;
            Vector3 targetPosition = moveToPosition.transform.position + (direction * moveToFactor * -1);
            
            // Debug.Log(Vector3.Distance(playerMain.transform.position, closestEnemy.transform.position));
            if(Vector3.Distance(playerMain.transform.position, closestEnemy.transform.position) > closestEnemy.GetComponent<EnemyAi>().minMoveDistance)
            {
                StartCoroutine(SmoothMoveTo(playerMain, targetPosition, moveSpeed));
            }
            
            direction.y = 0f;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            StartCoroutine(SmoothRotateTo(gameObject, targetRotation, moveSpeed));
            // GameObject.Find("MoveToLoc").transform.position = targetPosition;
        }
    }
    
    public GameObject GetNearestEnemy(float traceRadius)
    {
        List<GameObject> inRange = new List<GameObject>();
        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, traceRadius, transform.forward, 0, attackableLayers, QueryTriggerInteraction.UseGlobal);
    
        GameObject targetObject;
        float compareDistance;

        inRange.Clear();

        foreach (RaycastHit hit in raycastHits)
        {
            inRange.Add(hit.transform.gameObject);
        }

        if(inRange.Count > 0)
        {
            targetObject = inRange[0];
            compareDistance = Vector3.Distance(transform.position, targetObject.transform.position);

            for (int i = 0; i < inRange.Count; i++)
            {
                if(Vector3.Distance(transform.position, inRange[i].transform.position) < compareDistance)
                {
                    targetObject = inRange[i];
                }
            }
            
            return targetObject;
        } else {
            return null;
        }
    }

    public IEnumerator SmoothMoveTo(GameObject targetObject, Vector3 targetLocation, float seconds)
    {
        float elapsedTime = 0;
        Vector3 startingPos = targetObject.transform.position;
        while (elapsedTime < seconds)
        {
            targetObject.transform.position = Vector3.Lerp(startingPos, targetLocation, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetObject.transform.position = targetLocation;
    }

    public IEnumerator SmoothRotateTo(GameObject targetObject, Quaternion targetRotation, float seconds)
    {
        float elapsedTime = 0;
        Quaternion startingRot = targetObject.transform.rotation;
        while (elapsedTime < seconds)
        {
            targetObject.transform.rotation = Quaternion.Lerp(startingRot, targetRotation, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetObject.transform.rotation = targetRotation;
    }
}
