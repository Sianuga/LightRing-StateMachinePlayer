using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    bool beingChanged = false;
    float startingDistance;
    private void Awake()
    {
        this.gameObject.GetComponent<Renderer>().material = new Material(this.gameObject.GetComponent<Renderer>().material);
    }
  
    void Start()
    {
        
    }

   public float GetStartingDistance()
    {
        return startingDistance;
    }
    public void SetUpStartingDistance(Vector2 objectPositionFromThisBlock)
    {
        startingDistance = Vector2.Distance(transform.position, objectPositionFromThisBlock);
    }
    public void setBeingChanged(bool state)
    {
        beingChanged = state;
    }
    public bool getBeingChanged()
    {
        return beingChanged;
    }
}
