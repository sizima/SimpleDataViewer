using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;


public class AsciiDataToParticle_add : MonoBehaviour {

    // Structure of each data point
    public struct ATOM
    {
        public Vector3 _position;   // coordinate (x, y, z)
        public float _radius;       // radius
        public Color _rgb;          // Color (r, g, b, a)
    }


    // Compute Buffer of data points
    ComputeBuffer _cBuffer_render;

    // Release a Compute Buffer
    void OnDisable()
    {
        // ---------------------------------------- It is required to explicitly release the compute buffer.
        if (_cBuffer_render != null)
            _cBuffer_render.Release();
    }


    // Shader for rendering the data points
    public Shader _shader_particle;

    // material : Assign a shader (_shader_particle)
    Material _mat_particle;

    // texture : Select from Inspector Panel
    public Texture _tex_particle;



    // Read the data and store it in ATOM structure array
    public void ReadAsciiData()
    {
        // Data path
        string _path = Application.dataPath;
        if (Application.platform == RuntimePlatform.OSXPlayer)
            _path += "/../../";
        else if (Application.platform == RuntimePlatform.WindowsEditor)
            _path += "/../";
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
            _path += "/../";
        _path += "chaosB.txt";

        // ---------------------------------------- Count data rows
        int _particleCount = 0;
        string[] _allLines = File.ReadAllLines(@_path);
        for (int _i=0; _i<_allLines.Length; _i++)
            if (_allLines[_i].Length > 0)
                _particleCount++;

        // ---------------------------------------- Prepare ATOM structure array for data points
        ATOM[] _Atoms = new ATOM[_particleCount];

        // ---------------------------------------- store data in ATOM structure array
        int _k = 0;
        for (int _i = 0; _i < _allLines.Length; _i++)
        {
            if (_allLines[_i].Length > 0)
            {
                string[] _data = _allLines[_i].Split();
                _Atoms[_k]._position = new Vector3(float.Parse(_data[0]), float.Parse(_data[1]), float.Parse(_data[2]));
                _Atoms[_k]._radius = float.Parse(_data[3]);
                _Atoms[_k]._rgb = new Color( float.Parse(_data[4]), float.Parse(_data[5]), float.Parse(_data[6]), 1);
                _k++;
            }
        }

        // ---------------------------------------- Initialize the compute buffer and set the ATOM structure
        _cBuffer_render = new ComputeBuffer(_particleCount, Marshal.SizeOf(typeof(ATOM)));
        _cBuffer_render.SetData(_Atoms);
        // ---------------------------------------- Set compute buffer to material
        _mat_particle.SetBuffer("_atoms", _cBuffer_render);     
    }



    void Start()
    {
        _mat_particle = new Material(_shader_particle);
        _mat_particle.SetTexture("_MainTex", _tex_particle);
        ReadAsciiData();
    }


    // OnRenderObject is called after camera has rendered the scene.
    void OnRenderObject()
    {
        // ---------------------------------------- Start rendering the data points
        _mat_particle.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, _cBuffer_render.count);
    }
    
}
