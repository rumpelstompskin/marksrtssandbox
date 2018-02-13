using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRayCaster))]
public class CursorAffordance : MonoBehaviour {

    CameraRayCaster cameraRaycaster;


    public GameObject[] selectedUnits = null;
    public bool hasUnitSelected = false;

    void Start()
    {
        selectedUnits = new GameObject[24];
        cameraRaycaster = GetComponent<CameraRayCaster>();
        cameraRaycaster.layerChangeObservers += OnLayerChange;
    }

    void OnLayerChange (Layer newLayer, GameObject gameObject) { // TODO Consider de-registering this method.
        switch (newLayer)
        {
            case Layer.Vessels:
                UnitSelection(gameObject);
                break;
            case Layer.Resources:
                print("Harvesting resources.");
                break;
            case Layer.Infrastructure:
                print("Docking permission requested.");
                break;
            case Layer.RaycastEndStop:
                UnitSelection(gameObject);
                break;
            default:
                print("Shouldn't be getting here.");
                return;
        }
	}

    void UnitSelection(GameObject gameObject)
    {
        if (gameObject == null && hasUnitSelected == false) { return; }
        if (hasUnitSelected == false)
        {
            print("Selecting " + gameObject);
            selectedUnits[0] = gameObject;
            hasUnitSelected = true;
        } else if (hasUnitSelected == true)
        {
            print("Deselecting units");
            selectedUnits[0] = null;
            hasUnitSelected = false;
        }
    }
}
