using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : SimpleMapPlaceable
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.Rotate(new Vector3(0f, Random.Range(0, 4) * 90f, 0f));
    }

    protected override void Initialize()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}