using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{

    //Distancias de deteccion y ataque
    private float AwakeRadio = 10f;
    private float AttackRadio = 0.65f;

    //Salud
    public float Health = 5f;

    //Referencia a HitBoxes
    public GameObject hitboxRight;
    public GameObject hitboxLeft;

    //Referencia a componentes
    private Animator mAnimator;
    private Rigidbody mRb;

    //Referencia al NavMeshAgent para controlar la zona de movimiento
    private NavMeshAgent navMeshAgent;

    private Collider playerDetectionCollider;
    private Collider playerAttackCollider;

    //Flag de Ataque en curso -> Inicializado en Falso
    private bool mIsAttacking = false;

    //Tendremos una Maquina de Estados Finita (FSM)
    private FSM<ZombieController> mFSM;

    public Collider PlayerDetectionCollider { get => playerDetectionCollider; set => playerDetectionCollider = value; }
    public Collider PlayerAttackCollider { get => playerAttackCollider; set => playerAttackCollider = value; }
    public bool MIsAttacking { get => mIsAttacking; set => mIsAttacking = value; }
    public Animator MAnimator { get => mAnimator; set => mAnimator = value; }
    public Rigidbody MRb { get => mRb; set => mRb = value; }
    public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }

    //------------------------------------------------------------------

    void Start()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
        mAnimator = transform.GetComponentInChildren<Animator>(false); //<-- Le decimos que considere a los GO hijos desactivados
        navMeshAgent = GetComponent<NavMeshAgent>();

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //Creamos un FSM indicando este Script como principal componente
        mFSM = new FSM<ZombieController>(

            //El Estado inicial ser�...
            new Zombie.ZombieIdleState(this)
            );

        // Activamos la m�quina de estados
        mFSM.Begin();
    }

    //---------------------------------------------------------------------------

    void Update()
    {
        //Detectamos el collider del Player dentro de la zona de Ataque
        playerAttackCollider = IsPlayerInAttackArea();

        // - - - - - - - - - - - - - - - - - - - - - -

        //Detectamos el collider del Player dentro de la zona de Detecci�n
        playerDetectionCollider = IsPlayerNearby();
    }

    //---------------------------------------------------------------------
    private void FixedUpdate()
    {
        //Le pasamos el FixedDeltaTime para no Afectar el rendimiento
        mFSM.Tick(Time.fixedDeltaTime);
    }

    //---------------------------------------------------------------------------------------
    //Funci�n para detectar si el jugador est� cerca
    private Collider IsPlayerNearby()
    {
        //Creamos una esfera alrededor del Enemigo para detectar colliders con el LAYER "Player"
        var colliders = Physics.OverlapSphere(
            transform.position,
            AwakeRadio, //Radio de Detecci�n
            LayerMask.GetMask("Player")
        );
        //Si detectamos un collider de Jugador en el �rea; lo retornamos
        if (colliders.Length == 1) return colliders[0];

        //Caso contrario, retornamos un Nulo
        else return null;
    }

    //------------------------------------------------------------------------------------------
    //Funci�n para detectar si el jugador est� en la zona de Ataque

    private Collider IsPlayerInAttackArea()
    {
        //Creamos una esfera peque�a alrededor del Enemigo para detectar colliders con el LAYER "Player"
        var colliders = Physics.OverlapSphere(
            transform.position,
            AttackRadio, //radio de ataque
            LayerMask.GetMask("Player")
        );
        //Si detectamos un collider de Jugador en la zona de ataque; lo retornamos
        if (colliders.Length == 1) return colliders[0];

        //Caso contrario, retornamos nulo
        else return null;
    }

    //-----------------------------------------------------------------------------------------
    //Funci�n para Iniciar el Ataque
    public void StartAtack()
    {
        //Activamos el FLAG de ATACANDO
        mIsAttacking = true;
    }

    //----------------------------------------------------------------------
    //Funci�n para habilitar los objetos con los HitBoxes
    public void EnableHitbox()
    {
        //Activamos los Hitboxes
        hitboxLeft.SetActive(true);
        hitboxRight.SetActive(true);
    }

    //---------------------------------------------------------------------------------
    //Funci�n para DETENER EL ATAQUE
    public void StopAttack()
    {
        //Desactivamos el Flag de ATACANDO
        mIsAttacking = false;

        //Desactivamos los Objetos con HitBox
        hitboxLeft.SetActive(false);
        hitboxRight.SetActive(false);
    }

    //---------------------------------------------------------------------------------------------
    //Funci�n para el Recibimiento de DA�O

    public void TakeDamage(float damage)
    {
        //Reducimos a la Salud el da�o recibido por parte del jugador
        Health -= damage;
    }
}
