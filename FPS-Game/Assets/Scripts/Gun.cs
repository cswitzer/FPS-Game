using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float range = 10f;
    [SerializeField] float damage = 10f;

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // place shooting code using raycasting (using the camera)
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // does the actual raycasting and returns true if we hit something
            if (Physics.Raycast(ray, out hit, 100));
            {
                Debug.Log(hit.transform.name);
            }

            Target target = hit.transform.GetComponent<Target>();
            target.TakeDamage(damage);
            Debug.Log(target.health);
        }
    }

    public float GetDamage()
    {
        return damage;
    }

}
