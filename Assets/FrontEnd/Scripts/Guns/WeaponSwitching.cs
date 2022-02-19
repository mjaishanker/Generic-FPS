using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
            selectedWeapon = 1;

        if(previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        if(selectedWeapon == 0)
        {
            transform.GetChild(0).transform.gameObject.SetActive(true);
            transform.GetChild(1).transform.gameObject.SetActive(false);
        }
        else if(selectedWeapon == 1)
        {
            transform.GetChild(0).transform.gameObject.SetActive(false);
            transform.GetChild(1).transform.gameObject.SetActive(true);
        }
    }
}
