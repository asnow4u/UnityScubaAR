using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ARPlaneManager))]

public class PrefabPlacement : MonoBehaviour
{
  [SerializeField]
  private ARPlaneManager arPlaneManager;

  [SerializeField]
  private Button toggleButton;

  [SerializeField]
  private Camera arCamera;

  private ARRaycastManager arRaycastManager;

  private bool screenTouch = false;

  public GameObject bubblePrefab;
  public List<GameObject> prefabObjects = new List<GameObject>();

  private struct PlaneIdCount {
    public string id;
    public List<GameObject> placedObjects;
    public float timeInterval;

    public PlaneIdCount(string id){
      this.id = id;
      this.placedObjects = new List<GameObject>();
      this.timeInterval = Time.time;
    }

    public PlaneIdCount(string id, List<GameObject> gameObjectList){
      this.id = id;
      this.placedObjects = gameObjectList;
      this.timeInterval = Time.time;
    }
  };

  private List<PlaneIdCount> planeIdCount = new List<PlaneIdCount>();


  void Awake() {
    arRaycastManager = GetComponent<ARRaycastManager>();
    arPlaneManager = GetComponent<ARPlaneManager>();
    arPlaneManager.planesChanged += PlaneChanged;
    // arPlaneManager.enabled = false;
  }


  //TODO bind this function to a button ie: toggleButton.onCLick.AddListener(TogglePlaneDetection);
  private void TogglePlaneDetection() {
    arPlaneManager.enabled = true;
  }


  private void PlaneChanged(ARPlanesChangedEventArgs args) {

    if (args.updated.Count > 0) {
      for (int i=0; i<args.updated.Count-1; i++) {

        ARPlane arPlane = args.updated[i];

        if (planeIdCount.Count > 0){

          var planeIndex = planeIdCount.FindIndex(id => id.id == arPlane.trackableId.ToString());

          if ( planeIndex != -1){
            PlaceObjects(planeIndex, arPlane.size.x, arPlane.size.y, arPlane.transform.position);
          }

          //Plane not in struct list, add to list
          //Based on size spawn nessisary gameObjects
          else {
            PlaneIdCount tempPlaneId = new PlaneIdCount(arPlane.trackableId.ToString());
            planeIdCount.Add(tempPlaneId);
            PlaceObjects(planeIdCount.Count-1, arPlane.size.x, arPlane.size.y, arPlane.transform.position);

          }
        }

        else {
          PlaneIdCount tempPlaneId = new PlaneIdCount(arPlane.trackableId.ToString());
          planeIdCount.Add(tempPlaneId);
          PlaceObjects(planeIdCount.Count-1, arPlane.size.x, arPlane.size.y, arPlane.transform.position);

        }
      }
    }
  }


  private void PlaceObjects(int index, float x, float y, Vector3 pos) {

    var area = x*y;

    while(area / 1.0f > planeIdCount[index].placedObjects.Count) {
      int rand = Random.Range(0, prefabObjects.Count);
      planeIdCount[index].placedObjects.Add(Instantiate(prefabObjects[rand], pos + new Vector3(Random.Range(-1.0f, 1.0f) * x / 2f, 0f, Random.Range(-1.0f, 1.0f) * y / 2f), prefabObjects[rand].transform.rotation));
    }
  }


  private void Update() {

    if (Input.touchCount > 0) {
      Touch touch = Input.GetTouch(0);

      if (touch.phase == TouchPhase.Began) {
        screenTouch = true;
      }

      if (touch.phase == TouchPhase.Ended) {
        screenTouch = false;
      }

      SpawnBubbleOnTouch(touch.position);
    }



    if (planeIdCount.Count > 0){
      for (int i=0; i<planeIdCount.Count-1; i++){

        if (planeIdCount[i].placedObjects.Count > 0) {
          if (planeIdCount[i].timeInterval + 2f < Time.time){

            planeIdCount[i] = new PlaneIdCount(planeIdCount[i].id, planeIdCount[i].placedObjects);
            int rand = Random.Range(0, planeIdCount[i].placedObjects.Count);

            Instantiate(bubblePrefab, planeIdCount[i].placedObjects[rand].transform.position, Quaternion.identity);
          }
        }
      }
    }
  }


  private void SpawnBubbleOnTouch(Vector2 touchPos) {

    if (screenTouch) {

      List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();

      if (arRaycastManager.Raycast(touchPos, arRaycastHits)) {
        var pose = arRaycastHits[0].pose;
        Instantiate(bubblePrefab, pose.position, Quaternion.identity);
      }
    }
  }

}
