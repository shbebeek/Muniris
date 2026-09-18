using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : LeftHandItem
{
    public PlayerMovement user;
    public GameObject flame;
    public GameObject lightObject;

    public override void Start()
    {
        base.Start();
        if(transform.root.name == "Keyboard Player")
        {
            user = transform.root.Find("Player").GetComponent<PlayerMovement>();
        }else if (transform.root.name == "Controller Player")
        {
            user = transform.root.Find("Player Controller").GetComponent<PlayerMovement>();
        }
    }
    void Update()
    {
        if (user.isSwimming)
        {
            flame.SetActive(false);
            lightObject.SetActive(false);
        }
        else
        {
            flame.SetActive(true);
            lightObject.SetActive(true);
        }
    }
}
