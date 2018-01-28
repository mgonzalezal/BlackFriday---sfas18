using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to punch the other players, those are stored in a list when they enter the trigger
/// and we unregister them when they exit, so when we call the function push, we push all of the players in the array.
/// </summary>
public class PushBehaviour : MonoBehaviour
{
  bool is_pushing_;

  public List<GameObject> playersInPushZone;

  // Use this for initialization
  void Start()
  {
    playersInPushZone = new List<GameObject>();
  }

  void Awake()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void Push()
  {
    foreach (GameObject player in playersInPushZone)
    {
      Vector3 pos = (player.transform.position - gameObject.transform.parent.position).normalized;
      player.GetComponent<PlayerBehaviour>().PushPlayer(pos);
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.CompareTag("Player") && other.gameObject != transform.parent.gameObject)
    {
      playersInPushZone.Add(other.gameObject);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject.CompareTag("Player") && other.gameObject != transform.parent.gameObject)
    {
      playersInPushZone.Remove(other.gameObject);
    }
  }
}
