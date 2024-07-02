using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Mirror.ShootMe
{
    public class FireBallManagement : NetworkBehaviour
    {
        [SerializeField] private Animator animationSource;

        [SerializeField] private float speedSource, lifeTimeSource;

        [SerializeField] private LayerMask solid;

        private void Awake()
        {
            Invoke("ExecuteDestroy", lifeTimeSource);
        }

        private void Update()
        {
            if (!isServer) { return; }

            transform.Translate(Vector3.forward * speedSource * Time.deltaTime);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, solid))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.gameObject.GetComponent<PlayerManagement>().TakeDamage(1);

                    ExecuteDestroy();
                }
            }
        }

        private void ExecuteDestroy()
        {
            NetworkServer.Destroy(this.gameObject);
        }
    }
}
