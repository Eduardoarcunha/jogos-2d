using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    private Transform cam;
    public float ParallaxEffect;
    private float lockedY;
    // Start is called before the first frame update
    void Start()
    {
        
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
        lockedY = transform.position.y;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        float repos = cam.transform.position.x * (1 - ParallaxEffect);
        float distance = cam.transform.position.x * ParallaxEffect;
        transform.position = new Vector3(startpos + distance, lockedY, transform.position.z);

        // Debug.Log("Repos: " + repos + ", Distance: " + distance);

        if (repos > startpos + length) startpos += length;
        else if (repos < startpos - length) startpos -= length;
        
    }
}
