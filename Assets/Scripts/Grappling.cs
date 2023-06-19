using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("Referencias")]
    private PlayerController player;
    [SerializeField] private Transform camara;
    [SerializeField] private Transform inicioGancho;
    [SerializeField] private LayerMask capaAgarre;
    [SerializeField] private LineRenderer lineaAgarre;

    [SerializeField] private AudioSource ganchoAudioSource;
    [SerializeField] private AudioClip clipGanchoFallido;
    [SerializeField] private AudioClip clipGanchoExitoso;

    [Header("Agarre")]
    [SerializeField] private float maximaDistancia;
    [SerializeField] private float tiempoDelay;

    [SerializeField] private float movimientoSobreDisparo;

    private Vector3 puntoAgarre;
    private bool enganchado;

    [Header("Enfriamiento")]
    [SerializeField] private float tiempoEnfriamiento;
    private float timerEnfriamiento;

    //----------------------------------------------------------------------

    void Start()
    {
        player = GetComponent<PlayerController>();
    }

    //------------------------------------------------------------------------
    
    void Update()
    {
        //Si se oprime el click Derecho
        if (Input.GetMouseButtonDown(1))
        {
            //Iniciamos el Agarre
            StartGrapple();
        }

        //Si el Timer esta corriendo
        if (timerEnfriamiento > 0)
        {
            //Lo reducimos cada Frame
            timerEnfriamiento -= Time.deltaTime;
        }
    }

    //-----------------------------------------------------------------------------
    private void LateUpdate()
    {
        //Si el Flag de enganchado está activo...
        if (enganchado)
        {
            //Actualizamos la coordenada desde donde inicia el Cable
            lineaAgarre.SetPosition(0,inicioGancho.position);
        }
    }

    //--------------------------------------------------------------------------

    public void StartGrapple()
    {
        //Si el Gancho aun esta en enfriamiento...
        if (timerEnfriamiento > 0) return; //retornamos
        
        //Sino...

        //Activamos el Flag de Enganchado
        enganchado = true;

        //Activamo el Flag de Freeze en el Player
        player.Freeze = true;

        //Creamos estructura de Datos para el punto de agarre
        RaycastHit hit;

        //Si estabamos apuntando a un objeto apto para engancharnos
        if (Physics.Raycast(camara.position, camara.forward, out hit, maximaDistancia, capaAgarre))
        {
            //Asignamos el Punto de Agarre
            puntoAgarre = hit.point;
            ganchoAudioSource.PlayOneShot(clipGanchoExitoso, 0.75f);
            //Invocamos a la funcion de ejcutarAgarre, con un poco de Delay
            Invoke(nameof(ExecuteGrapple), tiempoDelay);
        }

        //Si no le apuntamos a un objeto apto para el enganche
        else
        {
            puntoAgarre = camara.position + camara.forward * maximaDistancia;
            ganchoAudioSource.PlayOneShot(clipGanchoFallido, 0.75f);
            //Invocamos a la funcion de ejcutarAgarre, con un poco de Delay
            Invoke(nameof(StopGrapple), tiempoDelay);
        }

        //Activamos la linea de agarre
        lineaAgarre.enabled = true;

        //Seteamos el Punto de Agarre como coordenada final
        lineaAgarre.SetPosition(1,puntoAgarre);

    }

    //--------------------------------------------------------------

    public void ExecuteGrapple()
    {
        //Desactivamos el Flag de Freeze en el Player
        player.Freeze = false;

        //Hacemos un efecto de profundidad en la camara
        camara.GetComponent<CameraMovement>().DoFov(90f);

        //Calculamos cuál sería la ruta para tomar, considerando las tantas opciones posibles

        Vector3 puntoMasBajo = new Vector3(
            player.Cuerpo.position.x,
            player.Cuerpo.position.y - 1f,
            player.Cuerpo.position.z);

        float puntoDeAgarreRelativoAPosY = puntoAgarre.y - puntoMasBajo.y;
        float puntoMasAltoDelArco = puntoDeAgarreRelativoAPosY + movimientoSobreDisparo;

        if (puntoDeAgarreRelativoAPosY < 0) puntoMasAltoDelArco = movimientoSobreDisparo;

        player.SaltoConGanchoHaciaPosicion(puntoAgarre, puntoMasAltoDelArco);

        Invoke(nameof(StopGrapple), 1f);
    }

    //--------------------------------------------------------------

    public void StopGrapple()
    {
        //Desactivamos el Flag de Freeze en el Player
        player.Freeze = false;

        //Devolvemos la profundidad de la camara a la normal
        camara.GetComponent<CameraMovement>().DoFov(60f);

        //Desactivamos el Flag de Enganchado
        enganchado = false;

        //Reseteamos el tiempo de enfriamiento
        timerEnfriamiento = tiempoEnfriamiento;

        //Desactivamos el cable
        lineaAgarre.enabled = false;
    }
}
