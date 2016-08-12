using UnityEngine;
using System.Collections;

public class AppMono : MonoBehaviour {
    // Use this for initialization
    void Start () {
        GameController.Instance.Initialize();
        UIController.Instance.Initialize();
    }
	
	// Update is called once per frame
	void Update () {
        GameController.Instance.Update();
        UIController.Instance.Update();
	}
}