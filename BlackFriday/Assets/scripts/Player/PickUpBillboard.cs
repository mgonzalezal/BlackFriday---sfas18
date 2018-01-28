using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Orientates the billboard to pick up stuff of the player to the camera assigned.
/// </summary>
public class PickUpBillboard : MonoBehaviour
{

  public Camera cameraUsing;

  // Use this for initialization
  void Start()
  {

  }

  void Awake()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (cameraUsing)
    {
      transform.LookAt(cameraUsing.transform);
    }
    else
    {
      transform.LookAt(Camera.main.transform);
    }
  }
}
