using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    [SerializeField] public float fireRate = 15f;
    [SerializeField] public float impactForce = 30f;
    [SerializeField] public bool isProjectileFire = false;

    [SerializeField] int maxAmmo = 10;
    private int currentAmmo;
    [SerializeField] TextMeshPro text;
    [SerializeField] TMP_FontAsset m_Font;
    [SerializeField] float reloadTime = 1;
    private bool isReloading;

    public AudioSource gunShot;
    public AudioClip singleShot;

    public Camera fpsCam;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactEffect;


    private float nextTimeToFire = 0f;

    [SerializeField] Animator animator;

    private void Start()
    {
        if(isReloading)
            return;

        currentAmmo = maxAmmo;
        //GameObject textGO = new GameObject();
        //textGO.transform.parent = transform;
        //textGO.AddComponent<TextMeshProUGUI>();
        //textGO.transform.name = "Ammo Counter";
        //text = textGO.GetComponent<TextMeshPro>();
        text.SetText(currentAmmo.ToString());
        text.font = m_Font;
        text.color = Color.cyan;
        text.fontSize = 7;
        text.alignment = TextAlignmentOptions.Center;
    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }


        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        animator.SetBool("isReloading", true);

        yield return new WaitForSeconds(reloadTime - 0.25f);

        animator.SetBool("isReloading", false);

        yield return new WaitForSeconds(0.25f);

        currentAmmo = maxAmmo;
        text.SetText(currentAmmo.ToString());
        isReloading = false;
    }

    void Shoot()
    {
        muzzleFlash.Play();

        currentAmmo--;
        text.SetText(currentAmmo.ToString());

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Enemy tempEnemy = hit.transform.GetComponent<Enemy>();
            if (tempEnemy != null)
            {
                tempEnemy.takeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

        }
    }

}
