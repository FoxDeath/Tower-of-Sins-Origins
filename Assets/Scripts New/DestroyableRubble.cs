using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class DestroyableRubble : MonoBehaviour
{
    [SerializeField] PlayableAsset asset;
    private void OnDestroy() {
        FindObjectOfType<PlayableDirector>().playableAsset = asset;
        FindObjectOfType<PlayableDirector>().Play();
    }
}
