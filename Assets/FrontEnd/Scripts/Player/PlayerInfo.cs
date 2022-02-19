using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{

    [Header("Player Health")]
    [SerializeField] float maxPlayerHealth = 100;
    [SerializeField] float currentPlayerHealth;
    [SerializeField] RawImage playerHealthImage;
    [SerializeField] GameObject cameraHolder;


    private float rechargeTimer;
    private bool playerIsHealing;
    // Start is called before the first frame update
    void Start()
    {
        rechargeTimer = 0f;
        currentPlayerHealth = maxPlayerHealth;

        if (playerHealthImage != null)
        {
            //Debug.Log("Bullets: " + bulletsLeft + " Mag Size: " + magazineSize + " Bullets / Mag Size: " + ((float)bulletsLeft / magazineSize));
            playerHealthImage.transform.localScale = new Vector3((float)currentPlayerHealth / maxPlayerHealth, 1f, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPlayerHealth == maxPlayerHealth)
        {
            playerIsHealing = false;
            return;
        }
        else if (currentPlayerHealth < maxPlayerHealth && playerIsHealing == false)
        {
            StartCoroutine(HealPlayer());
            if (currentPlayerHealth > 100)
                currentPlayerHealth = 100;
        }


        //if (rechargeTimer == 0 && currentPlayerHealth != maxPlayerHealth)
        //    rechargeTimer += Time.deltaTime;
        //if(rechargeTimer <= 4.25f)
        //{
        //    StartCoroutine(HealPlayer());
        //    if (currentPlayerHealth > 100)
        //        currentPlayerHealth = 100;
        //}

    }

    public void PlayerTakeDamage(int damage)
    {
        currentPlayerHealth -= damage;

        if (playerHealthImage != null)
        {
            playerHealthImage.transform.localScale = new Vector3((float)currentPlayerHealth / maxPlayerHealth, 1f, 1f);
        }

        if (currentPlayerHealth <= 0) Invoke(nameof(DestroyPlayer), 0.5f);
    }

    void DestroyPlayer()
    {
        gameObject.SetActive(false);
        cameraHolder.SetActive(false);
        //Destroy(gameObject);
        //Destroy(cameraHolder);
    }

    IEnumerator HealPlayer()
    {
        yield return new WaitForSeconds(7);

        float timePassed = 0;
        while (timePassed <= 3)
        {
            // Code to go left here
            if (currentPlayerHealth < 100)
            {
                currentPlayerHealth += 20f;
                if (playerHealthImage != null)
                {
                    playerHealthImage.transform.localScale = new Vector3((float)currentPlayerHealth / maxPlayerHealth, 1f, 1f);
                }
            }

            timePassed += Time.deltaTime;

            playerIsHealing = true;

            yield return null;
        }

    }

}
