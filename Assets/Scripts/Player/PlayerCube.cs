using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;



namespace Com.FoxGames.CubeShooter
{
    public class PlayerCube : Unit
    {



        #region Static Fields ==========================================================================================



        public const float MIN_VALUE = 0.05f; // very small value, used to get around quirks in engine behavior 



        #endregion



        #region Serializable Fields ====================================================================================



        [SerializeField]
        private float acceleration = 100.0f;

        [SerializeField]
        private float maxVelocity = 10.0f;

        [SerializeField]
        private float turnSpeed = 100.0f;

        [SerializeField]
        private float frictionCoefficient = 0.1f;

        [SerializeField]
        private float jumpSpeed = 100.0f;

        [SerializeField]
        private float gravity = 10.0f;

        [SerializeField]
        private float airControl = 0.5f;

        [SerializeField]
        private int maxHealth = 100;

        [SerializeField]
        private List<Weapon> weapons = new List<Weapon>();



        #endregion



        #region Public Fields



        public Weapon equippedWeapon
        {
            get
            {
                if (weapons.Count > 0)
                {
                    return weapons[equippedWeaponIndex];
                }
                else
                {
                    return null;
                }
            }
        }



        #endregion



        #region Private Fields



        // Components
        
        private Animator animator = null;

        private new SphereCollider collider = null;
        
        private new Rigidbody rigidbody = null;

        private PhotonView photonView = null;

        private Transform weaponLocationTransform = null;

        private PlayerLabel playerLabel = null;



        // Inputs

        private Vector2 inputVector = Vector2.zero;

        private bool jumpPressed = false;



        // Player Fields

        private Vector3 xzVelocity = Vector3.zero;

        private float yVelocity = 0;

        private Vector3 moveVector = Vector3.zero;

        private bool isGrounded = false;

        private int health = 0;

        private int equippedWeaponIndex = 0;



        // Timers

        private Timer fireTimer = null;

        private Timer reloadTimer = null;



        #endregion



        #region MonoBehaviour Callbacks



        private void Awake()
        {
            animator = GetComponent<Animator>();

            collider = GetComponent<SphereCollider>();

            rigidbody = GetComponent<Rigidbody>();

            photonView = GetComponent<PhotonView>();



            weaponLocationTransform = transform.Find("Mesh/Weapon Location");

            playerLabel = transform.Find("Player Label").GetComponent<PlayerLabel>();

            fireTimer = new Timer();

            reloadTimer = new Timer();
        }



        private void Start()
        {
            health = maxHealth;

            if (playerLabel)
            {
                if (!PhotonNetwork.IsConnected || photonView.IsMine)
                {
                    playerLabel.SetText("");
                }
                else
                {
                    playerLabel.SetText(photonView.Owner.NickName);
                }
            }

            if (!PhotonNetwork.IsConnected || photonView.IsMine)
            {
            }
        }



