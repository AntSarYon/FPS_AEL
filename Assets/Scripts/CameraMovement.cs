using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    //Definimos variables para controlar la rotación máxima y mínima permitida
    public float MaxRot = 20f;
    public float MinRot = -20f;
    
    //---------------------------------------------------------------------------
    //Función para Rotar la cámara en Vertical (Recibe un Angulo Float)

    public void RotateUpDown(float angle)
    {
        //Definimos una nueva rotación que será la misma, escepto por el Eje X...
        Vector3 newRotation = new Vector3(
            //Para el eje X, le añadimos (o restamos) los angulos recibidos
            transform.rotation.eulerAngles.x + angle,
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z
        );

        //Para evitar BUGS de rotación no deseados, controlamos el valor límites de los Angulos Euler
        newRotation.x = newRotation.x > 180f ? newRotation.x - 360f : newRotation.x;
        
        //Limitamos la rotación entre el mínimo y máximo permitido
        newRotation.x = Mathf.Clamp(newRotation.x, MinRot, MaxRot);

        //Actualizamos la rotación del Transform de la cámara
        transform.rotation = Quaternion.Euler(newRotation);

        print(transform.rotation.eulerAngles);

    }
}
