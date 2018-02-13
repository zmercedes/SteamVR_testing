/* Zoilo Mercedes
 * SteamVR input handling
 * can also use SteamVR_Controller.ButtonMask.TouchPad to get input from touchpad
 *
 *
 *
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandControllerInput : MonoBehaviour {
	public SteamVR_TrackedObject trackedObj; // keeps track of SteamVR_TrackedObject script attached to controller
	public SteamVR_Controller.Device device; // gets the device that will bind to this object

	// Teleporter
	private LineRenderer laser;
	public GameObject teleportAimerObject;
	public Vector3 teleportLocation;
	public GameObject player;
	public LayerMask laserMask;
	public float yNudgeAmt = 1f; // teleportaimerobject height

	// set mode of transport
	public bool teleport = false;
	public bool dash  = true;
	public bool walk = false;

	// dash
	public float dashSpeed = 20f;
	private bool isDashing;
	private float journeyLength;
	private float startTime;
	private float lerpTime;
	private Vector3 dashStartPosition;

	// walk
	public Transform playerCam;
	public float walkSpeed = 4f;
	private Vector3 movementDirection;

	void Start () {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		laser = GetComponentInChildren<LineRenderer>();
	}
	
	void Update () {
		device = SteamVR_Controller.Input((int)trackedObj.index); // pairs device to trackedobject in scene

		if(isDashing){
			float distCovered = (Time.time - startTime) * dashSpeed;
        	float fracJourney = distCovered / journeyLength;
			player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, fracJourney);
			//Debug.Log(lerpTime);
			if(player.transform.position == teleportLocation)
				isDashing = false;
			
		} else {
			if(device.GetPress(SteamVR_Controller.ButtonMask.Trigger)){ // activates on trigger held down
				if(teleport || dash){ // sets move location for dash/teleport
					laser.gameObject.SetActive(true);
					teleportAimerObject.SetActive(true);

					// setting up teleportation movement mechanic
					laser.SetPosition(0, gameObject.transform.position);
					RaycastHit hit;
					if(Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask)){ // teleports to point
						teleportLocation = hit.point;
						laser.SetPosition(1,teleportLocation);
						// aimer position
						teleportAimerObject.transform.position = new Vector3(teleportLocation.x,teleportLocation.y + yNudgeAmt, teleportLocation.z);
					} else { // teleports to ground below point
						teleportLocation = transform.position + transform.forward * 15;
						RaycastHit groundRay;
						if(Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17,laserMask)){
							teleportLocation = new Vector3(transform.forward.x *15 + transform.position.x,groundRay.point.y,transform.forward.z *15 + transform.position.z);
						}
						laser.SetPosition(1, transform.forward * 15 + transform.position);

						teleportAimerObject.transform.position = teleportLocation + new Vector3(0, yNudgeAmt, 0);
					}
				} else { // walking movement
					movementDirection = playerCam.transform.forward;
					movementDirection = new Vector3(movementDirection.x,0,movementDirection.z);
					movementDirection *= walkSpeed * Time.deltaTime;
					player.transform.position += movementDirection;
				}
			}
			if(device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)){  // activates on trigger release
				laser.gameObject.SetActive(false);
				teleportAimerObject.SetActive(false);
				if(dash){
					dashStartPosition = player.transform.position;
					journeyLength = Vector3.Distance(dashStartPosition, teleportLocation);
					isDashing = true;
					startTime = Time.time;
				} else if(teleport)
					player.transform.position = teleportLocation;
			}
		}
	}
}