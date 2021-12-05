using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class buildingEnd : MonoBehaviour
{
    [SerializeField] PlayableAsset asset;
    private void OnTriggerEnter2D(Collider2D other) {
                    FindObjectOfType<PlayableDirector>().playableAsset = asset;
            FindObjectOfType<PlayableDirector>().Play();
    }
}
