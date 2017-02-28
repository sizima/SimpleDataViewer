using UnityEngine;
using System.IO;


public class CubemapScreenshot : MonoBehaviour {
	public int _reso = 1024; 
    public string _baseFileName = "cubemap";

    string _filePath;

    string _path_F, _path_L, _path_R, _path_B, _path_BO, _path_T;

    void CaptureToPNG() {
		RenderTexture _rt = RenderTexture.GetTemporary(_reso, _reso, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, 1);
        this.GetComponent<Camera>().targetTexture = _rt;

		Texture2D _screenShot = new Texture2D(_reso, _reso, TextureFormat.RGB24, false);
        this.GetComponent<Camera>().Render();

        RenderTexture.active = _rt;

		_screenShot.ReadPixels(new Rect(0, 0, _reso, _reso), 0, 0, false);
		_screenShot.Apply();

		byte[] _pngData = _screenShot.EncodeToPNG ();
        this.GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
		Destroy(_rt);
		File.WriteAllBytes (_filePath, _pngData);
	}


    void CreateDirectory()
    {
        string _path = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
            _path += "/../../";
        else if (Application.platform == RuntimePlatform.WindowsEditor)
            _path += "/../";
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
            _path += "/../";

        if (!File.Exists(_path + "cubemap"))
            Directory.CreateDirectory(_path + "cubemap");

        _path_F = _path + "cubemap/f";
        _path_L = _path + "cubemap/l";
        _path_R = _path + "cubemap/r";
        _path_B = _path + "cubemap/b";
        _path_BO = _path + "cubemap/bo";
        _path_T = _path + "cubemap/t";

        if (!File.Exists(_path_F))
            Directory.CreateDirectory(_path_F);
        if (!File.Exists(_path_L))
            Directory.CreateDirectory(_path_L);
        if (!File.Exists(_path_R))
            Directory.CreateDirectory(_path_R);
        if (!File.Exists(_path_B))
            Directory.CreateDirectory(_path_B);
        if (!File.Exists(_path_BO))
            Directory.CreateDirectory(_path_BO);
        if (!File.Exists(_path_T))
            Directory.CreateDirectory(_path_T);

        _path_F += ("/f_" + _baseFileName + "_");
        _path_L += ("/l_" + _baseFileName + "_");
        _path_R += ("/r_" + _baseFileName + "_");
        _path_B += ("/b_" + _baseFileName + "_");
        _path_BO += ("/bo_" + _baseFileName + "_");
        _path_T += ("/t_" + _baseFileName + "_");
    }

    void GetFileName(string _f)
    {
        int _n = 0;
        while (File.Exists(_f + _n.ToString("00000") + ".png"))
            _n++;

        _filePath = _f + _n.ToString("00000") + ".png";
        Debug.Log("_filePath = " + _filePath);
    }
    

    public void ExportCubemap()
    {
        float _lastFoV = this.GetComponent<Camera>().fieldOfView;
        this.GetComponent<Camera>().fieldOfView = 90;
        CreateDirectory();
        Quaternion _lastRotation = this.transform.rotation;
        GetFileName(_path_F);
        CaptureToPNG();
        this.transform.RotateAround(this.transform.up, (90 * Mathf.Deg2Rad));
        GetFileName(_path_R);
        CaptureToPNG();
        this.transform.rotation = _lastRotation;
        this.transform.RotateAround(this.transform.up, (180 * Mathf.Deg2Rad));
        GetFileName(_path_B);
        CaptureToPNG();
        this.transform.rotation = _lastRotation;
        this.transform.RotateAround(this.transform.up, (-90 * Mathf.Deg2Rad));
        GetFileName(_path_L);
        CaptureToPNG();
        this.transform.rotation = _lastRotation;

        this.transform.RotateAround(this.transform.right, (90 * Mathf.Deg2Rad));
        GetFileName(_path_BO);
        CaptureToPNG();
        this.transform.rotation = _lastRotation;
        this.transform.RotateAround(this.transform.right, (-90 * Mathf.Deg2Rad));
        GetFileName(_path_T);
        CaptureToPNG();
        this.transform.rotation = _lastRotation;
        this.GetComponent<Camera>().fieldOfView = _lastFoV;
    }

    
    void Update()
    {
		if (Input.GetKeyDown (KeyCode.Return))
            ExportCubemap();
	}
}



