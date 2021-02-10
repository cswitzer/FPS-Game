using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] float range = 10f;
    [SerializeField] float damage = 10f;
    [SerializeField] float impactForce = 500f;
    [SerializeField] float fireRate = 2f;

    [SerializeField] int maxAmmo = 30;
    [SerializeField] float reloadTime = 1f;
    private int currentAmmo;
    private bool isReloading = false;

    public Animator animator;

    [SerializeField] ParticleSystem muzzleFlash;
    // we want to instantiate hit FX, so that is why this is gameobject
    [SerializeField] GameObject impactEffect;

    private float nextTimeToFire = 0f;

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

        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
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

    private void Shoot()
    {
        currentAmmo--;
        muzzleFlash.Play();
        audioSource.Play();
        RegisterHitResult();
    }

    private void RegisterHitResult()
    {
        // place shooting code using raycasting (using the camera)
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // does the actual raycasting and returns true if we hit something
        if (Physics.Raycast(ray, out hit, 100))
        {
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

}
