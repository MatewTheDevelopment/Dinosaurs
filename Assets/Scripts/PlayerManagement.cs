using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Mirror;

namespace Mirror.ShootMe
{
    public class PlayerManagement : NetworkBehaviour
    {
        [SerializeField] private float playerSpeed;

        [SerializeField] private Vector3 offset;

        [SerializeField] private Slider statsSlider, fireRateSlider;

        [SerializeField] private Transform bulletSpawnPoint;

        [SerializeField] private Canvas mainCanvas, statsCanvas;

        [SerializeField] private GameObject runParticle, bulletPrefab;

        [SerializeField] private PlayerInput inputManagement;

        [SerializeField] private CharacterController controllerManagement;

        [SerializeField] private Animator animatorManagement;

        [SerializeField] private float shootDelay;

        [SerializeField] private int maxHealth;

        private int currentHealth;

        private float shootDelayTimer;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;

            mainCanvas.worldCamera = mainCamera;

            statsCanvas.worldCamera = mainCamera;

            currentHealth = maxHealth;

            statsSlider.value = statsSlider.maxValue;

            fireRateSlider.value = fireRateSlider.maxValue;
        }

        private void Update()
        {
            if (!isLocalPlayer) return;

            statsCanvas.transform.LookAt(mainCamera.transform);

            shootDelayTimer += Time.deltaTime;

            fireRateSlider.value = fireRateSlider.maxValue * shootDelayTimer / shootDelay;

            mainCamera.transform.localPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z) - offset;

            Vector2 input = inputManagement.actions["Movement"].ReadValue<Vector2>();

            Vector3 move = new Vector3(input.x, 0, input.y);

            controllerManagement.Move(move * Time.deltaTime * playerSpeed);

            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;

                animatorManagement.SetBool("Move?", true);

                runParticle.SetActive(true);
            }
            else
            {
                animatorManagement.SetBool("Move?", false);

                runParticle.SetActive(false);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Fire();
            }
        }

        private void Fire()
        {
            if (fireRateSlider.value >= fireRateSlider.maxValue)
            {
                animatorManagement.SetTrigger("Execute");

                shootDelayTimer = 0;

                fireRateSlider.value = 0;

                FireCommand();
            }
        }

        [Command]
        private void FireCommand()
        {
            GameObject projectile = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
            
            NetworkServer.Spawn(projectile);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            statsSlider.value = 100 * currentHealth / maxHealth;
        }
    }
}