        protected virtual void Update()
        {
            if (!PhotonNetwork.IsConnected || photonView.IsMine)
            {
                if (!ClientManager.Instance.isPaused)
                {
                    // Read input

                    inputVector = new Vector2( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") );

                    inputVector = Vector2.ClampMagnitude(inputVector, 1.0f);

                    if (Input.GetButtonDown("Jump"))
                    {
                        jumpPressed = true;
                    }



                    // Handle Rotation

                    Vector3 mousePoint = MouseInput.MousePoint;

                    mousePoint.y = transform.position.y;

                    Vector3 lookDirection = mousePoint - transform.position;

                    if (lookDirection != Vector3.zero)
                    {
                        transform.rotation = Quaternion.Slerp(
                            transform.rotation,
                            Quaternion.LookRotation(lookDirection),
                            turnSpeed
                        );
                    }



                    // Handle Click

                    if (Input.GetMouseButtonDown(0) && MouseInput.IsOnMouseRayPlane)
                    {
                        Fire();
                    }
                }
                else
                {
                    inputVector = Vector2.zero;
                }
            }  
        }



        private void FixedUpdate()
        {
            if ((!PhotonNetwork.IsConnected || photonView.IsMine))
            {



                // Handle XZ Movement

                Vector3 xzInputVector = new Vector3( inputVector.x, 0, inputVector.y );

                // Base Movement direction on input vector
                xzInputVector = Quaternion.Euler(0, ClientManager.GetCameraRotation().eulerAngles.y, 0) * xzInputVector;

                float inputMagnitude = inputVector.magnitude;

                // Only accelerate if there is input being read
                if (inputMagnitude > MIN_VALUE)
                {
                    // Accelerate in the direction of input.
                    Vector3 accelerationVector = xzInputVector * acceleration;
                    
                    if (!isGrounded)
                    {
                        accelerationVector *= airControl;
                    }

                    if (xzVelocity.magnitude <= maxVelocity)
                    {
                        xzVelocity = Vector3.ClampMagnitude(
                            xzVelocity + accelerationVector, 
                            maxVelocity
                        );
                    }
                    else
                    {
                        xzVelocity = xzVelocity + accelerationVector;
                    }
                }



                // Handle Y Movement

                if (isGrounded)
                {
                    if (jumpPressed)
                    {
                        yVelocity = jumpSpeed;

                        animator.SetTrigger("Jump");
                    }

                    if (yVelocity < 0)
                    {
                        yVelocity = 0;
                    }
                }
                else
                {
                    yVelocity -= gravity;
                }

                jumpPressed = false;



                // Check if Cube is Grounded

                RaycastHit groundHitInfo;

                bool isTouchingGround = Physics.SphereCast(
                    transform.position + collider.center,
                    collider.radius - MIN_VALUE,
                    Vector3.down,
                    out groundHitInfo,
                    MIN_VALUE * 2,
                    Global.TerrainMask
                );

                isGrounded = (isTouchingGround && yVelocity <= 0);



                // Apply friction
                if (isGrounded)
                {
                    xzVelocity -= xzVelocity * frictionCoefficient;
                }
                else
                {
                    xzVelocity -= xzVelocity * frictionCoefficient * airControl;
                }

                if (xzVelocity.magnitude < MIN_VALUE)
                {
                    xzVelocity = Vector3.zero;
                }



                // Apply final movement

                rigidbody.velocity = new Vector3(xzVelocity.x, yVelocity, xzVelocity.z);



                // Set Animator Parameters

                animator.SetBool("Is Grounded", isGrounded);

                animator.SetFloat("Y Velocity", yVelocity);

                animator.SetFloat("Move Speed", inputMagnitude);



                // Check Death Zone

                if (transform.position.y < -50)
                {
                    Die();
                }
            }
        }



        #endregion



        #region Public Methods =========================================================================================



        public override void Hurt(int damage)
        {
            _Hurt(damage);

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_Hurt", RpcTarget.OthersBuffered, damage);
            }
        }



        public override void KnockBack(Vector3 knockbackVector)
        {
            _KnockBack(knockbackVector);

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_KnockBack", RpcTarget.Others, knockbackVector);
            }
        }



        public void Die()
        {
            _Die();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("_Die", RpcTarget.Others);
            }
        }



        #endregion



        #region Private Methods ========================================================================================



        private void Fire()
        {
            if (weapons.Count > 0)
            {
                Weapon weapon = weapons[equippedWeaponIndex];

                bool weaponFired = weapon.Fire(this);

                if (weaponFired)
                {
                    _AnimateFire(weapon.fireEffectResource);

                    if (PhotonNetwork.IsConnected)
                    {
                        photonView.RPC("_AnimateFire", RpcTarget.Others, weapon.fireEffectResource);
                    }
                }
            }
        }



        [PunRPC]
        private void _AnimateFire(string effectResource)
        {
            GameObject effectPrefab = Resources.Load<GameObject>(effectResource);

            Instantiate(effectPrefab, weaponLocationTransform);
        }



        [PunRPC]
        private void _Hurt(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                Die();
            }
        }



        [PunRPC]
        private void _KnockBack(Vector3 knockbackVector)
        {
            Debug.Log(xzVelocity + new Vector3(knockbackVector.x, 0, knockbackVector.z));

            xzVelocity += new Vector3(knockbackVector.x, 0, knockbackVector.z);

            yVelocity += knockbackVector.y;
        }


        
        [PunRPC]
        private void _Die()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (photonView.IsMine)
                {
                    ClientManager.StartRespawn();

                    PhotonNetwork.Destroy(gameObject);
                }
            }
            else
            {
                ClientManager.StartRespawn();

                Destroy(gameObject);
            }

        }



        #endregion
    }
}