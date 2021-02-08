using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] float damage = 25f;
    [SerializeField] float impactForce = 700f;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactEffect;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // GetButtonDown for one click fire
        if (Input.GetButtonDown("Fire1"))
        {
            muzzleFlash.Play();
            audioSource.Play();
            Shoot();
        }
    }

    void Shoot()
    {
        // the ray will map to the mouse's position
        RaycastHit hit; // this returns a Vector3!
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 120))
        {
            // Get the target at the transform we hit
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log(target.health);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            // particles point out from an object, so use normal
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2.5f);
        }
    }


}
