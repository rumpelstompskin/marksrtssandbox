using System.Collections;
using UnityEngine;

public class CameraRayCaster : MonoBehaviour
{
    public Layer[] layerPriorities = { // Different layer for different actions.
        Layer.Vessels,
        Layer.Resources,
        Layer.Infrastructure,
        Layer.RaycastEndStop
    };


    [SerializeField] float distanceToBackground = 100f;
    //[SerializeField] float unitMoveSpeed = 10f;
    UnitStats unitStats;
    bool unitIsMoving = false;
    Camera viewCamera; // Ref to our main camera
    CursorAffordance cursorAffordance;

    RaycastHit raycastHit;
    public RaycastHit hit
    {
        get { return raycastHit; }
    }

    Layer raycasterLayerHit;
    public Layer layerHit
    {
        get { return raycasterLayerHit; }
    }

    public delegate void OnLayerChange(Layer newLayer, GameObject gameObject); // Sets our cursor delegate.
    public event OnLayerChange layerChangeObservers; // Instantiate the delegate.

    void Start()
    {
        viewCamera = Camera.main; // Setup the referance to our main camera.
        cursorAffordance = GetComponent<CursorAffordance>(); // Referance to our CursorAffordance script.
    }

    void Update()
    {
        ProcessMouseClicks(); // TODO can we move this out of update? aka do we have to run this every frame?
    }

    private void ProcessMouseClicks() // TODO Separate in a different script maybe?
    {
        if (Input.GetMouseButtonUp(0)) // Process left click | Change: Switched from down to up.
        {
            // Look for and return priority layer hit
            foreach (Layer layer in layerPriorities)
            {
                var hit = RaycastForLayer(layer); // stores the value of a layer raycast.
                if (hit.HasValue) // checks if we hit a stored layer.
                {
                    raycastHit = hit.Value; // Stores the raycast hit value.
                    if (layerHit != layer) // Checks if the layer we hit doesn't equal to our enumerator.
                    {
                        raycasterLayerHit = layer; // stores the layer we hit as a layer.
                        layerChangeObservers(layer, GetClickedGameObject(layer)); // Executes a delegate from the script CursorAffordance and sets the data: Which layer we hit? and Which gameObject we hit?
                    }

                    return; // Return if we have no value.
                }
            }
            raycastHit.distance = distanceToBackground; // TODO Don't remember the purpose of this. Delete maybe?
            raycasterLayerHit = Layer.RaycastEndStop; // We clicked in the emptiness. Set the layer to void.
            layerChangeObservers(raycasterLayerHit, GetClickedGameObject(raycasterLayerHit)); // Calls our delegates with the void as value.
        }
        if (Input.GetMouseButton(1)) // Process Right click | Change: Switched from GetMouseButtonDown to not Down.
        {
            if (cursorAffordance.hasUnitSelected == true) // Checks is the player has a unit selected. Variable is located in CursorAffordance Script.
            {
                Vector3 currentMousePos = GetCurrentMousePosition().GetValueOrDefault(); ; // Stores our current mouse position by using a get method.
                GameObject currentSelectedUnit = cursorAffordance.selectedUnits[0]; // stores our current selected unit.
                if (unitIsMoving == false) // Is our selected unit currently moving?
                {
                    unitIsMoving = true;
                    StartCoroutine(ProcessMovement(currentSelectedUnit.transform.position, currentMousePos, currentSelectedUnit)); // Process Selected unit Movement.
                }
                if (unitIsMoving == true)
                {
                    unitIsMoving = false;
                    StopCoroutine("ProcessMovement");
                }
            }
        }
    }

    IEnumerator ProcessMovement(Vector3 currentPos, Vector3 currentMousePos,GameObject currentSelectedUnit) // TODO Separate into a different script maybe?
    {
        unitStats = currentSelectedUnit.GetComponent<UnitStats>();

        while (currentPos != currentMousePos && unitIsMoving == true) // As long as the unit isn't in position.
        {
            print("Our current position is : " + currentPos);
            print("Our current destination is : " + currentMousePos);
            var moveSpeed = unitStats.unitMovementSpeed * Time.deltaTime;
            currentSelectedUnit.transform.position = Vector3.MoveTowards(currentSelectedUnit.transform.position, currentMousePos, moveSpeed); // Move the unit at X speed.
             // Unit is in movement.
            yield return new WaitForSeconds(0.01f); // Wait for Fixed Update.
        }
    }

    private Vector3? GetCurrentMousePosition() // Returns the current mouse position in the world against a plane.
    {
        var ray = viewCamera.ScreenPointToRay(Input.mousePosition); // Stores the value of a raycast through the screen.
        var plane = new Plane(Vector3.up, Vector3.zero); // Stores an invisible plane that we use to register our raycast.

        float rayDistance; // TODO set to mathf.infinity?
        if (plane.Raycast(ray, out rayDistance)) // Check our ray against the plane.
        {
            return ray.GetPoint(rayDistance); // outputs the value.
        }
        return null;
    } 

    RaycastHit? RaycastForLayer(Layer layer) // TODO Figure out a way to merge both raycast actions.
    {
        int layerMask = 1 << (int)layer; // See Unity docs for mask formation
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); // stores a ray cast into the screen.

        RaycastHit hit; // used as an out parameter
        bool hasHit = Physics.Raycast(ray, out hit, distanceToBackground, layerMask); // checks if we hit a layer as a bool.
        if (hasHit) // Did we hit?
        {
            return hit; // If so return the value of our RaycastHit hit get method.
        }
        return null; // if not return null.
    }

    GameObject GetClickedGameObject(Layer layer) // Returns the gameobject selected by the player.
    {
        // Builds a ray from camera point of view to the mouse position 
        int layerMask = 1 << (int)layer; // TODO check if we really need this here?
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition); // stores a ray into the screen.
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanceToBackground, layerMask)) // Casts the ray and get the first game object hit 
        {
            return hit.transform.gameObject; // return the gameObject we just hit with our ray.
        }
        else
        {
            return null;
        }
    }
}