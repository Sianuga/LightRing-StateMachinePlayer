using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LightRing : MonoBehaviour
{
    GameObject blockGroup;
    List<GameObject> blocks = new List<GameObject>();
    GameObject lightRing;
    [SerializeField] float lerpTime = 1f;
    [SerializeField] float timeToPlatformOutlineDissapear = 1f;
    [SerializeField] float renderDistanceForLightRing = 30f;
    bool lightRingIsAlive = false;
    float distance,perc;
    int blocksBeingChanged = 0;


    void Start()
    {

        blockGroup = GameObject.FindGameObjectWithTag("GroupOfBlocks");
        for (int i = 0; i < blockGroup.transform.childCount; i++)
        {
            blocks.Add(blockGroup.transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !lightRingIsAlive && blocksBeingChanged==0)
        {
            foreach (GameObject block in blocks)
            {

                block.GetComponent<Block>().SetUpStartingDistance(transform.position);
            }
            FindObjectOfType<LightRing>().CreateLightRing();
        }


        SearchForLightRing();
        if (lightRingIsAlive)
        {
            Change();
        }
      
    }

    private void Change()
    {
        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<Block>().GetStartingDistance() < renderDistanceForLightRing)
            {
                block.GetComponent<Block>().setBeingChanged(true);
                changeShaderDirection(block);
                changeShaderVisibility(block);
            }

        }

    }
    public void returnToShaderRestState(float timeToReturn)
    {
        foreach (GameObject block in blocks)
        {
            
                float fadeValue = block.GetComponent<Renderer>().material.GetFloat("_Fade");
            if(fadeValue>0.01f)
            {
                StartCoroutine(increaseShaderVisibility(fadeValue, block));
                var yMovement = block.GetComponent<Renderer>().material.GetFloat("_yMovement");
                var xMovement = block.GetComponent<Renderer>().material.GetFloat("_xMovement");
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", -yMovement);
                block.GetComponent<Renderer>().material.SetFloat("_xMovement", -xMovement);
                fadeValue = block.GetComponent<Renderer>().material.GetFloat("_Fade");
            }
              
                StartCoroutine(platformOutlineVisibilityDuration(fadeValue, block));
            
            block.GetComponent<Renderer>().material.SetFloat("_Fade", 0);
            block.GetComponent<Block>().setBeingChanged(false);
        }
    }

    IEnumerator platformOutlineVisibilityDuration(float timeToReturn, GameObject block)
    {
        yield return new WaitForSeconds(timeToPlatformOutlineDissapear);
        StartCoroutine(decreaseShaderVisibility(timeToReturn, block));
    }

    IEnumerator decreaseShaderVisibility(float timeToReturn, GameObject block)
    {
        while (timeToReturn > 0)
        {
            timeToReturn -= 0.01f;
            block.GetComponent<Renderer>().material.SetFloat("_OutlineVisibility", timeToReturn);
            yield return new WaitForSeconds(0.001f);
        }
    }
    IEnumerator increaseShaderVisibility(float timeToFullVisibility, GameObject block)
    {
        while (timeToFullVisibility < 1)
        {
            timeToFullVisibility += 0.01f;
            block.GetComponent<Renderer>().material.SetFloat("_Fade", timeToFullVisibility);
            block.GetComponent<Renderer>().material.SetFloat("_OutlineVisibility", timeToFullVisibility);
            yield return new WaitForSeconds(0.001f);
        }
    }

    private void changeShaderVisibility(GameObject block)
    {
        distance = Vector2.Distance(lightRing.GetComponent<CircleCollider2D>().ClosestPoint(block.transform.position), block.transform.position);
        perc = distance / block.GetComponent<Block>().GetStartingDistance();
        lerpTime = Mathf.Lerp(0, 1, perc * perc * perc * (perc * (6f * perc - 15f) + 10f));
        //lerpTime = Mathf.Lerp(0, 1, (6.052f * Mathf.Pow(perc, 5) + 18.155f * Mathf.Pow(perc, 4) + 16.743f * Mathf.Pow(perc, 3) - 3.994f * Mathf.Pow(perc, 2) + 0.366f * perc - 0.012f));
        //lerpTime = Mathf.Lerp(0, 1, Mathf.Pow(10f * perc * 1.235f - 1f, 3) * ((Mathf.Pow(150f * perc, 2) - 405f * perc + 289f) / 2f*24790f));
        block.GetComponent<Renderer>().material.SetFloat("_Fade", 1 - lerpTime);
        block.GetComponent<Renderer>().material.SetFloat("_OutlineVisibility", 1 - lerpTime);
    }
    private void overTimeShaderVisibilityChange(float time, GameObject block)
    {
        block.GetComponent<Renderer>().material.GetFloat("_Fade");

    }
    private void changeShaderDirection(GameObject block)
    {
        float a = (block.transform.position.x - lightRing.transform.position.x)
        / Mathf.Sqrt(Mathf.Pow(block.transform.position.x - lightRing.transform.position.x, 2) + Mathf.Pow(block.transform.position.y - lightRing.transform.position.y, 2));
        if (lightRing.transform.position.y >= block.transform.position.y)
        {
            if (a < 0)
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", 1f + a);
            else
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", 1f - a);

            block.GetComponent<Renderer>().material.SetFloat("_xMovement", -a);
        }
        else
        {
            if (a < 0)
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", -1f - a);
            else
                block.GetComponent<Renderer>().material.SetFloat("_yMovement", -1f + a);
            block.GetComponent<Renderer>().material.SetFloat("_xMovement", -a);
        }
    }

    private void SearchForLightRing()
    {
        if (!FindObjectOfType<Ringo>())
        {
            if (lightRingIsAlive)
                returnToShaderRestState(1);
            lightRingIsAlive = false;
            return;
        }
        else
        {
            lightRingIsAlive = true;
            lightRing = FindObjectOfType<Ringo>().gameObject;

        }
    }
}
