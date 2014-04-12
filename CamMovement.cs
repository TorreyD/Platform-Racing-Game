using UnityEngine;
using System.Collections;

public class CamMovement : MonoBehaviour 
{
	public static GameObject _target;

	private float _xOffset;
	private float _initY;

	private Transform _myTransform;

	// Use this for initialization
	void Start () 
	{
		_myTransform = transform;
		_initY =  _myTransform.position.y;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (_target != null)
		{
			_myTransform.position = new Vector3 (_target.transform.position.x, _initY, -10);
		}
	}
}
