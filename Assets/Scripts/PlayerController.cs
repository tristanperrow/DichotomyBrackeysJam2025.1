using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;

    [Header("Options")]
    [Range(1, 20)][SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _interactRadius = 2f;
    [SerializeField] private LayerMask _interactLayer;

    // properties
    private GameObject _interactableObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FindClosestInteractableObject();
    }

    #region - Public Events -
    public void OnMove(InputAction.CallbackContext context)
    {
        var moveDirection = context.ReadValue<float>();
        rb.linearVelocity = new Vector2(moveDirection * _moveSpeed, 0f);
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
        if (renderer != null)
        {
            renderer.color = Color.cyan;
        }
    }

    // CODE FOR UNHIGHLIGHTING AN OBJECT
    private void DeselectObject(GameObject gameObj)
    {
        var renderer = gameObj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = Color.white;
        }
    }

    // DO INTERACTION CODE HERE
    private void InteractWithObject(GameObject gameObj)
    {
        if (gameObj == null) return;
        Debug.Log("Interacted with: " + gameObj.name);
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
