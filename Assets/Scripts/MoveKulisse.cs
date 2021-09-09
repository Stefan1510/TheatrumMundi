using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveKulisse : MonoBehaviour
{
    [SerializeField] private Dropdown _railSelectionTemplate;
    private Dropdown _RailSelectionClone;
    private bool dragging = false;
    private bool isActive = false;
    private float distance;
    private Vector3 startDist;
    private int _attachedToRail = 1;
    // Start is called before the first frame update

    private void Start()
    {
        //gameObject.SetActive(false);
        _RailSelectionClone = Instantiate(_railSelectionTemplate, GameObject.Find("Canvas").transform);
        _RailSelectionClone.onValueChanged.AddListener((val) => SelectRail(val));   
    }

    public void ToggleSceneryObject()
    {
        if (!gameObject.activeSelf)
        {
            
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    private void SelectRail(int value)
    {
        GameObject schiene = GameObject.Find("Schiene1");
        switch (value)
        {
            case 0:
                
                break;
            case 1:
                schiene = GameObject.Find("Schiene2");
                break;
            case 2:
                schiene = GameObject.Find("Schiene3");
                break;
            case 3:
                schiene = GameObject.Find("Schiene4");
                break;
            case 4:
                schiene = GameObject.Find("Schiene5");
                break;
            case 5:
                schiene = GameObject.Find("Schiene6");
                break;
            case 6:
                schiene = GameObject.Find("Nagelbrett1");
                break;
            case 7:
                schiene = GameObject.Find("Nagelbrett2");
                break;
            case 8:
                gameObject.SetActive(false);
                isActive = false;
                break;
        }
        //Debug.Log("hallo " + gameObject.name + " - " + value.ToString());
        gameObject.transform.position = new Vector3(schiene.transform.position.x + 0.06f, schiene.transform.position.y + gameObject.transform.localPosition.y, gameObject.transform.position.z);
        gameObject.transform.SetParent(schiene.transform);
        Update();
        
    }

    void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);
    }

    void OnMouseDown()
    {
        
        _RailSelectionClone.gameObject.transform.SetParent(null);
        _RailSelectionClone.gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        startDist = transform.position - rayPoint;
        isActive = true;
        //_RailSelectionClone.value = this._attachedToRail - 1;
        switch (gameObject.transform.parent.name)
        {
            case "Schiene1":
                _attachedToRail = 1;
                _RailSelectionClone.value = 0;
                break;
            case "Schiene2":
                _attachedToRail = 2;
                _RailSelectionClone.value = 1;
                break;
            case "Schiene3":
                _attachedToRail = 3;
                _RailSelectionClone.value = 2;
                break;
            case "Schiene4":
                _attachedToRail = 4;
                _RailSelectionClone.value = 3;
                break;
            case "Schiene5":
                _attachedToRail = 5;
                _RailSelectionClone.value = 4;
                break;
            case "Schiene6":
                _attachedToRail = 6;
                _RailSelectionClone.value = 5;
                break;
            case "Nagelbrett1":
                _attachedToRail = 7;
                _RailSelectionClone.value = 6;
                break;
            case "Nagelbrett2":
                _attachedToRail = 8;
                _RailSelectionClone.value = 7;
                break;
        }
    }

    private void OnMouseUp()
    {
        dragging = false;
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire2"))
        {
            isActive = false;
        }

        _RailSelectionClone.gameObject.SetActive(isActive);
        if (isActive)
        {
            this._attachedToRail = _RailSelectionClone.value + 1;
            //Debug.Log(this._attachedToRail);
        }

        /// Voodoo to realize if mouse is over a UIPanel
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        GameObject uiPanel = GameObject.FindGameObjectWithTag("UIPanel");
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == uiPanel)
            {
                //Debug.Log(result.gameObject);
                dragging = false;
                isActive = false;
            }
        }
        /// End of Voodoo 
        /// if Mouse is over a UIPanel no Movement of SceneryObjects is possible

        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            float xPos = transform.position.x;
            transform.position = rayPoint + startDist;
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
            if (transform.position.y < -1.0f)
            {
                transform.position = new Vector3(xPos, -0.99f, transform.position.z);
            }

                //RaycastHit hit;
                //if (Physics.Raycast(ray, out hit))
                //{
                //    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);

                //    if (hit.collider.gameObject == gameObject)
                //    {
                //        Debug.Log(hit.collider.gameObject);
                //        float zPos = transform.position.z;
                //        transform.position = ray.origin + (ray.direction * Mathf.Abs(ray.origin.z));
                //        transform.position = new Vector3(transform.position.x, transform.position.y, zPos);
                //    }
                //}
            }
        }
}
