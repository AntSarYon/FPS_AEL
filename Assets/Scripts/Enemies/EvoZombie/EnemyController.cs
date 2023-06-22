using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    //Velocidad de movimiento
    public float Speed = 2f;

    //Distancias de deteccion y ataque
    public float AwakeRadio = 2f;
    public float AttackRadio = .5f;

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

    //Vector de direcci�n para el movimiento
    private Vector2 mDirection; // XZ

    //Flag de Ataque en curso -> Inicializado en Falso
    private bool mIsAttacking = false;

    //Tendremos una Maquina de Estados Finita (FSM)
    private FSM<EnemyController> mFSM;

    //-----------------------------------------------------------------------

    private void Start()
    {
        //Obtenemos referencia a componentes
        mRb = GetComponent<Rigidbody>();
        mAnimator = transform.GetComponentInChildren<Animator>(false); //<-- Le decimos que considere a los GO hijos desactivados
        navMeshAgent = GetComponent<NavMeshAgent>();

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //Creamos un FSM indicando este Script como principal componente
        mFSM = new FSM<EnemyController>(

            //El Estado inicial ser�...
            new Enemy.EnemyIdleState(this)
            );

        // Activamos la m�quina de estados
        mFSM.Begin();
    }

    //-------------------------------------------------------------------------

    private void Update()
    {
        //Detectamos el collider del Player dentro de la zona de Ataque
        var collider1 = IsPlayerInAttackArea();

        //Si hay un collider en la zona de ataque, y NO ESTAMOS ATACANDO
        if (collider1 != null && !mIsAttacking)
        {
            //Nos detenemos asignando la velocididad en 0
            mRb.velocity = new Vector3(
                0f,
                0f,
                0f
            );

            //Detenemos al NavMeshAgent
            navMeshAgent.isStopped = true;

            //Desactivamos el Flag de Animacion para Caminar
            mAnimator.SetBool("IsWalking", false);
            //Disparamos el Trigger de Ataque
            mAnimator.SetTrigger("Attack");

            //Volvemos
            return;
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - 

        //Detectamos el collider del Player dentro de la zona de Detecci�n
        var collider2 = IsPlayerNearby();

        //Si hay un collider en la zona de Detecci�n, y NO ESTAMOS ATACANDO
        if (collider2 != null && !mIsAttacking)
        {
            //Activamose l Flag de Animaci�n para Caminar
            mAnimator.SetBool("IsWalking", true);

            //Indicamos que el NavMeshAgent est� en movimiento
            navMeshAgent.isStopped = false;

            //Asignamos que el destino del NavMeshAgent sea la pisici�n del Jugador
            navMeshAgent.SetDestination(collider2.transform.position);
        }

        //En caso de que no haya jugador cercano
        else 
        {
            // NOS DETENEMOS

            //Asignamos velocidad a 0
            mRb.velocity = Vector3.zero;
            
            //Desactivamos el Flag de animacion para el movimiento
            mAnimator.SetBool("IsWalking",false);

            //Detenemos al NavMeshAgent
            navMeshAgent.isStopped = true;

            //Reiniciamos la Ruta del NavMesh
            navMeshAgent.ResetPath();
        }
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
        //Si la salud baja, o iguala a 0
        if (Health <= 0f)
        { 
            //Matamos (Destruimos) el GameObject enemigo
            Destroy(gameObject);
        }
    }
}
