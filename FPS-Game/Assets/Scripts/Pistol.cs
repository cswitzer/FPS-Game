using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] float damage = 25f;
    [SerializeField] float impactForce = 700f;

    [SerializeField] int maxAmmo = 12;
    [SerializeField] float reloadTime = 1f;
    private int currentAmmo;
    private bool isReloading = false;

    public Animator animator;

    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactEffect;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        currentAmmo = maxAmmo;
    }

    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            // if I am reloading, I don't want be able to shoot or restart my coroutine
            return;
        }

        // Do we need to reload yet?
        CheckForReload();

        // GetButtonDown for one click fire
        if (Input.GetButtonDown("Fire1"))
        {
            muzzleFlash.Play();
            audioSource.Play();
            Shoot();
        }
    }

    void CheckForReload()
    {
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            // return because we don't want to shoot again
            return;
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // play reload animation
        animator.SetBool("Reloading", true);

        // pause for reloadTime seconds. -.25f so I can't shoot during reload animation
        yield return new WaitForSeconds(reloadTime - .25f);

        // play idle animation
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);
        Debug.Log("Time to blast some enemies!");
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        currentAmmo--;
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
