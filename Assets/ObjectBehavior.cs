using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectBehavior : MonoBehaviour
{
    private bool isSelected;
    public float y_offset;
    private GameObject position_marker;
    private Transform original_parent;

    // Start is called before the first frame update
    void Awake()
    {
        isSelected = false;
        position_marker = GameObject.Find("PositionMarker");
        // set the object to be selectable
        gameObject.layer = 10;
        if (y_offset == 0) {
            y_offset = gameObject.transform.position.y;
        }
        original_parent = gameObject.transform.parent;
        print("awaken");
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelected) {
            if (SteamVR_Actions._default.Left.GetState(SteamVR_Input_Sources.RightHand)) { // rotate clockwise 1 degree/frame
                gameObject.transform.Rotate(0, 1, 0, Space.Self);
            } else if (SteamVR_Actions._default.Right.GetState(SteamVR_Input_Sources.RightHand)) { // rotate counter-clockwise 1 degree/frame
                gameObject.transform.Rotate(new Vector3(0,-1,0));
            }
            if (SteamVR_Actions._default.Up.GetStateDown(SteamVR_Input_Sources.RightHand)) {
                AttachToMarker();
            } else if (SteamVR_Actions._default.Down.GetStateDown(SteamVR_Input_Sources.RightHand)) {
                DetachFromMarker();
            }
        }
    }

    public void SetPositionMarker(GameObject pm) {
        position_marker = pm;
    }

    public void AttachToMarker() {
        gameObject.GetComponent<Collider>().enabled = false;
        gameObject.transform.parent = position_marker.transform;
        gameObject.transform.localPosition = new Vector3(0, y_offset, 0);
    }

    public void DetachFromMarker() {
        gameObject.transform.parent = original_parent;
        gameObject.GetComponent<Collider>().enabled = true;
    }

    public void SetSelect(bool status) {
        isSelected = status;
        print(isSelected);
    }
}
