using UnityEngine;
using UnityEngine.EventSystems;


public class CameraController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject _cameraObj;
    public Camera _camera;

    public GameObject _cam_horizontalRot;
    public GameObject _cam_verticalRot;
    public float _rotSpeed = 0.25f;
    public float _moveSpeed = 0.001f;
    public float _dollySpeed = 0.001f;
    public float _dollySpeed2 = 0.3f;
    private Vector3 oldPos;

    public Vector3 _bestPos = new Vector3(0, 0, 0);
    public float _bestDis = -400;

    public void SetBestPos()
    {
        _cam_horizontalRot.transform.position = _bestPos;
        _leapTarget_dolly = new Vector3(0f, 0f, _bestDis);
        _cameraObj.transform.localPosition = new Vector3(0f, 0f, _bestDis);
    }


    void mouseDragEvent(Vector3 mousePos)
    {
        Vector3 _diff = mousePos - oldPos;

        if (_diff.magnitude < Vector3.kEpsilon)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            cameraRotate(_diff);
        }
        else if (Input.GetMouseButton(2))
        {
            cameraTranslate(_diff);
        }
        else if (Input.GetMouseButton(1))
        {

        }
        oldPos = mousePos;
        return;
    }


    public Vector3 _lerpTarget_rotate;
    public void cameraRotate(Vector3 _d)
    {
        _lerpTarget_rotate += _d;
    }
    void Rotate2target()
    {
        _lerpTarget_rotate = Vector3.Lerp(_lerpTarget_rotate, new Vector3(0, 0, 0), Time.deltaTime * 10f);

        _cam_verticalRot.transform.Rotate(-_lerpTarget_rotate.y * _rotSpeed, 0, 0);
        _cam_horizontalRot.transform.Rotate(0, _lerpTarget_rotate.x * _rotSpeed, 0);
    }

    
    public void cameraTranslate(Vector3 _d)
    {
        float _z = _cameraObj.transform.localPosition.z;
        Vector2 _ryrz;
        if (_cam_verticalRot.transform.localEulerAngles.y == 0)
            _ryrz = new Vector2(Mathf.Cos(_cam_verticalRot.transform.localEulerAngles.x * Mathf.PI / 180.0f),
                                Mathf.Sin((_cam_verticalRot.transform.localEulerAngles.x) * Mathf.PI / 180.0f));
        else
            _ryrz = new Vector2(-Mathf.Cos(_cam_verticalRot.transform.localEulerAngles.x * Mathf.PI / 180.0f),
                                Mathf.Sin((_cam_verticalRot.transform.localEulerAngles.x) * Mathf.PI / 180.0f));

        _cam_horizontalRot.transform.Translate(_d.x * _moveSpeed * _z, _d.y * _moveSpeed * _ryrz.x * _z, _d.y * _moveSpeed * _ryrz.y * _z);
    }


    public void cameraDolly(Vector3 _d)
    {
            float _z = _cameraObj.transform.localPosition.z;
            Vector3 _camPos = new Vector3(0f, 0f, _dollySpeed * _z * _d.x);

            _leapTarget_dolly -= _camPos;
            if (_z > -0.5f)
            {
                _leapTarget_dolly = new Vector3(0f, 0f, -1);
            }
            else if(_z < -400)
        {
            _leapTarget_dolly = new Vector3(0f, 0f, -390);
        }
    }

    public void cameraDolly2(float _d)
    {
        float _z = _cameraObj.transform.localPosition.z;
        Vector3 _camPos = new Vector3(0f, 0f, _dollySpeed2 * _z * _d);
        _leapTarget_dolly -= _camPos;
        if (_z > -0.5f)
        {
            _leapTarget_dolly = new Vector3(0f, 0f, -1);
        }
        else if (_z < -400)
        {
            _leapTarget_dolly = new Vector3(0f, 0f, -390);
        }
    }

    Vector3 _leapTarget_dolly;

    void Dolly2target()
    {
        float _z = _cameraObj.transform.localPosition.z;
        if (_z > -0.5f)
        {
            _leapTarget_dolly = new Vector3(0f, 0f, -1);
        }
        else if (_z < -400)
        {
            _leapTarget_dolly = new Vector3(0f, 0f, -390);
        }
        _cameraObj.transform.localPosition = Vector3.Lerp(_cameraObj.transform.localPosition, _leapTarget_dolly, Time.deltaTime * 10.0f);
    }

    

    public void OnBeginDrag(PointerEventData ped)
    {

    }

    
    public void OnDrag(PointerEventData eventData)
    {

    }


    public void OnEndDrag(PointerEventData ped)
    {
        
    }


    void Awake()
    {
        _leapTarget_dolly = new Vector3(0f, 0f, _bestDis);
        _lerpTarget_rotate = new Vector3(0f, 0f, 0f);
        _camera.fov = 40;
    }


    // Update is called once per frame
    void Update()
    {
        float _wheelDelta = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) )
        {
            this.oldPos = Input.mousePosition;
        }

        mouseDragEvent(Input.mousePosition);
        cameraDolly2(_wheelDelta);

        if (Input.GetKey(KeyCode.F))
        {
            SetBestPos();
        }

        Dolly2target();
        Rotate2target();
    }
}