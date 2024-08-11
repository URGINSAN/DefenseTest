using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10.0f;
    private Vector3 movement;
    [Space]
    public float xInput;
    public float zInput;
    Vector3 direction;
    [Space]
    public Transform CamParent;
    public Camera Cam;
    private Vector3 MousePos;
    [Space]
    public bool CanStep = true;
    public float StepDelay = 0.5f;
    [Space]
    public PlayerAnimation PlayerAnim;
    public PlayerShoot PlayerShoot;
    public PlayerAudio PlayerAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        CamParent.transform.SetParent(null);
    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000.0f))
        {
            MousePos = hit.point;
        }

        Vector3 lookVector = MousePos - transform.position;
        lookVector.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
    }

    void FixedUpdate()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        zInput = Input.GetAxisRaw("Vertical");

        direction = xInput * transform.right + zInput * transform.forward;
        direction = direction.normalized * Mathf.Min(1f, direction.magnitude);

        rb.MovePosition(transform.position + speed * Time.deltaTime * direction);

        if (xInput == 0 && zInput == 0)
        {
            PlayerAnim.PlayAnim("Rifle Idle");
            PlayerShoot.GunEndCorrect(false);
        }
        else
        {
            PlayerShoot.GunEndCorrect(true);

            if (CanStep)
            {
                PlayerAudio.PlaySnd("Move");

                IEnumerator IE()
                {
                    CanStep = false;
                    yield return new WaitForSeconds(StepDelay);
                    CanStep = true;
                }

                StartCoroutine(IE());
            }
        }
        if (xInput == 0 && zInput > 0)
            PlayerAnim.PlayAnim("Run Forward");
        if (xInput < 0 && zInput == 0)
            PlayerAnim.PlayAnim("Run Left");
        if (xInput > 0 && zInput == 0)
            PlayerAnim.PlayAnim("Run Right");
        if (xInput < 0 && zInput > 0)
            PlayerAnim.PlayAnim("Run Forward Left");
        if (xInput > 0 && zInput > 0)
            PlayerAnim.PlayAnim("Run Forward Right");

        if (xInput == 0 && zInput < 0)
            PlayerAnim.PlayAnim("Run Backward");
        if (xInput < 0 && zInput < 0)
            PlayerAnim.PlayAnim("Run Backward Left");
        if (xInput > 0 && zInput < 0)
            PlayerAnim.PlayAnim("Run Backward Right");
    }
}