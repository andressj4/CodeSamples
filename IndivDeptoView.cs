using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class IndivDeptoView : MonoBehaviour
{
    public GameObject backButtonDepto;
    Button backButtonDptobtn;
    public GameObject backButtonGral;
    Button backButtonGralbtn;
    DragMouseOrbit _drag;
    GameObject SliderPiso;
    public static int nroDeptoS;
    Transform lastTarget;
    DepthOfField depthOfField;
    bool view;

    void Start()
    {
        backButtonDptobtn = backButtonDepto.GetComponent<Button>();
        backButtonGralbtn = backButtonGral.GetComponent<Button>();
        SliderPiso = GameObject.Find("SliderPisos");
        _drag = Camera.main.GetComponent<DragMouseOrbit>();
        GetComponent<PostProcessVolume>().profile.TryGetSettings(out depthOfField);
    }
    void LateUpdate()
    {
        view = Botonera.deptoIndividualViewEnabled;
        if(!Botonera.deptoIndividualViewEnabled)
        {
            depthOfField.enabled.Override(true);
        }
        else
        {
            depthOfField.enabled.Override(false);
        }
        #if !UNITY_EDITOR && UNITY_WEBGL
        if(WebView.showing || !_drag.canInteract)
        {
            backButtonDptobtn.interactable = false;
            backButtonGralbtn.interactable = false;
        }
        else
        {
            backButtonDptobtn.interactable = true;
            backButtonGralbtn.interactable = true;
        }
        #endif
    }

    public void EnterDepto(int nroDepto)
    {
        nroDeptoS = nroDepto;
        lastTarget = _drag.newTarget;
        GameObject.Find(nroDepto.ToString()).GetComponent<Toggler>().ToggleObject(true);
        GameObject.Find(nroDepto.ToString()).GetComponent<Toggler>().ToggleSpecific(false,1);
        _drag.deptoY = _drag.rotationXAxis;
        _drag.deptoX = _drag.rotationYAxis;
        _drag.lastRotation = _drag.transform.rotation;
        _drag.SetNewTarget(GameObject.Find(nroDepto.ToString()).transform.GetChild(2));
        _drag.canView = false;
        float dist = GameObject.Find(nroDepto.ToString()).GetComponent<NroDepto>()._distance;
        _drag.ChangeDistance(dist,dist,dist,90);
        SliderPiso.SetActive(false);
        backButtonDepto.SetActive(true);
        backButtonGral.SetActive(false);
    }
    public void Salirdepto()
    {
        Botonera.deptoIndividualViewEnabled = false;
        if(GameObject.Find(nroDeptoS.ToString()) != null)
        {
            GameObject.Find(nroDeptoS.ToString()).GetComponent<Toggler>().ToggleObject(false);
            GameObject.Find(nroDeptoS.ToString()).GetComponent<Toggler>().ToggleSpecific(true,1);
        }
        SliderPiso.SetActive(true);
        InteractDepto.canWork = true;
        _drag.canView = true;
        _drag.SetNewTarget(lastTarget);
        _drag.xSpeed = 0.1f;
        _drag.ChangeDistance(72,40,55,10);
        backButtonDepto.SetActive(false);
        backButtonGral.SetActive(true);
    }
}
