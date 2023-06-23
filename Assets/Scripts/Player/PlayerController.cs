using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum MovementState
{
    walking,
    sprinting,
    inAir,
    crouch,
    wallRunning,
    freeze

};

public class PlayerController : MonoBehaviour
{
    //weaponSwitch
    static WeaponSwitch weaponSwitch;

    //Variable para manejar el Estado de Movimiento
    private MovementState movementState;

    // UI
    private UIController UI;

    [Header("Velocidad de Movimiento")]
    private float velocidadMovimiento;
    [SerializeField] private float velCaminar;
    [SerializeField] private float velSprintar;
    [SerializeField] private float velAgachado;
    [SerializeField] private float velWallRun;

    //VARIABLES PARA Agacharse (Crouch)
    private float escalaAgachadoY = 0.3f;
    private float escalaInicial;

    [Header("Velocidad de Rotaci�n")]
    [SerializeField]
    private float turnSpeed;

    [Header("Distancia de Disparo")]
    [SerializeField]
    private float[] shootDistance;

    [Header("Particulas de Disparo)")]
    [SerializeField]
    private ParticleSystem[] shootPS;

    [Header("Salud")]
    [SerializeField]
    public float health;

    [Header("Cuerpo")]
    [SerializeField]
    private Transform cuerpo;

    [Header("Control de Fricci�n")]
    [SerializeField] 
    private LayerMask capaSuelo;

    private float friccionSuelo = 5;
    private float multiplicadorDeAire = 0.4f;

    //Flags de pegado al suelo y a Pared
    private bool enElSuelo;
    private bool wallRunning;
    private bool freeze;
    private bool ganchoActivo;

    private Vector3 velocidadResultanteDeGancho;
    private bool permitirMovimientoEnSiguienteAccion;


    [Header("Clip de Disparo")]
    public AudioClip[] clipDisparo;

    //Referencia a C�mara
    private Transform cameraMain;

    private Rigidbody mRb;
    private AudioSource mAudioSource;

    private Vector2 mDirection;
    private Vector2 mDeltaLook;

    private GameObject debugImpactSphere;

    private GameObject bloodObjectParticles;
    private GameObject otherObjectParticles;

    public bool EnElSuelo { get => enElSuelo; set => enElSuelo = value; }
    public bool WallRunning { get => wallRunning; set => wallRunning = value; }
    public Vector2 MDirection { get => mDirection; set => mDirection = value; }
    public bool Freeze { get => freeze; set => freeze = value; }
    public bool GanchoActivo { get => ganchoActivo; set => ganchoActivo = value; }
    public Transform Cuerpo { get => cuerpo; set => cuerpo = value; }

    //-----------------------------------------------------------------------------------------

    private void Start()
    {
        //Inicializamoe salud en 100
        health = 100;

        //Seteamos este como el PlayerController que el GameManager debe evaluar
        GameManager.Instance.Player = this;

        //Obtenemos la referencia al array de weapons
        weaponSwitch = gameObject.GetComponent<WeaponSwitch>();

        //Obtenemos referencia al componente RigidBody
        mRb = GetComponent<Rigidbody>();
        mAudioSource = GetComponent<AudioSource>();
        
        //Interfaz
        UI = GetComponent<UIController>();

        //Obtenemos referencia a la Camara Principal (Vista de jugador)
        cameraMain = transform.Find("SujetadorDeCamara").Find("Main Camera");

        //Asignamos las Part�culas a trav�s de la Carpeta de Resources
        debugImpactSphere = Resources.Load<GameObject>("DebugImpactSphere");
        bloodObjectParticles = Resources.Load<GameObject>("BloodSplat_FX Variant");
        otherObjectParticles = Resources.Load<GameObject>("GunShot_Smoke_FX Variant");


        //Bloqueamos el Cursor para que este no sea visible
        Cursor.lockState = CursorLockMode.Locked;

        //Almacenamos la escala inical (en Y)
        escalaInicial = transform.localScale.y;
    }

