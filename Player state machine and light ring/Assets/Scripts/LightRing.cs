using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightRing : MonoBehaviour
{
    [SerializeField] GameObject lightRingModel;
    [Header("Inflation rates")]
    [SerializeField] float ringInflationRate = 1.1f;
    [SerializeField] float ringInflationRateSlowed = 1.01f;
    [Header("Expansion rates")]
    [SerializeField] float expansionInterval = 0.01f;
    [SerializeField] float expansionIntervalSlowed = 0.5f;

    [Header("Miscellanous")]
    [SerializeField] float slowedStateScale = 1.5f;
    [SerializeField] float maxScale = 10f;

    Vector2 startingPosition;
    int i = 0;

    public bool ringIsCreated = false;
    public void CreateLightRing()
    {
        if(!ringIsCreated)
        {
            setUpStartingPosition();
 GameObject lightRing = Instantiate(lightRingModel, getStartingPosition(), Quaternion.identity) as GameObject;
        StartCoroutine(RingInflation(lightRing));
        }
       
    }
    IEnumerator RingInflation(GameObject RingBeingInflated)
        {
        ringIsCreated = true;
        while(RingBeingInflated.transform.localScale.x<maxScale)
        {

            if (RingBeingInflated.transform.localScale.x < slowedStateScale)
            {
      RingBeingInflated.transform.localScale = new Vector3(RingBeingInflated.transform.localScale.x * ringInflationRate, RingBeingInflated.transform.localScale.y * ringInflationRate, RingBeingInflated.transform.localScale.z);
               
            yield return new WaitForSeconds(expansionInterval);
            }
            else
            {
                RingBeingInflated.transform.localScale = new Vector3(RingBeingInflated.transform.localScale.x * ringInflationRateSlowed, RingBeingInflated.transform.localScale.y * ringInflationRateSlowed, RingBeingInflated.transform.localScale.z);
               
                yield return new WaitForSeconds(expansionIntervalSlowed);
            }
                
        }
        
       Destroy(RingBeingInflated);
      //  GetComponentInParent<Player>().returnToShaderRestState(1f);
     
        ringIsCreated = false;
        }
    public void setUpStartingPosition()
    {
        startingPosition = transform.position;
    }
    public Vector2 getStartingPosition()
    {
        return startingPosition;
    }
}
