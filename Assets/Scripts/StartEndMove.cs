using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEndMove : MonoBehaviour
{
    bool holded;

    void OnMouseOver()
    {
        if ((GameHandler.stopped || GameHandler.paused) && !GameHandler.helpIsOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                holded = true;
                return;
            }
        }


    }

    void Update()
    {
        if (holded)
        {
            transform.localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, 1);
            if (Input.GetMouseButtonUp(0))
                holded = false;
        }
    }
}
