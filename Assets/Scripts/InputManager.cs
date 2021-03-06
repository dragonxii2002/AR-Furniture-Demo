using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class InputManager : ARBaseGestureInteractable
{
    //[SerializeField] private GameObject arObj;
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] private GameObject crosshair;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private Touch touch;
    private Pose pose;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture) //Check of the Object being Tapped is already placed in the Scene.
    {
        if(gesture.TargetObject == null)
            return true;
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.WasCancelled) //if no Gesture (Nothing happened), then return.
            return;
        if (gesture.TargetObject != null || IsPointerOverUI(gesture)) //if the Target Object is already in the Scene, then return; if the Gesture is Tapping on the UI, then return.
        {
            return;
        }
        if (GestureTransformationUtility.Raycast(gesture.startPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            GameObject placedObj = Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation); //place the GameObject to the Scene with the fixed position and rotation.

            var anchorObject = new GameObject("PlacementAnchor");          //define the reference point of an anchor. Looking for an anchor and Object going into the same place
            anchorObject.transform.position = pose.position;
            anchorObject.transform.rotation = pose.rotation;
            placedObj.transform.parent = anchorObject.transform; //Finally anchor is going to be a parent and going to hold the placedObj.
        }
    }

    //void Update() //Comment out because of using raycasting, change to void FixedUpdate.
    //{
    //    CrosshairCalculation();
    //    touch = Input.GetTouch(0);

    //    if (Input.touchCount < 0 || touch.phase != TouchPhase.Began)
    //        return;

    //    if (IsPointerOverUI(touch)) return;

    //    Instantiate(DataHandler.Instance.GetFurniture(), pose.position, pose.rotation);
    //}

    void FixedUpdate()
    {
        CrosshairCalculation();
    }

    bool IsPointerOverUI(TapGesture touch) //Gesture Function is using XR Toolkit.
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void CrosshairCalculation()
    {
        Vector3 origin = arCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        //Ray ray = arCam.ScreenPointToRay(origin); //comment out because we do not need this using XR Toolkit.
        if (GestureTransformationUtility.Raycast(origin, _hits, TrackableType.PlaneWithinPolygon)) //Gesture Function is using XR Toolkit.
        {
            pose = _hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.eulerAngles = new Vector3(90, 0, 0); //Just let the Marker moving align with the ground.
        }

    }
}
