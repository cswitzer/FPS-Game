using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float damage = 10f;
    [SerializeField] float range = 100f;

    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        HandleGunVerticalRotation();
    }

    private static void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // place shooting code using raycasting (using the camera)
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // does the actual raycasting and returns true if we hit something
            if (Physics.Raycast(ray, out hit, 100));
            {
                Debug.DrawLine(ray.origin, hit.point);
                Debug.Log(hit.transform.name);
            }
        }
    }

    void HandleGunVerticalRotation()
    {

    }

}
