using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;

public class DragMouseOrbit : MonoBehaviour
{
    public Transform target;
    public Transform newTarget;
    [SerializeField]
    float camSpeed = 5f;
    float YnewLimit;
    [HideInInspector]
    public Quaternion lastRotation;
    float t;
    public float distance = 2.0f;
    public float xSpeed = 20.0f;
    public float ySpeed = 20.0f;
    public float yMinLimit = -90f;
    public float yMaxLimit = 90f;
    public float xMinLimit = -60f;
    public float xMaxLimit = 120f;
    public float distanceMin = 10f;
    public float distanceMax = 10f;
    bool cameFromIndiv = false;
    public float smoothTime = 2f;
    public float rotationYAxis = 0.0f;
    public float rotationXAxis = 0.0f;
    float velocityX = 0.0f;
    float velocityY = 0.0f;
    public float scrollSensitivity = 40;
    float timeSinceLastClick = 0;
    public float timeToEnableTuto = 20;
    bool tutoPassed = false;
    [HideInInspector]
    public float deptoX;
    [HideInInspector]
    public float deptoY;
    Vector3 distances;
    [HideInInspector]
    public bool canView = true;
    [HideInInspector]
    public bool canWork = true;
    Vector3 lastPos;
    bool changingDistance = false;

    Vector3 velocity = Vector3.zero;
    [HideInInspector]
    public bool canInteract = true;
    DepthOfField depthOfField;
    public GameObject[] tutoDeactivate;
    void Start()
    {
        /*Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;*/
        GameObject.Find("Manager").GetComponent<PostProcessVolume>().profile.TryGetSettings(out depthOfField);
        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().freezeRotation = true;
        }
    }
    void LateUpdate()
    {
        if(!canInteract)
        {
            t+= Time.deltaTime*camSpeed*6;
            target.position = Vector3.Lerp(target.position,newTarget.position,t);
            if(Vector3.Distance(target.position,newTarget.position) < .01f)
            {
                t= 0;
                target.position = newTarget.position;
                distanceMax = distances.x;
                distanceMin = distances.y;
                distance = distances.z;
                yMinLimit = YnewLimit;
                canInteract = true;
            }
        }
        if(changingDistance)
        {
            //Debug.Log("Max: " + distanceMax+" " + distances.x + "Min: " +distanceMin + " " + distances.y + "Dist: " + distance+ " " + distances.z);
            //Debug.Log("ChangingMax");
            //MaxChanging
            distanceMax = Mathf.Lerp(distanceMax,distances.x,t);
            //MinChanging
            distanceMin = Mathf.Lerp(distanceMin,distances.y,t);
            //DistChanging
            distance = Mathf.Lerp(distance,distances.z,t);
            yMinLimit = Mathf.Lerp(yMinLimit,YnewLimit,t);
            if(distanceMax == distances.x && distanceMin == distances.y && distance == distances.z)
            {
                changingDistance = false;
            }
        }
        if(Botonera.deptoViewEnabled && !Botonera.deptoIndividualViewEnabled && canInteract)
        {
            if(GameObject.Find("Core"+Botonera.currentPiso.ToString()) != null)
            {
                if(!changingDistance && distanceMax != 86)
                {
                    ChangeDistance(86,72,76,0);
                }
                depthOfField.focusDistance.Override(50);
                depthOfField.aperture.Override(0.50f);
                if(Vector3.Distance(target.position,GameObject.Find("Core"+Botonera.currentPiso.ToString()).transform.position) > 1)
                {
                    SetNewTarget(GameObject.Find("Core"+Botonera.currentPiso.ToString()).transform);
                }
            }
        }
        else if(!Botonera.deptoViewEnabled && !Botonera.deptoIndividualViewEnabled && canInteract)
        {
            if(!changingDistance && distanceMax != 72)
            {
                ChangeDistance(72,40,55,10);
            }
            depthOfField.aperture.Override(0.25f);
            depthOfField.focusDistance.Override(65);
            if(Vector3.Distance(target.position,GameObject.Find("LookAtCamera").transform.position) > 1)
            {
                SetNewTarget(GameObject.Find("LookAtCamera").transform);
            }
        }
        if (!WebView.showing)
        {
            if (Input.GetMouseButton(0) && canView && canWork && canInteract)
            {
                if(!tutoPassed && !Botonera.deptoViewEnabled)
                {
                    timeSinceLastClick = 0;
                    tutoPassed = true;
                    foreach(GameObject tuto in tutoDeactivate)
                    {
                        tuto.GetComponent<Animator>().Play("OpacityOff");
                    }
                }
                velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
                velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
            }
            else
            {
                timeSinceLastClick += Time.deltaTime;
            }
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;
            rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
            rotationYAxis = ClampAngle(rotationYAxis, xMinLimit, xMaxLimit);
            //Quaternion fromRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, distanceMin, distanceMax);
            int layerMask = 1 << 3;
            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit,layerMask))
            {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            if(canView)
            {
                if(cameFromIndiv)
                {
                        rotationXAxis = Mathf.Lerp(rotationXAxis,deptoY,t);
                        rotationYAxis = Mathf.Lerp(rotationYAxis,deptoX,t);
                        Debug.Log("A");
                        if(Mathf.Abs(rotationXAxis-deptoY) < 1 && Mathf.Abs(rotationYAxis-deptoX) < .1f)
                        {
                            cameFromIndiv = false;
                        }
                        //transform.rotation = lastRotation;
                }
            }
            else
            {
                if(canInteract)
                {
                    cameFromIndiv = true;
                    //transform.rotation = Quaternion.Euler(90,0,0);
                }
                rotationXAxis = Mathf.Lerp(rotationXAxis,90,t);
                rotationYAxis = Mathf.Lerp(rotationYAxis,0,t);                   
            }
            transform.rotation = rotation;
            transform.position = position;
            velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
            velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
            if(timeSinceLastClick >= timeToEnableTuto && !Botonera.deptoViewEnabled)
            {
                tutoPassed = false;
                foreach(GameObject tuto in tutoDeactivate)
                {
                    tuto.SetActive(true);
                    tuto.GetComponent<Animator>().Play("OpacityOn");
                }
            }
        }
    }
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void canWorkSet(bool set)
    {
        canWork = set;
    }

    public void SetNewTarget(Transform newTgt)
    {
        newTarget = newTgt;
        canInteract = false;
        lastPos = target.position;
        /*while(Vector3.Distance(newTarget.position, target.position) > 1)
        {
            Debug.Log("Moving");
        }*/
    }

    public void ChangeDistance(float distMax,float distMin,float newDistance,float yLimit)
    {
        distances = new Vector3(distMax,distMin,newDistance);
        YnewLimit = yLimit;
        changingDistance = true;
    }
}