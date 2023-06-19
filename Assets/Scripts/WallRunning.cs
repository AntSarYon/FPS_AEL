using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    [SerializeField] private LayerMask capaSuelo;
    [SerializeField] private LayerMask capaEdificios;

    [SerializeField] private float fuerzaWallRun;
    [SerializeField] private float velocidadEscalada;
    [SerializeField] private float fuerzaJumpUp;
    [SerializeField] private float fuerzaJumpSide;

    [SerializeField] private float tiempoMaxEnPared;
    [SerializeField] private float tiempoEnPared;

    [Header("Detección")]
    [SerializeField] private float distanciaDeteccion;
    private float alturaMinima = 0.475f;
    
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    private bool wallLeft;
    private bool wallRight;

    //SALIENDO DE LA PARED
    private bool saliendoDePared;
    [SerializeField] private float tiempoDeSalida;
    private float timerDeSalida;

    [Header("Referencias")]
    [SerializeField] Transform cuerpo;
    private PlayerController player;
    private Rigidbody mRb;

    //-----------------------------------------------------------

    private void Start()
    {
        player = GetComponent<PlayerController>();
        mRb = GetComponent<Rigidbody>();
    }

    //-------------------------------------------------------------

    private void RevisarParedes()
    {
        //Lanzamos Raycasta hacia ambos lados para detectar si hay Paredes cerca
        //En caso de que las haya, almacenamos la información en un Hit.
        wallRight = Physics.Raycast(cuerpo.position, cuerpo.right, out rightWallHit, distanciaDeteccion, capaEdificios);
        wallLeft = Physics.Raycast(cuerpo.position, -cuerpo.right, out leftWallHit, distanciaDeteccion, capaEdificios);

    }

    //--------------------------------------------------------------------------------

    private bool RevisarDistanciaDelSuelo()
    {
        //Mediante un Raycast, detectamos si estamos lo suficientemence por encima del suelo
        //Considerar que, queremos que nos devuelva Falso, si está muy cerca del suelo (hace contacto)
        return !Physics.Raycast(cuerpo.position, Vector3.down, alturaMinima, capaSuelo);
    }

    //-----------------------------------------------------------------------------------

    private void Update()
    {
        RevisarParedes();

        StateMachine();
    }

    //-----------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Si el jugador esta haciendo Wall Running...
        if (player.WallRunning)
        {
            //Aplicamos la Fuyerza de movimiento en Pared
            WallRunningMovement();
        }
    }

    //-----------------------------------------------------------------------------------------

    private void StateMachine()
    {
        //Entrar a ESTADO 1 - WALL RUN

        //Si hay una pared cerca,
        //y nos estamos moviendo hacia adelante,
        //Y estams a una distancia elevada del suelo
        //Y no estamos saliendo (saltando) de la pared
        if ((wallLeft || wallRight) && player.MDirection.y > 0 && RevisarDistanciaDelSuelo() && !saliendoDePared)
        {
            //Si el jugador NO ESTA HACIENDO WALL RUNNING
            if (!player.WallRunning)
            {
                //Iniciamos el Wall Running
                StartWallRun();
            }
            //Si el jugador pulsa espacio
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Hacemos un salto
                WallJump();
            }
        }

        //Entrar a Estado 2 -- Saliendo de Pared (por Salto)
        else if (saliendoDePared)
        {
            if (player.WallRunning)
            {
                StopWallRun();
            }
            if (timerDeSalida > 0)
            {
                timerDeSalida -= Time.deltaTime;
            }
            if (timerDeSalida <= 0)
            {
                saliendoDePared = false;
            }
        }

        //Entrar a Estado 3 -- Normal RUN
        //Caso contrario
        else
        {
            //Si el jugador se encontraba haciendo Wall Running...
            if (player.WallRunning)
            {
                //Detenemos el Wall Run
                StopWallRun();
            }
        }
        
    }

    //--------------------------------------------------------------------------------------------

    private void StartWallRun()
    {
        //Activamos el Flag de Wall Running en el PlayerController
        player.WallRunning = true;
    }

    //------------------------------------------------------------------------

    private void WallRunningMovement()
    {
        //Desactivamos la gravedad
        mRb.useGravity = false;

        //Quitamos toda velocidad en el Eje Y
        mRb.velocity = new Vector3(mRb.velocity.x, 0, mRb.velocity.z);

        //Obtenemos la normal de la Pared con la que entramos en contacto
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        //Obtenemos la dirección en que aplicar fuerza para mantenernos pegados a la pared
        Vector3 wallForward = Vector3.Cross(wallNormal, cuerpo.up);
        
        //Controlamos la dirección de la Fuerza que nos brinda la pared; sino se hace esto,
        //dependiendo de desde donde toquemos la pared, nos botará hacia adelante o atrás
        if ((cuerpo.forward - wallForward).magnitude > (cuerpo.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        //Aplicamos la Fuerza que nos brinda la pared para avanzar pegados a ella
        mRb.AddForce(wallForward * fuerzaWallRun, ForceMode.Force);

        //Fuerza de escalada o descenso
        if (Input.GetKey(KeyCode.LeftShift))
        {
            mRb.velocity = new Vector3(mRb.velocity.x, velocidadEscalada, mRb.velocity.z);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            mRb.velocity = new Vector3(mRb.velocity.x, -velocidadEscalada, mRb.velocity.z);
        }

        //Aplicamos fuerza para mantenernos pegados a la pared
        //Esto es SOLO si el jugador lo demanda oprimiendo las teclas <- o ->

        if (!(wallLeft && player.MDirection.x > 0) && !(wallRight && player.MDirection.x < 0))
        {
            mRb.AddForce(-wallNormal * 20, ForceMode.Force);
        }

    }

    //------------------------------------------------------------------------------------------------

    private void StopWallRun()
    {
        //Desactivamos el Flag de WallRunning del Player
        player.WallRunning = false;

        //Activamos la gravedad nuevamente
        mRb.useGravity = true;
    }

    //-----------------------------------------------------------------------------------------

    private void WallJump()
    {
        //Entramos al estado de SALIENDO DE PARED
        saliendoDePared = true;
        timerDeSalida = tiempoDeSalida;

        //Obtenemos la normal de la Pared con la que entramos en contacto
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        //Calculamos la fuerza a aplicar
        Vector3 fuerzaAAplicar = (transform.up * fuerzaJumpUp) + (wallNormal * fuerzaJumpSide);

        //Reiniciamos la velocidad vertical a 0
        mRb.velocity = new Vector3(mRb.velocity.x, 0f, mRb.velocity.z);

        //Añadimos fuerza como un impulso
        mRb.AddForce(fuerzaAAplicar,ForceMode.Impulse);
    }
}
