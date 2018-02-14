/* Zoilo Mercedes
 * Implements hand interaction for grabbing blocks.
 * could add additional func later
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteraction : MonoBehaviour {
	private SteamVR_TrackedObject trackedObj; // keeps track of SteamVR_TrackedObject script attached to controller
	private SteamVR_Controller.Device device;

	public float throwForce = 1.5f;

	// swipe
	public ObjectMenuManager OMM;
	public float swipeSum;
	public float touchLast;
	public float touchCurrent;
	public float distance;
	public bool hasSwipedLeft;
	public bool hasSwipedRight;

	// Use this for initialization
	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		device = SteamVR_Controller.Input((int)trackedObj.index);
		if(device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
			touchLast = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			
		if(device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad)){
			touchCurrent = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x;
			distance = touchCurrent - touchLast;
			touchLast = touchCurrent;
			swipeSum += distance;

			if(!hasSwipedRight){
				if(swipeSum > 0.5f){
					swipeSum = 0;
					Swipe(true);
					hasSwipedRight = true;
					hasSwipedLeft = false;
				}
			}

			if(!hasSwipedLeft){
				if(swipeSum < -0.5f){
					swipeSum = 0;
					Swipe(false);
					hasSwipedLeft = true;
					hasSwipedRight = false;
				}
			}
		}
		if(device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad)){
			swipeSum = 0;
			touchCurrent = 0;
			touchLast = 0;
			hasSwipedLeft = false;
			hasSwipedRight = false;
		}

		if(device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
			SpawnObject();
	}

	void SpawnObject(){
		OMM.SpawnCurrentObject();
	}

	void Swipe(bool direction){
		if(direction){
			OMM.MenuRight();
			Debug.Log("Swiped right!");
		} else {
			OMM.MenuLeft();
			Debug.Log("Swiped left!");
		}
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

	void GrabObject(Collider coll){
		coll.transform.SetParent(gameObject.transform);     // make controller parent
		coll.GetComponent<Rigidbody>().isKinematic = true;  // turn off physics
		device.TriggerHapticPulse(2000);				   // vibrate controller
		Debug.Log("Grabbing object!");
	}

	void ThrowObject(Collider coll){
		coll.transform.SetParent(null);
		Rigidbody rigidBody = coll.GetComponent<Rigidbody>();
		rigidBody.isKinematic = false;
		rigidBody.velocity = device.velocity * throwForce;
		rigidBody.angularVelocity = device.angularVelocity;
		Debug.Log("Released object!");
	}
}