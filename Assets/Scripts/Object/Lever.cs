using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Lever : MonoBehaviour
{
    #region Attributes
    [SerializeField] OjbType gObject;

    [SerializeField] GameObject item;

    private Animator animator;
    private TilemapRenderer tilemapRenderer;
    private TilemapCollider2D tilemapCollider;
    private AudioManager audioManager;

    private Gate gate;

    public enum OjbType
    {
        Gate,
        Tile
    }

    [SerializeField] bool shouldNotClose;
    private bool isOn;
    private bool canActivate;
    #endregion

    #region MonoBehavior Methods
    private void Awake()
    {
        if(gObject == OjbType.Gate)
        {
            gate = item.GetComponent<Gate>();
        }

        if(gObject == OjbType.Tile)
        {
            tilemapRenderer = item.GetComponent<TilemapRenderer>();

            tilemapCollider = item.GetComponent<TilemapCollider2D>();
        }

        animator = GetComponent<Animator>();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void Start()
    {
        isOn = false;

        canActivate = true;
    }
    #endregion

    #region Normal Methods
    public void LeverOn()
    {
        if(!canActivate)
        {
            return;
        }

        isOn = true;

        switch(gObject)
        {
            case OjbType.Gate:
                StartCoroutine(GateBehaviour());
            break;

            case OjbType.Tile:
                StartCoroutine(TileBehaviour());
            break;
        }

        if(shouldNotClose)
        {
            Destroy(base.gameObject.GetComponent<CapsuleCollider2D>());
        }
    }

    public void LeverOff()
    {
        if(!canActivate)
        {
            return;
        }

        isOn = false;

        switch(gObject)
        {
            case OjbType.Gate:
                StartCoroutine(GateBehaviour());
            break;

            case OjbType.Tile:
                StartCoroutine(TileBehaviour());
            break;
        }
    }

    public bool GetLeverState()
    {
        return isOn;
    }
    #endregion

    #region Coroutines
    private IEnumerator GateBehaviour()
    {
        canActivate = false;
            
        if(isOn)
        {
            audioManager.Play("LeverOn");

            if(!gate.GetIsOpen())
            {
                gate.OpenGate();
            }
        }
        else
        {
            audioManager.Play("LeverOff");

            if(gate.GetIsOpen())
            {
                gate.CloseGate();
            }
        }

        animator.SetBool("IsOn", isOn);

        yield return new WaitForSeconds(1f);

        canActivate = true;
    }

    private IEnumerator TileBehaviour()
    {
        canActivate = false;

        if(isOn)
        {
            audioManager.Play("LeverOn");

            tilemapRenderer.enabled = false;

            tilemapCollider.enabled = false;
        }
        else
        {
            audioManager.Play("LeverOff");

            tilemapRenderer.enabled = true;

            tilemapCollider.enabled = true;
        }

        animator.SetBool("IsOn", isOn);

        yield return new WaitForSeconds(1f);

        canActivate = true;
    }
    #endregion
}