    //-------------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        MoverseConFuerza();
    }

    //--------------------------------------------------------------------------------------------

    private void Update()
    {
        //Manejamos los estados para controlar el movimiento que se realizar�
        StateHandler();

        ControlarVelocidad();

        DetectarSuelo();

        ControlarAgachado();

        ControlarRotacion();
    }

    //-------------------------------------------------------------------------------------------
    //Manejador de Estados para controlar la Velocidad seg�n cada acci�n o evento en suceso
    private void StateHandler()
    {
        //Modo - Quieto (enganchado)
        if (freeze)
        {
            movementState = MovementState.freeze;
            velocidadMovimiento = 0;
            mRb.velocity = Vector3.zero;
        }

        //Modo - WallRunning
        else if (wallRunning)
        {
            movementState = MovementState.wallRunning;
            velocidadMovimiento = velWallRun;
            multiplicadorDeAire = 1;
        }
        //Modo - AGACHADO
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            movementState = MovementState.crouch;
            velocidadMovimiento = velAgachado;
            multiplicadorDeAire = 1;
        }
        //Modo - SPRINTING
        else if (Input.GetKey(KeyCode.LeftShift) && enElSuelo)
        {
            movementState = MovementState.sprinting;
            velocidadMovimiento = velSprintar;
            multiplicadorDeAire = 1;
        }

        //Modo - WALKING
        else if (enElSuelo)
        {
            movementState = MovementState.walking;
            velocidadMovimiento = velCaminar;
            multiplicadorDeAire = 1f;
        }

        //Modo - EN EL AIRE
        else
        {
            movementState = MovementState.inAir;
            multiplicadorDeAire = 0.4f;
        }
    }

    //--------------------------------------------------------------------------------------------
    #region InputActions
    private void OnMove(InputValue value)
    {
        //Obtenemos Direccion de movimiento (en un Vector2, con X e Y)
        mDirection = value.Get<Vector2>();
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    private void OnJump(InputValue value)
    {
        //Si se oprime el boton de Salto
        if (value.isPressed)
        {
            //Si el Player est� en el suelo...
            if (enElSuelo)
            {
                //Saltamos A�adiendo una fuerza de impulso
                mRb.AddForce(Vector3.up * 4.25f, ForceMode.Impulse);
            }
        }
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    private void OnLook(InputValue value)
    {
        //Obtenemos el Vector2 generado por la rotaci�n del Raton
        mDeltaLook = value.Get<Vector2>();
    }

    // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

    private void OnFire(InputValue value)
    {
        //Si se oprime el boton de disparo
        if (value.isPressed)
        {
            //DISPARA
            Shoot();
        }
    }
    #endregion
    //--------------------------------------------------------------------------------------------

    private void MoverseConFuerza()
    {
        //Si el Gancho esta activo, no te muevas
        if (ganchoActivo) return;

        //Aplicamos una fuerza al RB del Player para moverlo
        mRb.AddForce(
            (mDirection.y * transform.forward + mDirection.x * transform.right).normalized * velocidadMovimiento * multiplicadorDeAire,
            ForceMode.Force
            );

    }

    //-----------------------------------------------------------------------------------

    private void ControlarAgachado()
    {
        //Si se est� oprimiendo la tecla Control, y estamos sobre el suelo...
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //Reducimos la escala del player
            transform.localScale = new Vector3(
                transform.localScale.x,
                escalaAgachadoY,
                transform.localScale.z);

            //Empujamos hacia abajo a modo de impulso
            mRb.AddForce(Vector3.down * 5, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            //Devolvemos a la Escala Inicial la escala del player
            transform.localScale = new Vector3(
                transform.localScale.x,
                escalaInicial,
                transform.localScale.z);
        }
    }

    //----------------------------------------------------------------------------------------------

    private void ControlarVelocidad()
    {
        //Si el Gancho est� activo, no controles la velocidad
        if (ganchoActivo) return;

        Vector3 velocidadPlana = new Vector3(mRb.velocity.x, 0, mRb.velocity.z);

        //Limitamos la velocidad dentro del limite
        if (velocidadPlana.magnitude > velocidadMovimiento)
        {
            Vector3 velocidadLimitada = velocidadPlana.normalized * velocidadMovimiento;
            mRb.velocity = new Vector3(velocidadLimitada.x, mRb.velocity.y, velocidadLimitada.z);
        }
    }
    //----------------------------------------------------------------------------------

    private void ControlarRotacion()
    {
        //Actualizamos constantemente la rotaci�n horizontal del Player en torno al Eje Y
        transform.Rotate(
            Vector3.up,
            turnSpeed * Time.deltaTime * mDeltaLook.x
        );

        ////Actualizamos constantemente la rotaci�n vertical del Player en torno al Eje X
        cameraMain.GetComponent<CameraMovement>().RotateUpDown(
            -turnSpeed * Time.deltaTime * mDeltaLook.y
        );
    }

    //---------------------------------------------------------------------------------------------


    private void DetectarSuelo()
    {
        //Si Detectamos que estamos en contacto con el suelo; y el gancho NO esta activo
        if (Physics.Raycast(cuerpo.position, Vector3.down, 0.45f, capaSuelo) && !ganchoActivo)
        {
            enElSuelo = true;
            //Asignamos el Drag del suelo
            mRb.drag = friccionSuelo; //5
        }
        else if (ganchoActivo)
        {
            enElSuelo = false;
            mRb.drag = 0f;
        }
        else
        {
            enElSuelo = false;
            mRb.drag = 0.75f;
        }
    }

    //-----------------------------------------------------------------------------------------------

    private void Shoot()
    {
        //Posicion actual del array weapons
        int selectedWeaponIndex = weaponSwitch.selectedWeapon;
        //Reproducimos el sistema de Particulas del Disparo
        shootPS[selectedWeaponIndex].Play();
        
        mAudioSource.PlayOneShot(clipDisparo[selectedWeaponIndex], 0.40f);

        //Lanzamos un RAYCAST hacia el frente, considerando la distancia de disparo delimitada
        RaycastHit hit;

        //Si impactamos algo...
        if (Physics.Raycast(cameraMain.position,cameraMain.forward,out hit,shootDistance[selectedWeaponIndex]))
        {
            //Si la etiqueta del Objeto impactado es "ENEMIGOS"
            if (hit.collider.CompareTag("Enemigos"))
            {
                //Instanciamos el Sistema de Particulas para la Sangre, en el punto dexacto donde impacta el disparo
                var bloodPS = Instantiate(bloodObjectParticles, hit.point, Quaternion.identity);

                //Destruimos las Particulas habiendo pasado 3 segundos
                Destroy(bloodPS, 3f);


                //Calculamos el daño inflingido segun el arma que tengamos

                float ataqueinflingido = 0;

                if (selectedWeaponIndex == 0) ataqueinflingido = 2f;
                else ataqueinflingido = 1f;

                ZombieController zombieController = null;
                EnemyController enemyCnotroller = null;
                CrawlerController crawlerController = null;

                //Obtenemos referencia al Controller del enemigo impactado
                //Tras esto, le hacemos daño

                if (hit.collider.TryGetComponent<ZombieController>(out zombieController))
                {
                    zombieController.TakeDamage(ataqueinflingido);
                }
                else if (hit.collider.TryGetComponent<EnemyController>(out enemyCnotroller))
                {
                    enemyCnotroller.TakeDamage(ataqueinflingido);
                }
                else if (hit.collider.TryGetComponent<CrawlerController>(out crawlerController))
                {
                    crawlerController.TakeDamage(ataqueinflingido);
                }

            }
            
            //En caso el Objeto impactado no sea un Zombie...
            else
            {
                //Instanciamos Y Reproducimos el Sistema de Particulas de P�lvora, en el punto donde impacta el disparo
                var otherPS = Instantiate(otherObjectParticles, hit.point, Quaternion.identity);
                otherPS.GetComponent<ParticleSystem>().Play();

                //Destruimos las part�culas habiendo pasado 3 segundos
                Destroy(otherPS, 3f);
            }
            
        }
    }

    //-------------------------------------------------------------------------------------------------

    public void SaltoConGanchoHaciaPosicion(Vector3 posicionObjetivo, float alturaDeTrayectoria)
    {
        //Activamos el Flag de Gancho Activo
        ganchoActivo = true;

        //Almacenamos la Velocidad resultado del calculo
        velocidadResultanteDeGancho = CalcularVelocidadDeSaltoGancho(cuerpo.position, posicionObjetivo, alturaDeTrayectoria);

        Invoke(nameof(AsignacionDeVelocidadGancho), 0.1f);

        //Reiniciamos las restricciones y habilitamos el movimiento pasados 3 segundos
        Invoke(nameof(ReiniciarRestricciones), 3f);
    }

    //---------------------------------------------------------------------------------------------

    public void AsignacionDeVelocidadGancho()
    {
        permitirMovimientoEnSiguienteAccion = true;
        mRb.velocity = velocidadResultanteDeGancho;
    }

    //---------------------------------------------------------------------------------------------------

    public Vector3 CalcularVelocidadDeSaltoGancho(Vector3 puntoInicial, Vector3 puntoFinal, float alturaDeTrayectoria)
    {
        //Obtenemos la gravedad (Fuerza constante en Y)
        float gravedad = Physics.gravity.y;

        //Obtenemos el desplazamiento que se llevar� a cabo en Y
        float desplazamientoEnY = puntoFinal.y - puntoInicial.y;

        //Obtenemos el desplazamiento que se llevar� a cabo en X y Z
        Vector3 desplazamientoEnXZ = new Vector3(
            puntoFinal.x - puntoInicial.x,
            0f,
            puntoFinal.z - puntoInicial.z
            );

        //Calculamos la velocidad en Y
        Vector3 velocidadEnY = Vector3.up * Mathf.Sqrt(-2 * gravedad * alturaDeTrayectoria);

        //Calculamos la velocidad en X y Z
        Vector3 velocidadEnXZ = desplazamientoEnXZ / (Mathf.Sqrt(-2 * alturaDeTrayectoria / gravedad)
            + Mathf.Sqrt(2 * (desplazamientoEnY - alturaDeTrayectoria) / gravedad));

        //Regreamos el Vector Velocidad total
        return velocidadEnXZ + velocidadEnY;
    }

    //----------------------------------------------------------------------------------------------------

    public void TakeDamage(float damage)
    {
        //Le restamos al FLOAT de SALUD el da�o recibido
        health -= damage;

        //Si la salud se reduci� por debajo dle limite
        if (health <= 0f)
        {
            // Fin del juego
            Debug.Log("Fin del juego");
        }
    }

    //-------------------------------------------------------------------

    public void ReiniciarRestricciones()
    {
        ganchoActivo = false;
    }

    //----------------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (permitirMovimientoEnSiguienteAccion)
        {
            permitirMovimientoEnSiguienteAccion = false;
            ReiniciarRestricciones();

            GetComponent<Grappling>().StopGrapple();
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        //Si entramos en contacto con el Collider del ataque del enemigo...
        if (col.CompareTag("Enemigo-Attack"))
        {
            //Llamamos a la funci�n de Recibir Da�o
            TakeDamage(1f);
        }
        
    }

    //-----------------------------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(cuerpo.position, Vector3.down * 0.45f);
    }

    // Funciones públicas para obtener estados
    public bool IsOnFloor(){
        return enElSuelo;
    }
    public bool IsGrappling(){
        return ganchoActivo;
    }
    public bool IsWallrunning(){
        return wallRunning;
    }
    public bool IsInAir(){
        //bool isInAir = movementState =
        if(movementState == MovementState.inAir){
            return true;
        }else{
            return false;
        }
    }

}
