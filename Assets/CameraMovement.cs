using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] float scrollSpeed = 15f;
    //Camera mainCamera;
    CursorAffordance cursorAffordance;
    public bool mouseRotationActive = false;
    bool activatorRunning = false;

    void Start()
    {
        //mainCamera = Camera.main;
        cursorAffordance = GetComponentInChildren<CursorAffordance>();
    }

    void LateUpdate () {
        ProcessCameraMovement();
	}

    void ProcessCameraMovement()
    {
        float xThrow = Input.GetAxis("Horizontal");
        float zThrow = Input.GetAxis("Vertical");

        float xOffset = xThrow * scrollSpeed * Time.deltaTime;
        float zOffset = zThrow * scrollSpeed * Time.deltaTime;

        float newXpos = transform.position.x + xOffset;
        float newZpos = transform.position.z + zOffset;
        
        transform.position = new Vector3(newXpos, 0f, newZpos);

        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (activatorRunning == false)
            {
                mouseRotationActive = true;
                StartCoroutine(MouseRotator());
            }
        } else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            mouseRotationActive = false;
            activatorRunning = false;
            StopCoroutine("MouseRotator");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (cursorAffordance.hasUnitSelected == true)
            {
                transform.position = cursorAffordance.selectedUnits[0].transform.position;
            }
        }
    }

    IEnumerator MouseRotator()
    {
        activatorRunning = true;
        while (mouseRotationActive == true)
        {
            float mouseXThrow = Input.GetAxis("Mouse X");
            float mouseYThrow = Input.GetAxis("Mouse Y");
            float mouseScrollSpeed = scrollSpeed * 10f;

            float mouseXOffset = mouseXThrow * mouseScrollSpeed * Time.deltaTime;
            float mouseYOffset = mouseYThrow * mouseScrollSpeed * Time.deltaTime;

            transform.Rotate(mouseYOffset, 0, 0, Space.Self);
            transform.Rotate(0, mouseXOffset, 0, Space.World);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
