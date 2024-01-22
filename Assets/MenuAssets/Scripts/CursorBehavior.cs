using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBehavior : MonoBehaviour
{
    private Vector2 movement;
    [SerializeField] private float cursorSpeed;

    private float screenEdgeThreshold = .02f;
    public bool objectSelected = false;
    public GameObject playerSelection;

    public static EventHandler DoneSelectingEvent;
    public MenuEvents menu;

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        menu = GameObject.Find("Canvas").GetComponent<MenuEvents>();
    }
    void Update()
    {
        // Don't allow the cursor past the edge of the screen!
        var viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if ((viewportPosition.x < screenEdgeThreshold && movement.x < 0) ||
            (viewportPosition.x > 1 - screenEdgeThreshold && movement.x > 0) ||
            (viewportPosition.y < screenEdgeThreshold && movement.y < 0) ||
            (viewportPosition.y > 1 - screenEdgeThreshold && movement.y > 0))
            return;

        // Moves the cursor
        transform.Translate(new Vector3(movement.x, movement.y, 0f) * cursorSpeed/1000);
        // UnityEngine.Debug.Log("There are currently " + PlayerInput.all.Count + " players.");
    }



    public void OnCursorMove(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Canceled && !objectSelected)
            movement = context.ReadValue<Vector2>();
        else  // Released button!
            movement = Vector2.zero;
    }


    public void OnSelectButton (InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // Debug.DrawRay(transform.position, Vector3.forward, Color.red, 50f);

            RaycastHit2D target;
            target = Physics2D.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.forward, 1000f, LayerMask.GetMask("PlayerObjects"));
            if (target)
            {
                if (!objectSelected)
                {
                    objectSelected = true;
                    playerSelection = target.transform.gameObject;
                    menu.PreviewConq(playerSelection.tag, gameObject.name.Substring(0,2));
                    return;
                }
            }

            if (objectSelected)
            {
                objectSelected = false;
                playerSelection = null;
                menu.ClearConq(gameObject.name.Substring(0, 2));
            }
        }
    }



    public void OnStartButton(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            // UnityEngine.Debug.Log("Start method tripped");
            DoneSelectingEvent?.Invoke(this, EventArgs.Empty);
        }
    }


}
