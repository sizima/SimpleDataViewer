using UnityEngine;


public class Grid : MonoBehaviour {
	
	public Material _mat;
	public float _gap = 1.0f;
	public int _area = 4;


	private Vector3[] _vertices;
	
	public void Start () 
	{
		SetVertices();
	}
    

	void OnRenderObject()
	{
        //if (Camera.current.name == "MainCamera")
        if (Camera.current.name == this.name)
            DrawLine();
	}


	private void SetVertices()
	{
		int _vNum = (_area * 2 + 1) * 4;
		_vertices = new Vector3[_vNum];
		int _k = 0;
		for (int _i=0; _i<_vNum/4; _i++) {
			_vertices[_k] 	= new Vector3((-_gap*_area+_gap*_i), 0, (-_gap*_area));
			_vertices[_k+1]	= new Vector3((-_gap*_area+_gap*_i), 0, (_gap*_area));
			_k += 2;
		}
		for (int _i=0; _i<_vNum/4; _i++) {
			_vertices[_k] 	= new Vector3((-_gap*_area), 0, (-_gap*_area+_gap*_i));
			_vertices[_k+1]	= new Vector3((_gap*_area), 0, (-_gap*_area+_gap*_i));
			_k += 2;
		}
	}

	private void DrawLine() 
	{
        Vector3 _vPos = Vector3.zero;
        Quaternion _qRot = Quaternion.identity;
        Matrix4x4 _matrix = Matrix4x4.TRS(_vPos, _qRot, Vector3.one);



        _mat.SetPass(0);
		
		GL.PushMatrix();
		GL.MultMatrix(_matrix);
		GL.Begin(GL.LINES);

		int _k = 0;
		for (int i=0; i < _vertices.Length / 2; i++) 
		{
			GL.Vertex(_vertices[_k]);
			GL.Vertex(_vertices[_k + 1]);
			_k+=2;
		}
		GL.End();
		GL.PopMatrix();
	}
}