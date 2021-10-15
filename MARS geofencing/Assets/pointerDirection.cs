using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointerDirection : MonoBehaviour
{
    public GameObject arrow;
    public Material redMaterial;
    public Material greenMaterial;
    public Camera arCamera;
    public GameObject objectPrize;
    private Renderer rend;

    void Awake()
    {
        arrow.SetActive(false);
        rend = arrow.GetComponent<Renderer>();
    }

    void Update()
    {
        if (objectPrize.activeSelf == true) {
            // point to gem if it has appeared
            arrow.SetActive(true);
            transform.LookAt(objectPrize.transform, Vector3.up);
        }
        else {
            arrow.SetActive(false);
        }
        
        if (((objectPrize.transform.position.x - arCamera.transform.position.x) <= 5) && ((objectPrize.transform.position.z - arCamera.transform.position.z) <= 5)) {
            rend.material = greenMaterial;
        } else {
            rend.material = redMaterial;
        }

    }
}