using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    private Transform cam;
    public float ParallaxEffect;
    // Start is called before the first frame update
    void Start()
    {
        
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        float repos = cam.transform.position.x * (1 - ParallaxEffect);
        float distance = cam.transform.position.x * ParallaxEffect;
        transform.position = new Vector3(startpos + distance, transform.position.y, transform.position.z);

        Debug.Log("Repos: " + repos + ", Distance: " + distance);

        if (repos > startpos + length) startpos += length;
        else if (repos < startpos - length) startpos -= length;
        
    }
}
