using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraFollowPersonage : MonoBehaviour
{
    public float Speed = 5;
    public float CamRotSpeed = 15;
    public Transform PlayerTarget;
    private Vector3 StartPos;
    private bool canLook = true;
    public float RotySwipe;

    private IDOrbitCamera orbit;

    private void Awake()
    {
        StartPos = transform.position;
        orbit = GetComponent<IDOrbitCamera>();
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.localEulerAngles.y, 0);
    }

    private void Update()
    {
        if (PlayerTarget != null)
        {
            if (!orbit.enabled)
            {
                transform.position = Vector3.Lerp(transform.position, StartPos, Speed * Time.deltaTime);
                orbit.target = PlayerTarget;
                orbit.secondaryTarget = PlayerTarget;
                orbit.enabled = true;
            }
        }
    }

    public void SetTarget(Transform trg, Transform trgPlayer)
    {
        //Target = trg;
        //PlayerTarget = trgPlayer;
    }

    public void RightSwipe()
    {
        if (IsPointerOverUIObject())
            return;

        canLook = false;

        //RotySwipe += CamRotSpeed * Time.deltaTime;
    }

    public void LeftSwipe()
    {
        if (IsPointerOverUIObject())
            return;

        canLook = false;
        //RotySwipe -= CamRotSpeed * Time.deltaTime;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
