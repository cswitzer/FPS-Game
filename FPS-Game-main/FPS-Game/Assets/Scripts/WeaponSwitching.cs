using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    // default to first weapon
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
        SwitchWeapons(previousSelectedWeapon);
    }

    private void SwitchWeapons(int previousSelectedWeapon)
    {
        // checks if players scrolls mouse wheel to change weapon
        SwitchWeaponsByScroll();

        // checks if players press a number key to switch weapons
        SwitchWeaponByKey();

        // if we switched to a new weapon, then activate that weapon and deactivate all others
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void SwitchWeaponsByScroll()
    {
        // if we scroll up
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // e.g. if we have 3 weapons, we will check if weapon our selected weapon index is higher
            // than our current amount of weapons (index 0, 1, then 2)
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else
            {
                selectedWeapon++;
            }
        }
        // if we scroll down
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectedWeapon <= 0)
            {
                // go to our last weapon
                selectedWeapon = transform.childCount - 1;
            }
            else
            {
                selectedWeapon--;
            }
        }
    }

    private void SwitchWeaponByKey()
    {
        // if the one key is pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }
        // checking to see if we have more than one weapon
        else if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedWeapon = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 2)
        {
            selectedWeapon = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 2)
        {
            selectedWeapon = 3;
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        // take all transforms that are children to the parent transform
        foreach (Transform weapon in transform)
        {
            // if i does not equal selected weapon, then those weapons are disabled
            if (i == selectedWeapon)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
