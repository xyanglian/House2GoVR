using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerBehavior : MonoBehaviour
{
    // private GameObject my_camera;
    public GameObject left_controller;
    public GameObject right_controller;
    private GameObject position_marker;
    private GameObject object_marker;
    private GameObject point_marker;
    public GameObject object_menu;
    private RaycastHit hit;
    private GameObject selected;
    private GameObject highlighted_option;
    private GameObject highlighted_button;
    public int numOfPages;
    private int pageNum;
    private int state;
    private LineRenderer line;
    public AudioClip audio_teleport;
    public AudioClip audio_click;
    public AudioClip audio_open;
    private AudioSource audio_source;
    
    // Start is called before the first frame update
    void Start()
    {
        // my_camera = GameObject.Find("Main Camera");
        position_marker = GameObject.Find("PositionMarker");
        object_marker = GameObject.Find("ObjectMarker");
        point_marker = GameObject.Find("PointMarker");
        // object_menu = GameObject.Find("Object Menu");
        Cursor.visible = false;
        state = -1;
        pageNum = 0;
        line = gameObject.AddComponent<LineRenderer>();
        line.sortingLayerName = "OnTop";
        line.sortingOrder = 5;
        line.positionCount = 2;
        line.startWidth = 0.001f;
        line.endWidth = 0.001f;
        line.useWorldSpace = true;
        line.material = new Material(new Material(Shader.Find("Transparent/Diffuse")));
        line.material.color = Color.green;
        audio_source = gameObject.AddComponent<AudioSource>();
    }

    private void highlightOption(GameObject option, bool state) {
        if (option.name != "ColliderCube") {
            print(option.name + " collider problem");
        }
        Color new_color = state ? Color.red : Color.white;
        Transform wireframe = option.transform.parent.Find("wireframe");
        // print(option.name +":"+ option.transform.parent.gameObject.name);
        foreach (Transform child in wireframe) {
            child.gameObject.GetComponent<Renderer>().material.color = new_color;
        }
    }

    private void UpdateMarkers() {
        Vector3 origin = right_controller.transform.position;
        Vector3 direction = right_controller.transform.forward;
        float range = 1000;
        if (Physics.Raycast(origin, direction, out hit, range)){
            // Debug.DrawRay(origin, direction, Color.green, 2, false);
            // Debug.DrawLine(origin, hit.point, Color.green);
            line.SetPosition(0, origin);
            line.SetPosition(1, hit.point);
            Gizmos.color = Color.red;
            // Gizmos.DrawRay(origin, direction);
            if ( hit.point.y <0.01) {
                // ray collide with floor
                state = 0;
                position_marker.SetActive(true);
                position_marker.transform.position = hit.point;
                position_marker.GetComponent<Renderer>().material.color = Color.green;
                point_marker.SetActive(false);
            } else {
                // ray collide with object or wall
                point_marker.transform.position = hit.point;
                if (hit.transform.gameObject.layer == 10) {
                    point_marker.SetActive(true);
                    state = 1;
                    point_marker.GetComponent<Renderer>().material.color = Color.green;
                } else if (hit.transform.gameObject.layer == 11) {
                    state = 2;
                    if (highlighted_option != null) {
                        highlightOption(highlighted_option, false);
                    }
                    highlighted_option = hit.transform.gameObject;
                    highlightOption(highlighted_option, true);
                } else if (hit.transform.gameObject.layer == 12) {
                    state = 3;
                    if (highlighted_button != null) {
                        highlighted_button.GetComponent<Renderer>().material.color = new Color(1f,0,0,0);
                    }
                    highlighted_button = hit.transform.gameObject;
                    highlighted_button.GetComponent<Renderer>().material.color = new Color(1f,0,0,0.2f);
                } else {
                    state = 4;
                    point_marker.GetComponent<Renderer>().material.color = Color.red;
                }
                position_marker.SetActive(false);
            }
        }
    }

    private void Select(GameObject go) {
        if (selected != null) {
            selected.GetComponent<ObjectBehavior>().SetSelect(false);
        }
        selected = go;
        selected.GetComponent<ObjectBehavior>().SetSelect(true);
        object_marker.transform.parent = selected.transform;
        object_marker.GetComponent<Renderer>().material.color = Color.green;
        object_marker.transform.localPosition = new Vector3(0,1,0);
    }

    // Update is called once per frame
    void Update()
    {
        // if (SteamVR_Actions._default.Teleport.GetStateUp(SteamVR_Input_Sources.RightHand)) {
        //     print("hello, finally");
        // }  
        UpdateMarkers();
        if (SteamVR_Actions._default.Teleport.GetStateUp(SteamVR_Input_Sources.RightHand)) {
            switch (state) {
                case 0: // ray collide with floor
                    gameObject.transform.position = position_marker.transform.position;
                    audio_source.clip = audio_teleport;
                    audio_source.Play();
                    break;
                case 1: // ray collide with object
                    Select(hit.transform.gameObject);
                    audio_source.clip = audio_click;
                    audio_source.Play();
                    break;
                case 2: // ray collide with menu option
                    GameObject newObject = GameObject.Instantiate(hit.transform.parent.Find("model").gameObject);
                    newObject.transform.localScale = new Vector3(1,1,1);
                    newObject.AddComponent<ObjectBehavior>();
                    Select(newObject);
                    newObject.GetComponent<ObjectBehavior>().SetPositionMarker(position_marker);
                    newObject.GetComponent<ObjectBehavior>().AttachToMarker();
                    audio_source.clip = audio_click;
                    audio_source.Play();
                    break;
                case 3: // ray collide with menu button
                    GameObject buttonHit = hit.transform.parent.gameObject;
                    object_menu.GetComponent<MenuBehavior>().Apply(buttonHit.name);
                    audio_source.clip = audio_click;
                    audio_source.Play();
                    break;
            }
        }
        if (SteamVR_Actions._default.Teleport.GetStateUp(SteamVR_Input_Sources.LeftHand)) {
            audio_source.clip = audio_open;
            audio_source.Play();
            object_menu.SetActive(!object_menu.activeSelf);
        }
    }
}
