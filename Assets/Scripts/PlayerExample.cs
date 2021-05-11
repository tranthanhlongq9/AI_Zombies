﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerExample : MonoBehaviour
{
    public AudioClip shootSound;
    public float soundIntensity = 5f;
    public float walkEnemyPerceptionRadius = 1f;
    public float sprintEnemyPerceptionRadius = 2f;
    public LayerMask zombieLayer;

    private AudioSource audioSource;
    private FirstPersonController fpsc;
    private SphereCollider sphereCollider;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        fpsc = GetComponent<FirstPersonController>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        //bấm chuột trái thì sẽ gọi đến fire()
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
        if (fpsc.GetPlayerStealthProfile() == 0) // nếu player trả về 0 (đi bộ)xem code bên FirstPersonController.cs 243
        {
            //thay đổi bán kính
            sphereCollider.radius = walkEnemyPerceptionRadius;
        }
        else
        // ngược lại player trả về 1(chạy)
        {
            //thay đổi bán kính
            sphereCollider.radius = sprintEnemyPerceptionRadius;
        }
    }

    //xử lý bắn
    public void Fire()
    {
        audioSource.PlayOneShot(shootSound);
        Collider[] zombies = Physics.OverlapSphere(transform.position, soundIntensity, zombieLayer);
        for(int i = 0; i < zombies.Length; i++)
        {
            zombies[i].GetComponent<AIExample>().OnAware();
        }
    }

    //xử lý va chạm Trigger kích hoạt
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zombie"))
        {
            other.GetComponent<AIExample>().OnAware();
        }
    }

}
