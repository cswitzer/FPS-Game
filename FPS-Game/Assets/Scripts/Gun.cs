using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] float range = 10f;
    [SerializeField] float damage = 10f;
    [SerializeField] float impactForce = 500f;
    [SerializeField] float fireRate = 2f;

    [SerializeField] ParticleSystem muzzleFlash;
    // we want to instantiate hit FX, so that is why this is gameobject
    [SerializeField] GameObject impactEffect;

    private float nextTimeToFire = 0f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            muzzleFlash.Play();
            Shoot();
        }
    }

    private void Shoot()
    {
        RegisterHit();
    }

    private void RegisterHit()
    {
        // place shooting code using raycasting (using the camera)
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // does the actual raycasting and returns true if we hit something
        if (Physics.Raycast(ray, out hit, 100));
        {
            Debug.Log(hit.transform.name);

            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log(target.health);
            }

            if (hit.rigidbody != null)
            {
                // goes backwards when we hit it
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            // particles point out from object, so we are using the normal
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2.5f);
        }
    }

    public float GetDamage()
    {
        return damage;
    }

}
