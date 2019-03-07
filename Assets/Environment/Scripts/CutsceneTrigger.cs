using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour, IInteractionListener, ICutscene {

	#region IInteractionListener

	public void OnInteract(){
		PreSetup();
	}

	#endregion

	#region ICutscene

	public void PreSetup(){
		// blackout camera

		// setup all models
		PlayerManager.instance.player.transform.position = new Vector3(0,-10, 0);

		Play();
	}

	public void Play(){
		var csTimeline = GetComponent<PlayableDirector>(); //Instantiate(GameObject.Find("Special Attack Timeline"), transform.position, transform.rotation) as PlayableDirector;
		csTimeline.Play();
		csTimeline.stopped += OnPlayableDirectorStopped;
	}

	void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
		PostSetup();
    }


	public void PostSetup(){
		PlayerManager.instance.player.transform.position = new Vector3(0,0,0);
	}

	#endregion
}
