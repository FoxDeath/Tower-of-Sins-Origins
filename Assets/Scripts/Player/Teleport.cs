using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport : MonoBehaviour
{
    [SerializeField] Object sceneObject;

    private Coroutine portalCoroutine;

    private void Update()
    {
        if(PlayerState.GetState() != PlayerState.State.Teleporting && portalCoroutine != null)
        {
            StopCoroutine(portalCoroutine);
        }
    }

    public void Teleporting()
    {
        portalCoroutine = StartCoroutine(TeleportBehaviour());
    }

    private IEnumerator TeleportBehaviour()
    {
        PlayerState.SetState(PlayerState.State.Teleporting);

        PlayerMovement.StopMoving();

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneObject.name);
    }
}
