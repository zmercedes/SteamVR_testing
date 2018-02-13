using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
	private SteamVR_TrackedObject trackedObj; // keeps track of SteamVR_TrackedObject script attached to controller
	private SteamVR_Controller.Device device;

	public float throwForce = 1.5f;

	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		device = SteamVR_Controller.Input((int)trackedObj.index);
	}

	void OnTriggerStay(Collider col){
		if(col.gameObject.CompareTag("Throwable")){
			if(device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)){ // throws nullreference exception at runtime
				ThrowObject(col);
			} else if(device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)){
				GrabObject(col);
			}
		}
	}

	void GrabObject(Collider col){
		col.transform.SetParent(gameObject.transform);     // make controller parent
		col.GetComponent<Rigidbody>().isKinematic = true;  // turn off physics
		device.TriggerHapticPulse(2000);				   // vibrate controller
		Debug.Log("Grabbing object!");
	}

	void ThrowObject(Collider col){
		col.transform.SetParent(null);
		Rigidbody rigidBody = col.GetComponent<Rigidbody>();
		rigidBody.isKinematic = false;
		rigidBody.velocity = device.velocity * throwForce;
		rigidBody.angularVelocity = device.angularVelocity;
		Debug.Log("Released Object!");
	}
}
