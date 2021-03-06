﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://www.c7pixel.com/post/first_person_shooting_in_unity_using_raycast

public class AttackController : MonoBehaviour
{
    // The current weapon our 
    // player is using
    public Weapon currentWeapon;
    public GameObject activeWeapon;
    // The number of bullets the 
    // player has available for reloading
    public int currentExtraBullets;
    // The number of extra 
    // bullets the player CAN have
    public int maxExtraBullets = 100;
    // Used to check if 
    // the player wants to shoot
    private bool shoot;
    // Used to check if the 
    // player should wait 
    // before shooting another bullet
    private bool isShooting;
    // Used to check if 
    // the player is reloading
    private bool isReloading;
    // This variable will store 
    // information about raycast hits.
    private RaycastHit hit;

    public GameObject MuzzleFlashObject;
    private float MuzzleFlashTimer = 0.1f;
    private float MuzzleFlashTimerStart;
    public bool muzzleFlashEnabled = false;

    public AudioSource gunSound;
    public AudioSource reloadSound;
    
    public GameObject reloadTextObject;
    public bool needReload = false;

    public GameObject hitImage;
    
    void Start()
    {
        currentExtraBullets = maxExtraBullets;
        MuzzleFlashTimerStart = MuzzleFlashTimer;
    }

    void Update()
    {
        if (Time.timeScale != 0) {

            // If the player click the fire 
            // button (mouse left click) 
            // we set 'fire=true'
            // and if the player release 
            // the fire button we set 'shoot=false'
            // and last if shoot is true 
            // we run our 'Shoot()' function
            // By doing so can we make the 
            // player shoot as long the 
            // fire button haven't been released.
            if (Input.GetButtonDown("Fire1"))
            
            {
                if (activeWeapon.activeSelf == true) {
                    shoot = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                shoot = false;
            }

            if (shoot)
            {
                Shoot();
            }

            if (muzzleFlashEnabled == true) {

                MuzzleFlashObject.SetActive(true);
                MuzzleFlashTimer -= Time.deltaTime;
            }

            if (MuzzleFlashTimer <= 0) {

                MuzzleFlashObject.SetActive(false);
                muzzleFlashEnabled = false;
                MuzzleFlashTimer = MuzzleFlashTimerStart;
            }

            // if we click the 'R' button 
            // on the keyboard we run our reload function
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }
    }

    void Shoot()
    {
        // Return until the fire cooldown 
        // is gone and if we don't have a weapon
        // OR
        // if the player is reloading 
        // the weapon or the weapon's magazin is empty
        if (isShooting || currentWeapon == null ||
            isReloading || currentWeapon != null &&
            currentWeapon.currentBullets <= 0)
        {
            reloadTextObject.SetActive(true);
            return;
        }

        muzzleFlashEnabled = true; //enable muzzle flash
        gunSound.Play();
        
        // remove a bullet from the weapon's magazin
        currentWeapon.currentBullets -= 1;

        // If a weapon doesn't have a 
        // fire rate we assume it's 
        // a non-automatic weapon
        // and we set 'shoot=false' 
        // to implement a 'tap-to-shoot' functionality
        if (currentWeapon.fireRate == 0)
        {
            shoot = false;
        }

        // We set up a Ray which moves 
        // towards the center of the main camera
        Ray ray = Camera.main.ViewportPointToRay(new Vector2(.5f, .5f));
        // We set the origin of the ray 
        // to gun barrel position so it 
        // doesn't start at the camera position
        ray.origin = currentWeapon.Gun_Barrel.position;
        // We fire the raycast in an 'if' 
        // statement and pass the 'ray', 
        // our RaycastHit variable
        // and we limit the distance of 
        // the raycast to match the 
        // current weapon's fire length
        if (Physics.Raycast(ray, out hit, currentWeapon.fireLength))
        {
            // if the raycast it's a game 
            // object we check if it has the player tag
            if (hit.collider.CompareTag("Player"))
            {
                // and finally if it is a player, 
                // we want to add some damage to him/her
                // I won't explain how to build a 
                // health controller class in this tutorial,
                // but you would be able to access 
                // one like this if the player had one:
                // var healthCtrl = hit.collider.GetComponent<HealthController>();
                // healthCtrl.ApplyDamage(currentWeapon.damage);
            }
        }

        // We set 'isShooting=true' and run FireCooldown 
        // as an IEnumerator to use the WaitForSeconds class
        isShooting = true;
        StartCoroutine(FireCooldown());
    }

    IEnumerator FireCooldown()
    {
        // The weapon will be ready to fire 
        // another bullet when 'isShooting=false'
        yield return new WaitForSeconds(currentWeapon.fireRate);
        isShooting = false;
    }

    void Reload()
    {
        // return the function if the 
        // player already is reloading
        // OR the player don't have a weapon
        // OR the player have a 
        // weapon that already is full
        // OR the player don't have 
        // any extra bullets to 
        // insert into the weapon's magazin
        if (isReloading || currentWeapon == null ||
            currentWeapon != null && currentWeapon.currentBullets
            >= currentWeapon.maxBullets ||
            currentExtraBullets <= 0)
        {
            return;
        }

        reloadSound.Play();

        // The first thing we need to know 
        // is how many bullets we can 
        // insert into the player's weapon
        var diff = currentWeapon.maxBullets -
                    currentWeapon.currentBullets;

        // Then we check if the number of 
        // bullets missing is less or 
        // equal to the number our player has
        // we know if this is true, the player 
        // must have more or the exact 
        // number of extra bullets as
        // the current weapon is missing and 
        // we can therefore just set the 
        // number of bullets on the weapon
        // to the max number of bullets 
        // the weapon's magazin can store
        if (diff <= currentExtraBullets)
        {
            currentExtraBullets -= diff;
            currentWeapon.currentBullets = currentWeapon.maxBullets;
        }
        else
        {
            // if the player doesn't have more 
            // or the exact number of 
            // bullets it's either 0 or greater
            // and we can safely add the 
            // current number of extra 
            // bullets to our weapon and set
            // the player's number 
            // of extra bullets to 0
            currentWeapon.currentBullets += currentExtraBullets;
            currentExtraBullets = 0;
        }

        // And then we do the exact same 
        // thing here as in the Shoot() function
        // We could just have added a function 
        // to handle this, but it might be easier
        // to implement other functionality to 
        // the reload or shoot function 
        // if you keep the seperated
        isReloading = true;
        StartCoroutine(ReloadCooldown());
        reloadTextObject.SetActive(false);
    }

    IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        isReloading = false;
    }
}