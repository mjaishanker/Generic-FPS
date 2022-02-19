using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileGun : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] GameObject bullet;

    [Header("Bullet Force")]
    [SerializeField] float shootForce;
    [SerializeField] float upwardForce;

    [Header("Gun Stats")]
    [SerializeField] float timeBetweenShooting;
    [SerializeField] float spread;
    [SerializeField] float reloadTime;
    [SerializeField] float timeBetweenShots;
    [SerializeField] int magazineSize;
    [SerializeField] int bulletsPerTap;
    [SerializeField] bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    // Bools
    bool shooting, readyToShoot, reloading;

    [Header("Reference")]
    [SerializeField] Camera cam;
    [SerializeField] Transform attackPoint;

    [Header("Bug Fixing")]
    [SerializeField] bool allowInvoke = true;

    [Header("Graphics")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject impactEffect;
    [SerializeField] TextMeshPro ammunitionDisplay;
    [SerializeField] RawImage ammunitionAR;
    [SerializeField] TMP_FontAsset m_Font;
    [SerializeField] Animator animator;


    private void Awake()
    {
        // Make sure magazine is full
        bulletsLeft = magazineSize;
        readyToShoot = true;

        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft.ToString());
            ammunitionDisplay.font = m_Font;
            ammunitionDisplay.color = Color.cyan;
            ammunitionDisplay.fontSize = 7;
            ammunitionDisplay.alignment = TextAlignmentOptions.Center;
        }

        else if (ammunitionAR != null)
        {
            //Debug.Log("Bullets: " + bulletsLeft + " Mag Size: " + magazineSize + " Bullets / Mag Size: " + ((float)bulletsLeft / magazineSize));
            ammunitionAR.transform.localScale = new Vector3((float)bulletsLeft / magazineSize, 1f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (bulletsLeft <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        MyInput();
    }

    private void OnEnable()
    {
        shooting = false;
        readyToShoot = true;
        reloading = false;
        animator.SetBool("Reloading", false);
    }

    private void MyInput()
    {
        // Check if allowed to hold down button and take corresponding input
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) StartCoroutine(Reload());
        // Reload automatically
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) StartCoroutine(Reload());



        if(readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            // Set bullets shot to 0
            bulletsShot = 0;

            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        // Find exact hit position using raycast
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Check if ray hit something
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        // Direction of the bullet
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        // Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        // Direction of bullet with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        // Instantiate bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.GetComponent<CustomBullet>().sourceTag = "Player";

        // Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithoutSpread.normalized;

        // Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithoutSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);

        // Play Muzzle Flash
        muzzleFlash.Play();

        bulletsLeft--;
        bulletsShot++;

        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft.ToString());

        if (ammunitionAR != null)
        {
            //Debug.Log("Bullets: " + bulletsLeft + " Mag Size: " + magazineSize + " Bullets / Mag Size: " + ((float)bulletsLeft / magazineSize));
            ammunitionAR.transform.localScale = new Vector3((float)bulletsLeft / magazineSize, 1f, 1f);
        }

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    IEnumerator Reload()
    {
        reloading = true;

        animator.SetBool("isReloading", true);

        yield return new WaitForSeconds(reloadTime - 0.25f);

        animator.SetBool("isReloading", false);

        yield return new WaitForSeconds(0.25f);

        bulletsLeft = magazineSize;
        if (ammunitionDisplay != null)
            ammunitionDisplay.SetText(bulletsLeft.ToString());

        if (ammunitionAR != null)
        {
            //Debug.Log("Bullets: " + bulletsLeft + " Mag Size: " + magazineSize + " Bullets / Mag Size: " + ((float)bulletsLeft / magazineSize));
            ammunitionAR.transform.localScale = new Vector3((float)bulletsLeft / magazineSize, 1f, 1f);
        }
        reloading = false;
    }

    //private void Reload()
    //{
    //    reloading = true;
    //    Invoke("ReloadingFinished", reloadTime);
    //}

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

}
