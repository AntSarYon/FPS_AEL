using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrawlerController : MonoBehaviour
{
    //Distancias de deteccion y ataque
    private float AwakeRadio = 3.5f;
    //Salud
    public float Health = 3f;

    //Referencia a HitBoxes
    public GameObject hitboxRight;
    public GameObject hitboxLeft;

    //Referencia a componentes
    [SerializeField] private Animator mAnimator;
    private Rigidbody mRb;

    //Referencia al NavMeshAgent para controlar la zona de movimiento
    private NavMeshAgent navMeshAgent;

    private Collider playerCollider;

    //Tendremos una Maquina de Estados Finita (FSM)
    private FSM<CrawlerController> mFSM;

    public Animator MAnimator { get => mAnimator; set => mAnimator = value; }
    public Rigidbody MRb { get => mRb; set => mRb = value; }
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }
    public Collider PlayerCollider { get => playerCollider; set => playerCollider = value; }

    //------------------------------------------------------------------------

    void Start()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
        //mAnimator = transform.GetComponentInChildren<Animator>(false); //<-- Le decimos que considere a los GO hijos desactivados
        navMeshAgent = GetComponent<NavMeshAgent>();

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //Creamos un FSM indicando este Script como principal componente
        mFSM = new FSM<CrawlerController>(

            //El Estado inicial será...
            new Crawler.CrawlerIdleState(this)
            );

        // Activamos la máquina de estados
        mFSM.Begin();
    }

    //---------------------------------------------------------------------------

    void Update()
    {
        //Detectamos el collider del Player dentro de la zona de Detección
        playerCollider = IsPlayerNearby();
    }

    //----------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Le pasamos el FixedDeltaTime para no Afectar el rendimiento
        mFSM.Tick(Time.fixedDeltaTime);
    }

    //---------------------------------------------------------------------------------------
    //Función para detectar si el jugador está cerca
    private Collider IsPlayerNearby()
    {
        //Creamos una esfera alrededor del Enemigo para detectar colliders con el LAYER "Player"
        var colliders = Physics.OverlapSphere(
            transform.position,
            AwakeRadio, //Radio de Detección
            LayerMask.GetMask("Player")
        );
        //Si detectamos un collider de Jugador en el área; lo retornamos
        if (colliders.Length == 1)
        {
            return colliders[0];
        }

        //Caso contrario, retornamos un Nulo
        else
        {
            return null;
        }

    }

    //----------------------------------------------------------------------
    //Función para habilitar los objetos con los HitBoxes
    public void EnableHitbox()
    {
        //Activamos los Hitboxes
        hitboxLeft.SetActive(true);
        hitboxRight.SetActive(true);
    }

    //----------------------------------------------------------------------
    //Función para habilitar los objetos con los HitBoxes
    public void DisableHitbox()
    {
        //Activamos los Hitboxes
        hitboxLeft.SetActive(false);
        hitboxRight.SetActive(false);
    }

    //---------------------------------------------------------------------------------------------
    //Función para el Recibimiento de DAÑO
    public void TakeDamage(float damage)
    {
        //Reducimos a la Salud el daño recibido por parte del jugador
        Health -= damage;
    }
}
