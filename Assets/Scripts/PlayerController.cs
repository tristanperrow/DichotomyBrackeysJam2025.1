using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Player State")]
    public bool canMove = true;

    [Header("Options")]
    [Range(1, 20)][SerializeField] private float _moveSpeed = 5f;
    [Range(2, 20)][SerializeField] private float _interactRadius = 2f;
    [SerializeField] private LayerMask _interactLayer;

    [SerializeField] private Material _normMaterial;
    [SerializeField] private Material _highlightMaterial;

    [Header("Player Headbob")]
    [SerializeField] private float _minHeadbobMagnitude = 3f;
    [Range(0.025f, 0.2f)][SerializeField] private float _headbobScale = 0.05f;
    [Range(1f, 20f)]    [SerializeField] private float _headbobRate = 10f;
    [SerializeField] private GameObject _playerHead;

    // private properties
    private Rigidbody2D rb;
    private GameObject _interactableObject;
    private Vector3 _playerHeadOrigin;
    private bool _isFacingRight = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerHeadOrigin = _playerHead.transform.localPosition;
    }

    private void Update()
    {
        FindClosestInteractableObject();

        if (_interactableObject != null)
            UpdateTooltip();

        if (rb.linearVelocity.sqrMagnitude > _minHeadbobMagnitude)
        {
            var bobOffset = Mathf.Sin(Time.time * _headbobRate) * _headbobScale;
            _playerHead.transform.localPosition = _playerHeadOrigin + new Vector3(0f, bobOffset, 0f);
        }
        else
        {
            _playerHead.transform.localPosition = _playerHeadOrigin;
        }
    }

    #region - Public Events -
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canMove) return;

        var moveDirection = context.ReadValue<float>();
        rb.linearVelocity = new Vector2(moveDirection * _moveSpeed, 0f);

        if (moveDirection > 0 && !_isFacingRight)
        {
            Flip();
        }
        else if (moveDirection < 0 && _isFacingRight)
        {
            Flip();
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            InteractWithObject(_interactableObject);
    }
    #endregion

    #region - Methods -

    private void FindClosestInteractableObject()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position + new Vector3(0f,1f,0f), _interactRadius, Vector2.zero, 0f, _interactLayer);

        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = hit.collider.gameObject;
            }
        }

        if (closestObject != _interactableObject)
        {
            // deselect previous object
            if (_interactableObject != null)
            {
                DeselectObject(_interactableObject);
            }

            _interactableObject = closestObject;

            // select the new object
            if (_interactableObject != null)
            {
                SelectObject(_interactableObject);
            }
        }
    }

    // CODE FOR HIGHLIGHTING AN OBJECT
    private void SelectObject(GameObject gameObj)
    {
        var renderer = gameObj.GetComponent<SpriteRenderer>();
        Interactable interactable = gameObj.GetComponent<Interactable>();
        if (renderer != null && interactable != null)
        {
            renderer.color = Color.cyan;
            //renderer.material = _highlightMaterial;

            // shows the interactable's tooltip
            Vector3 ip = new Vector3(interactable.interactionPosition.x, interactable.interactionPosition.y, 0f);
            UIManager.Instance.ShowTooltip(interactable.interactionPrompt, gameObj.transform.position + ip);
        }
    }

    // CODE FOR UNHIGHLIGHTING AN OBJECT
    private void DeselectObject(GameObject gameObj)
    {
        var renderer = gameObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = Color.white;
            //renderer.material = _normMaterial;
            // hides the interactable's tooltip
            UIManager.Instance.HideTooltip();
        }
    }

    // DO INTERACTION CODE HERE
    private void InteractWithObject(GameObject gameObj)
    {
        if (gameObj == null) return;
        Interactable interactable = gameObj.GetComponent<Interactable>();
        if (interactable)
        {
            interactable.Interact();
        }
    }

    private void UpdateTooltip()
    {
        Interactable interactable = _interactableObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            // shows the interactable's tooltip
            Vector3 ip = new Vector3(interactable.interactionPosition.x, interactable.interactionPosition.y, 0f);
            UIManager.Instance.ShowTooltip(interactable.interactionPrompt, _interactableObject.transform.position + ip);
        }
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        // flip only the x component of the scale
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    #endregion

    #region - Gizmos -
    private void OnDrawGizmosSelected()
    {
        // draws interaction sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f,1f,0f), _interactRadius);
    }
    #endregion
}
