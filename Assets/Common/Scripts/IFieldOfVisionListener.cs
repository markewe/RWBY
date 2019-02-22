using UnityEngine;

public interface IFieldOfVisionListener {
	void OnFieldOfVisionEnter(GameObject gameObject);
	void OnFieldOfVisionExit(GameObject gameObject);
}
