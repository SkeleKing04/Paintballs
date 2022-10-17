using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class GunHandler : MonoBehaviour
{
    public bool isClient;
    public List<GunSO> gunList;
    public List<int> ammoInWeapon;
    public int gunIndex;
    public TextMeshProUGUI gunNameText, gunAmmoText;
    public Image gunAmmoFill;
    public float scrollChangeThreashhold;
    public bool invertScroll;
    public MeshFilter gunMeshDisplay;
    
    void Start()
    {
        changeGun(0);
        updateUIElements();
    }
    public void giveGun(GunSO GivenGun, bool forceSwitch)
    {
        if(!gunList.Contains(GivenGun))
        {
            gunList.Add(GivenGun);
            ammoInWeapon.Add(GivenGun.ammo);
        }
        if(forceSwitch)
        {
            changeGun(gunList.IndexOf(GivenGun));
        }
    }
    public void removeGun(GunSO gunToTake)
    {
        if(gunList.Contains(gunToTake))
        {
            int index = gunList.IndexOf(gunToTake);
            gunList.RemoveAt(index);
            ammoInWeapon.RemoveAt(index);
            changeGun(gunIndex);
        }
    }
    public void shootWeapon()
    {
        ammoInWeapon[gunIndex]--;
        if(isClient) updateUIElements();
    }
    public void updateUIElements()
    {
        gunNameText.text = gunList[gunIndex].name.ToUpper();
        gunAmmoText.text = "" + ammoInWeapon[gunIndex] + "/" + gunList[gunIndex].ammo;
    }
    public void Update()
    {
        if(Input.GetAxisRaw("Mouse ScrollWheel") > scrollChangeThreashhold || Input.GetAxisRaw("Mouse ScrollWheel") < -scrollChangeThreashhold)
        {
            //Mathf.Clamp(gunIndex + 1 * Input.GetAxisRaw("Mouse ScrollWheel") * Convert.ToInt32(invertScroll),0,gunList.Count);
            Debug.Log("Mouse Input = " + Input.GetAxisRaw("Mouse ScrollWheel") * 10);
            changeGun(Convert.ToInt32(Input.GetAxisRaw("Mouse ScrollWheel") * 10));
            
        }
    }
    private void changeGun(int newIndex)
    {
        newIndex = gunIndex + newIndex;
        if(newIndex < 0) newIndex = gunList.Count - 1;
        if(newIndex > gunList.Count - 1) newIndex = 0;
        gunIndex = newIndex;
        gunMeshDisplay.mesh = gunList[gunIndex].modelPrefab.GetComponent<MeshFilter>().sharedMesh;
        updateUIElements();
    }
}
