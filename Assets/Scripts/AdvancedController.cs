using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AdvancedController : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private float turnSpeed;

    [SerializeField]
    private float shootDistance = 4f;

    [SerializeField]
    private ParticleSystem shootPS;

    [SerializeField]
    private float health;

    private Transform cameraMain;

    private Rigidbody mRb;

    private Vector2 mDirection;
    private Vector2 mDeltaLook;

    private GameObject debugImpactSphere;

    private GameObject bloodObjectParticles;
    private GameObject otherObjectParticles;

    //-----------------------------------------------------------------------------------------

    private void Start()
    {
        //Obtenemos referencia al componente RigidBody
        mRb = GetComponent<Rigidbody>();

        //Obtenemos referencia a la Camara Principal (Vista de jugador)
        cameraMain = transform.Find("Main Camera");

        //Asignamos las Partículas a través de la Carpeta de Resources
        debugImpactSphere = Resources.Load<GameObject>("DebugImpactSphere");
        bloodObjectParticles = Resources.Load<GameObject>("BloodSplat_FX Variant");
        otherObjectParticles = Resources.Load<GameObject>("GunShot_Smoke_FX Variant");

        //Bloqueamos el Cursor para que este no sea visible
        Cursor.lockState = CursorLockMode.Locked;
    }
    //-------------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Aplicamos una fuerza al RB del Player
        mRb.AddForce(
            mDirection.y * transform.forward + mDirection.x * transform.right,
            ForceMode.Force
            );
    }

    private void Update()
    {
        //------------------------------------------------------------------------------
        //------------------------------------ SE QUEDA --------------------------------
        //Actualizamos constantemente la rotación horizontal del Player en torno al Eje Y
        transform.Rotate(
            Vector3.up,
            turnSpeed * Time.deltaTime * mDeltaLook.x
        );

        ////Actualizamos constantemente la rotación vertical del Player en torno al Eje X
        cameraMain.GetComponent<CameraMovement>().RotateUpDown(
            -turnSpeed * Time.deltaTime * mDeltaLook.y
        );
        //-------------------------------------------------------------------------------------

    }

    //---------------------------------------------------------------------------------------------

    private void OnMove(InputValue value)
    {
        //Obtenemos Direccion de movimiento (en un Vector2, con X e Y)
        mDirection = value.Get<Vector2>();
    }
    private void OnJump(InputValue value)
    {
        //Si se oprime el boton de Salto
        if (value.isPressed)
        {
            mRb.AddForce(Vector3.up * 20, ForceMode.Impulse);
        }
    }

    private void OnLook(InputValue value)
    {
        //Obtenemos el Vector2 generado por la rotación del Raton
        mDeltaLook = value.Get<Vector2>();
    }

    private void OnFire(InputValue value)
    {
        //Si se oprime el boton de disparo
        if (value.isPressed)
        {
            //DISPARA
            Shoot();
        }
    }

    //-----------------------------------------------------------------------------------------------

    private void Shoot()
    {
        //Reproducimos el sistema de Particulas del Disparo
        shootPS.Play();

        //Lanzamos un RAYCAST hacia el frente, considerando la distancia de disparo delimitada
        RaycastHit hit;

        //Si impactamos algo...
        if (Physics.Raycast(cameraMain.position, cameraMain.forward, out hit, shootDistance))
        {
            //Si la etiqueta del Objeto impactado es "ENEMIGOS"
            if (hit.collider.CompareTag("Enemigos"))
            {
                //Instanciamos el Sistema de Particulas para la Sangre, en el punto dexacto donde impacta el disparo
                var bloodPS = Instantiate(bloodObjectParticles, hit.point, Quaternion.identity);
                //Destruimos las Particulas habiendo pasado 3 segundos
                Destroy(bloodPS, 3f);

                //Obtenemos referencia al EnemyController del enemigo impactado
                var enemyController = hit.collider.GetComponent<EnemyController>();
                //Hacemos que reciba daño
                enemyController.TakeDamage(1f);

            }

            //En caso el Objeto impactado no sea un Zombie...
            else
            {
                //Instanciamos Y Reproducimos el Sistema de Particulas de Pólvora, en el punto donde impacta el disparo
                var otherPS = Instantiate(otherObjectParticles, hit.point, Quaternion.identity);
                otherPS.GetComponent<ParticleSystem>().Play();

                //Destruimos las partículas habiendo pasado 3 segundos
                Destroy(otherPS, 3f);
            }

        }
    }

    //---------------------------------------------------------------------------------------------------

    public void TakeDamage(float damage)
    {
        //Le restamos al FLOAT de SALUD el daño recibido
        health -= damage;

        //Si la salud se redució por debajo dle limite
        if (health <= 0f)
        {
            // Fin del juego
            Debug.Log("Fin del juego");
        }
    }

    //----------------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider col)
    {
        //Si entramos en contacto con el Collider del ataque del enemigo...
        if (col.CompareTag("Enemigo-Attack"))
        {
            //Llamamos a la función de Recibir Daño
            TakeDamage(1f);
        }

    }

}