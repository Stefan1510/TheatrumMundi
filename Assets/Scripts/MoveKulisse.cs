using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(distance);
        startDist = transform.position - rayPoint;
        isActive = true;
        _RailSelectionClone.value = this._attachedToRail - 1;
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
        

        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Vector3 rayPoint = ray.GetPoint(distance);
            float xPos = transform.position.x;
            transform.position = rayPoint + startDist;
            transform.position = new Vector3(xPos, transform.position.y, transform.position.z);

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
