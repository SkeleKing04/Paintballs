using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour
{
     [Header("Positioning")]
    //public Transform objectToFollow;
    public float camSpeed, camRotSpeed;
    public Vector2 camPos, sensitivity;
    public Vector3 originOffset;
    public bool useWorldX, useWorldY, useWorldZ;
    private Vector3 rotation;
    private Vector3 velocity;
    public bool doMouseMovement, lookAtTarget;
    public int mainIndex;
    // The first object in this class will be the object followed
    // Any other objects that need to be visible to the camera must be a child of this first object
    [System.Serializable]
    public class objectsToControl
    {
        public Transform objectTransform;
        //Set only one object to be head, first object will be selected
        public bool followCameraRotation, controlXRot, controlYRot, controlZRot;
    }
    public List<objectsToControl> objectToFollow;
    private new Camera camera;
    void Start()
    {
        camera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void setMainIndex(int index)
    {
        mainIndex = index;
        //Debug.Log("Main index set at " + mainIndex.ToString());
    }
    public void increaseMainIndex(int index)
    {
        mainIndex += index;
        if(mainIndex < 0) mainIndex = objectToFollow.Count - 1;
        if(mainIndex >= objectToFollow.Count) mainIndex = 0;
        //Debug.Log("Main index increased by " + index + ". Now set at " + mainIndex.ToString());
    }
    // Update is called once per frame
    void Update()
    {
        if(mainIndex >= 0 && mainIndex < objectToFollow.Count)
        {
            Transform mainTransform = objectToFollow[mainIndex].objectTransform;
            Ray ray = new Ray(mainTransform.position + originOffset.x * mainTransform.right + originOffset.y * mainTransform.up, mainTransform.forward * camPos.x  +  mainTransform.up * camPos.y);
            Vector3 rayPoint = ray.GetPoint(originOffset.z);
            Debug.DrawRay(objectToFollow[mainIndex].objectTransform.position + originOffset, rayPoint, Color.green, 0.1f);
            if(useWorldX) rayPoint.x = objectToFollow[mainIndex].objectTransform.position.x + originOffset.x;
            if(useWorldY) rayPoint.y = objectToFollow[mainIndex].objectTransform.position.y + originOffset.y;
            if(useWorldZ) rayPoint.z = objectToFollow[mainIndex].objectTransform.position.z + originOffset.z;

            transform.position = Vector3.SmoothDamp(transform.position, rayPoint, ref velocity, camSpeed);
            Vector3 lookPos = objectToFollow[mainIndex].objectTransform.position;

            if(!doMouseMovement)
            {
                if(lookAtTarget)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Normalize(objectToFollow[mainIndex].objectTransform.position - transform.position)), Time.deltaTime * camRotSpeed);
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, objectToFollow[mainIndex].objectTransform.rotation, Time.deltaTime * camRotSpeed);
                }
            }
            else if (doMouseMovement)
            {
                float mouseX = Input.GetAxisRaw("Mouse X")* Time.deltaTime * sensitivity.x;
                float mouseY = Input.GetAxisRaw("Mouse Y")* Time.deltaTime * sensitivity.y;

                rotation.y += mouseX;
                rotation.x -= mouseY;
                rotation.x = Mathf.Clamp(rotation.x, -90f, 90f);

                transform.rotation = Quaternion.Euler(rotation.x,rotation.y,0);
                for(int i = 0; i < objectToFollow.Count; i++)
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
        else
        {
            Debug.LogError("The main index does not exist! Is the main index less then the length of objects to follows and a non-negative? Are there targets in objects to follow?");
        }
    }
    
}