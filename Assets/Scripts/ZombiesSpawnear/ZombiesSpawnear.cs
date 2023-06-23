using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombiesSpawnear : MonoBehaviour
{
    [SerializeField] private List<GameObject> listaPrefabsZombies;
    [SerializeField] private List<Transform> listaCoordenadasSpawn;

    private int indiceCoordenada;

    private float timer;
    private float valorBaseTimer;

    private int cantidadASpawnear;

    //-------------------------------------------------------------------------------------

    void Start()
    {
        //Inicializamos el Timer a 60
        timer = 60;
        valorBaseTimer = 60;

        //El indice de coordenada inicia con la ultima coordenada de Spawn
        indiceCoordenada = listaCoordenadasSpawn.Count-1;

        //La cantidad inicia de Enemigos a spawnear sera la indicada en el GameManager
        cantidadASpawnear = GameManager.Instance.cantEnemigosIniciales;

        SpawnearZombies();


    }

    // Update is called once per frame
    void Update()
    {
        ControlarIndiceDeCoordenada();

        //Reducimos el Timer constantemente
        timer -= Time.deltaTime;

        //Si el timer llega a 0
        if (timer<=0)
        {
            SpawnearZombies();

            //Si el valorBase del Timer aun era mayor a 10
            if (valorBaseTimer > 10)
            {
                //Reducimos el BaseTimer en 5
                valorBaseTimer -= 5;

                //reiniciamos el tiempo, asignandole el nuevo TimerBase 
                timer = valorBaseTimer;
            }
        }
            
    }

    private void ControlarIndiceDeCoordenada()
    {
        //Si el indice de coordenada llegó a 0
        if (indiceCoordenada == 0)
        {
            //Lo regresamos al original
            indiceCoordenada = listaCoordenadasSpawn.Count - 1;
        }
    }

    private void SpawnearZombies()
    {
        //Por cada uno de los Zombies que se deben spawnear
        for (int i = 1; i <= cantidadASpawnear; i++)
        {
            GameObject.Instantiate(
                listaPrefabsZombies[Random.Range(0, 4)],
                listaCoordenadasSpawn[indiceCoordenada].position,
                Quaternion.identity
                );

            //Disminuimos el indice de coordenadas (pasaremos a la siguiente)
            indiceCoordenada--;

            ControlarIndiceDeCoordenada();
        }
    }
}
