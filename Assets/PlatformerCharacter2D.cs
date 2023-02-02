using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private float m_SecondJumpForce = 300f;                  // Amount of force added when the player double jumps.
        [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
        [SerializeField] private LayerMask m_WhatIsPortal;

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .01f; // Radius of the overlap circle to determine if grounded
        public bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        public float m_DamagePercentage = 0.0f;
        public bool m_FacingRight = true;  // For determining which way the player is currently facing.
        public bool m_Jump;
        private bool m_JumpedOnce = false;
        private bool m_JumpedTwice = false;
        public float m_JumpCooldown = .08f;
        public float m_TimeSinceJump = 0.0f;
        public float m_playerHeight;
        public float m_playerWidth;
        private float hitStunTime;
        private float bufferTime = 0.0f;

        public Text m_DamageDisplay;
        public Text m_InfoDisplay;


        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_playerHeight = transform.localScale.y;
            m_playerWidth = transform.localScale.x;

            UpdateAnimClipTimes();
        }

        private void Update()
        {
            m_DamageDisplay.text = m_DamagePercentage + "%";
            CheckOverlaps();

            bufferTime += Time.deltaTime;
        }


        private void FixedUpdate()
        {
            
        }

        public void UpdateAnimClipTimes()
        {
            AnimationClip[] clips = m_Anim.runtimeAnimatorController.animationClips;
            foreach (AnimationClip clip in clips)
            {
                switch (clip.name)
                {
                    case "HitStun":
                        hitStunTime = clip.length;
                        break;
                }
            }
        }

        //Called whenever player collides with something
        void OnCollisionEnter2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag("hitbox"))
            {
                m_DamagePercentage += 3.0f;
            }
            
        }

        private void OnCollisionStay2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag("PortalForeground"))
            {
                m_InfoDisplay.text = "Press Enter";
                if (CrossPlatformInputManager.GetButtonDown("Submit"))
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 10, 25);
                    transform.tag = "PlayerMid";
                    gameObject.layer = LayerMask.NameToLayer("PlayerMid");
                    var currRenderer = gameObject.GetComponent<SpriteRenderer>();
                    currRenderer.sortingLayerName = "Background";
                }
            }
        }

        private void OnCollisionExit2D(Collision2D coll)
        {
            if (coll.gameObject.CompareTag("PortalForeground"))
            {
                m_InfoDisplay.text = "";
            }
        }

        private void CheckOverlaps()
        {
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            contactFilter2D.useLayerMask = true;
            contactFilter2D.SetLayerMask(m_WhatIsPortal);

            List<Collider2D> collisions = new List<Collider2D>();
            int colCount = collider.OverlapCollider(contactFilter2D, collisions);
            int i = 0;
            bool atPortal = false;
            foreach (Collider2D coll in collisions)
            {
                //Check for portal
                if (coll.gameObject.CompareTag("PortalForeground"))
                {
                    atPortal = true;
                    m_InfoDisplay.text = "Press Enter";
                    if (CrossPlatformInputManager.GetButtonDown("Submit"))
                    {
                        transform.position = new Vector3(transform.position.x, transform.position.y + 15, 25);
                        transform.tag = "PlayerMid";
                        gameObject.layer = LayerMask.NameToLayer("PlayerMid");
                        var currRenderer = gameObject.GetComponent<SpriteRenderer>();
                        currRenderer.sortingLayerName = "Background";
                    }
                }
            }
            if (atPortal == false)
            {
                m_InfoDisplay.text = "";
            }
        }


        public void Move(float move, bool crouch)
        {
            // If crouching, check to see if the character can stand up
            if (!crouch && m_Anim.GetBool("Crouch"))
            {
                // If the character has a ceiling preventing them from standing up, keep them crouching
                if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
                {
                    crouch = true;
                }
            }

            // Set whether or not the character is crouching in the animator
            m_Anim.SetBool("Crouch", crouch);

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // Reduce the speed if crouching by the crouchSpeed multiplier
                move = (crouch ? move*m_CrouchSpeed : move);

                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                if (bufferTime >= hitStunTime )
                {
                    m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);
                }
                    
                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            //Tracking jumps
            m_JumpCooldown = .1f;
            if (m_Jump)
            {
                if (!m_JumpedOnce)
                {
                    m_JumpedOnce = true;
                    m_TimeSinceJump = 0.0f;
                }
                else if (m_JumpedOnce && !m_JumpedTwice && m_TimeSinceJump > m_JumpCooldown)
                {
                    // Read the jump input in Update so button presses aren't missed.
                    if (m_Jump)
                    {
                        m_JumpedTwice = true;
                    }
                }
                else
                {
                    m_Jump = false;
                }

            }

            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
            //RaycastHit2D groundContact = Physics2D.Raycast(transform.position, -Vector2.up, m_CharDimensions.rect.height / 2);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject && m_TimeSinceJump > m_JumpCooldown)
                {
                    if ((colliders[i].gameObject.CompareTag("MidGround") && !transform.CompareTag("PlayerMid")) || (colliders[i].gameObject.CompareTag("Foreground") && !transform.CompareTag("Player")))
                    {
                        Physics2D.IgnoreCollision(colliders[i], gameObject.GetComponent<BoxCollider2D>(), true);
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(colliders[i], gameObject.GetComponent<BoxCollider2D>(), false);
                        m_Grounded = true;
                    }
                }
                if (colliders[i].gameObject.CompareTag("PortalForeground"))
                {
                    if (CrossPlatformInputManager.GetButtonDown("Submit"))
                    {
                        transform.position = new Vector3(55, 16, 25);
                        transform.tag = "PlayerMid";
                        gameObject.layer = LayerMask.NameToLayer("PlayerMid");
                        var currRenderer = gameObject.GetComponent<SpriteRenderer>();
                        currRenderer.sortingLayerName = "Background";
                    }
                }


            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);


            if (m_Grounded)
            {
                m_JumpedOnce = false;
                m_JumpedTwice = false;
                m_TimeSinceJump = 0.11f;
            }


            // If the player should jump...
            if (m_JumpedOnce && m_Jump && !m_JumpedTwice)
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                Vector2 newVelocity = m_Rigidbody2D.velocity;
                newVelocity.y = 0f;
                m_Rigidbody2D.velocity = newVelocity;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            // If the player should double jump...
            if (!m_Grounded && m_Jump && m_JumpedTwice)
            {
                Vector2 newVelocity = m_Rigidbody2D.velocity;
                newVelocity.y = 0f;
                m_Rigidbody2D.velocity = newVelocity;
                m_Rigidbody2D.AddForce(new Vector2(0f, 15f), ForceMode2D.Impulse);
            }
        }


        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        public void TakeDamage(float damage)
        {
            m_DamagePercentage += damage;

            //Play hurt animation
        }



        public void Knockback(float BaseKB, float contactAngle, float modifierx, float modifiery)
        {
            m_Anim.SetTrigger("Hitstun");
            bufferTime = 0.0f;

            //Make a vector inverse of collision angle
            float radians = contactAngle * Mathf.Deg2Rad;
            Vector2 KBVector = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));

            //Calculate knockback force
            KBVector.x = KBVector.x * BaseKB * (m_DamagePercentage * 0.75f) + modifierx;
            KBVector.y = KBVector.y * BaseKB * (m_DamagePercentage * 0.75f) + modifiery;

            KBVector.x = Mathf.Round(KBVector.x * 100f) / 100f;
            KBVector.y = Mathf.Round(KBVector.y * 100f) / 100f;

            var e_Rigidbody2D = GetComponent<Rigidbody2D>();

            //Note to self: failing to apply x knockback here because x velocity is being modified by line 157
            e_Rigidbody2D.AddForce(KBVector, ForceMode2D.Impulse);
        }
    }
}
