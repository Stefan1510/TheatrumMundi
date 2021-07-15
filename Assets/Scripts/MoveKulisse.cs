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
        _RailSelectionClone = Instantiate(_railSelectionTemplate, GameObject.Find("Canvas").transform);
        _RailSelectionClone.onValueChanged.AddListener((val) => SelectRail(val));
    }

    public void toggleSceneryObject()
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
                schiene = GameObject.Find("Schiene3");
                break;
            case 2:
                ///ToDO
                ///
                break;
        }
        Debug.Log("hallo " + gameObject.name + " - " + value.ToString());
        gameObject.transform.SetParent(schiene.transform);
        gameObject.transform.localPosition = new Vector3(schiene.transform.localPosition.z + 0.06f, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
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
            float zPos = transform.position.z;
            transform.position = rayPoint + startDist;
            transform.position = new Vector3(transform.position.x, transform.position.y, zPos);

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
