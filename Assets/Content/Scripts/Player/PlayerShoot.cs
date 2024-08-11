using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float Damage = 15;
    [Space]
    public float fireRate = 0.25f;
    public float weaponRange = 50f;
    public float RandomizeVectorMin = 1;
    public float RandomizeVectorMax = 1;
    public Transform GunEnd;
    [Space]
    public GameObject ImpactPart;
    public ParticleSystem Muzzle;
    [Space]
    public PlayerAudio PlayerAudio;

    private float nextFire;

    private void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            Muzzle.Play();
            PlayerAudio.PlaySnd("Shoot");
            nextFire = Time.time + fireRate;

            float rand = Random.Range(RandomizeVectorMin, RandomizeVectorMax);

            Vector3 rayOrigin = GunEnd.transform.position;
            RaycastHit hit;

            Vector3 forw = new Vector3(GunEnd.transform.forward.x, GunEnd.transform.forward.y, GunEnd.transform.forward.z);

            Debug.DrawRay(rayOrigin, forw * weaponRange, Color.yellow, 0.3f);

            if (Physics.Raycast(rayOrigin, forw, out hit, weaponRange))
            {
                if (hit.transform.gameObject.GetComponent<HealthModule>() != null)
                {
                    hit.transform.gameObject.GetComponent<HealthModule>().Damage(-Damage);
                }

                if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                {
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(-hit.normal * 100, ForceMode.Impulse);
                }

                GameObject go = Instantiate(ImpactPart, hit.point, Quaternion.identity);
            }
        }
    }

    public void GunEndCorrect(bool correct)
    {
        if (correct)
        {
            GunEnd.transform.localEulerAngles = new Vector3(10, 0, 0);
        }
        else
        {
            GunEnd.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }
}