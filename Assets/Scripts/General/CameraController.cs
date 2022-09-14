using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Positioning")]
    //public Transform objectToFollow;
    public float camSpeed, camRotSpeed, camDistance;
    public Vector2 camPos, sensitivity;
    public Vector3 originOffset;
    private Vector3 rotation, velocity;
    public bool doMouseMovement, lookAtTarget;
    // The first object in this class will be the object followed
    // Any other objects that need to be visible to the camera must be a child of this first object
    [System.Serializable]
    public class objectsToControl
    {
        public Transform objectTransform;
        //Set only one object to be head, first object will be selected
        public bool followCameraRotation, controlXRot, controlYRot, controlZRot;
    }
    public objectsToControl[] objectToFollow;
    private int mainIndex;
    // Start is called before the first frame update
    void Start()
    {
        setMainIndex(0);
    }
    public void setMainIndex(int index)
    {
        if(index >= 0 && index < objectToFollow.Length)
        {
            mainIndex = index;
        }
        else
        {
            Debug.LogError("Given index (" + index + ") is greater then array bounds.");
        }
    }
    public void increaseMainIndex(int amount)
    {
        if(mainIndex + amount >= objectToFollow.Length)
        {
            mainIndex = mainIndex - objectToFollow.Length + amount;
        }
        else
        {
            mainIndex += amount;
        }
    }
    public void decreaseMainIndex(int amount)
    {
        if(mainIndex - amount < 0)
        {
            mainIndex = mainIndex + objectToFollow.Length - amount;
        }
        else
        {
            mainIndex -= amount;
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Position of the camera
        Transform mainTransform = objectToFollow[mainIndex].objectTransform;
        Vector3 startPos = mainTransform.position + originOffset.x * mainTransform.right + originOffset.y * mainTransform.up;
        Gizmos.DrawSphere(startPos, 1);
        Vector3 camDir = mainTransform.forward * camPos.x + mainTransform.up * camPos.y;
        Ray ray = new Ray(startPos, camDir);
        Debug.DrawRay(startPos, camDir, Color.green, 0.1f);
        Gizmos.DrawSphere(ray.GetPoint(originOffset.z),1);
        transform.position = Vector3.SmoothDamp(transform.position, ray.GetPoint(originOffset.z), ref velocity, camSpeed);
        Debug.DrawRay(transform.position, transform.forward*Mathf.Infinity,Color.magenta,0.01f);

        //Rotation of the camera
        float mouseX = Input.GetAxisRaw("Mouse X")* Time.deltaTime * sensitivity.x;
        float mouseY = Input.GetAxisRaw("Mouse Y")* Time.deltaTime * sensitivity.y;
        rotation.y += mouseX;
        rotation.x -= mouseY;
        rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

        Transform lookPos = objectToFollow[mainIndex].objectTransform;
        if(lookAtTarget) transform.LookAt(objectToFollow[mainIndex].objectTransform, objectToFollow[mainIndex].objectTransform.up);
        else transform.rotation = Quaternion.Euler(rotation.x,rotation.y,0);  

        for(int i = 0; i < objectToFollow.Length; i++)
        {
            if(objectToFollow[i].followCameraRotation)
            {
                Vector3 tempRotation = new Vector3(0,0,0);
                if(objectToFollow[i].objectTransform.parent != null)
                {
                    tempRotation = objectToFollow[i].objectTransform.transform.eulerAngles;
                }
                if(objectToFollow[i].controlXRot)
                {
                    tempRotation = new Vector3(rotation.x, tempRotation.y, tempRotation.z);
                }
                if(objectToFollow[i].controlYRot)
                {
                    tempRotation = new Vector3(tempRotation.x, rotation.y, tempRotation.z);
                }
                if(objectToFollow[i].controlZRot)
                {
                    tempRotation = new Vector3(tempRotation.x, tempRotation.y, rotation.z);
                }                
                objectToFollow[i].objectTransform.eulerAngles = tempRotation;
            }
        }
    }
}
