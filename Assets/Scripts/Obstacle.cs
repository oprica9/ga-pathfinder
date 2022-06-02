using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    bool holded;
    bool resize;
    bool rotate;

    void OnMouseOver()
    {
        if ((GameHandler.stopped || GameHandler.paused) && !GameHandler.helpIsOpen)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    resize = true;
                    return;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Destroy(gameObject);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                rotate = true;
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                holded = true;
                return;
            }
        }

    }

    void Update()
    {

        if (holded && !resize && !rotate)
        {
            transform.localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localPosition = new Vector3(transform.position.x, transform.position.y, 1);
            if (Input.GetMouseButtonUp(0))
                holded = false;
        }

        if (resize && !holded && !rotate)
        {
            transform.localScale = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonUp(0))
                resize = false;
        }

        if (rotate && !resize && !holded)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation_z);

            if (Input.GetMouseButtonUp(1))
                rotate = false;
        }

    }

}
