using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [SerializeField] float scrollSpeed  = 2f;
    float offset;
    float rotate;
    // Update is called once per frame
    void Update()
    {

        offset +=  (Time.deltaTime* scrollSpeed) / 10f;
            GetComponent<SpriteRenderer>().material.SetTextureOffset("_MainTex", new Vector2(0,-offset));

    }
}
