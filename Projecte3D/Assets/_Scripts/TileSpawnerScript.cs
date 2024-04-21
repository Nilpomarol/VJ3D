using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

namespace TempleRun {
public class tileSpawner : MonoBehaviour
{

    
    [SerializeField]
    private int tileStartCount = 10; //tiles sense obstacles quan comences
    [SerializeField]
    private int minStraightTiles = 3; //minim de tiles rectes des de l'últim gir
    [SerializeField]
    private int maxStraightTiles = 15; //maxim de tiles rectes
    [SerializeField]
    private GameObject startingTile; //tile on es comença
    [SerializeField]
    private List<GameObject> turnTiles; //tiles de gir
    [SerializeField]
    private List<GameObject> obstacles; //obstacles
    [SerializeField]
    private float obstacleProbability  = 0.2f; //provabilitat d'obstacle
    
    private Vector3 currentTileLocation = Vector3.zero;
    private Vector3 currentTileDirection = Vector3.forward;
    private GameObject prevTile;

    private List<GameObject> currentTiles; //tiles mostrades actualment a la escena
    private List<GameObject> currentObstacles; //obstacles actuals a la escena

    // Start is called before the first frame update
    void Start()
    {
        currentTiles = new List<GameObject>();
        currentObstacles = new List<GameObject>();

        Random.InitState(System.DateTime.Now.Millisecond); //Random seed per fer random de veritat

        //spawn de les primeres tiles rectes i sense obstacles
        for (int i = 0; i < tileStartCount; i++) {
            SpawnTile(startingTile.GetComponent<Tile>(), false);
        }

        SpawnTile(SelectRandoGameObject(turnTiles).GetComponent<Tile>()); //spawn random turn tile
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void SpawnTile(Tile tile, bool spawnObstacle = false){
        Quaternion newTileRotation = tile.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up); //direccio en la qual el jugador s'esta movent

        prevTile = GameObject.Instantiate(tile.gameObject, currentTileLocation, newTileRotation); //instancia un tile nou
        
        currentTiles.Add(prevTile); //l'afageix a la llista de tiles que hi ha actualment

        if(spawnObstacle) SpawnObstacle(); //spawnega obstacles si aixi s'indica

        if (tile.type == TileType.STRAIGHT) currentTileLocation += Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size, currentTileDirection); //offset per a la seguent tile perque es crei en el lloc correcte
    }


    //esborra les tiles que ja no es veuen
    private void DeletePreviousTiles() {
        while (currentTiles.Count != 1) {
            GameObject tile = currentTiles[0];
            currentTiles.RemoveAt(0);
            Destroy(tile);

        }

        while (currentObstacles.Count != 0) {
            GameObject obstacle = currentObstacles[0];
            currentObstacles.RemoveAt(0);
            Destroy(obstacle);

        }
    }

    //canvia la direccio en la que el jugador va
    public void AddNewDirection(Vector3 direction) {
        currentTileDirection = direction;
        DeletePreviousTiles();

        //determina la posicio de la seguent tile en funcio del gir que es faci
        Vector3 tilePlacementScale;
        if(prevTile.GetComponent<Tile>().type == TileType.SIDEWAYS) {
            tilePlacementScale = Vector3.Scale(prevTile.GetComponent<Renderer>().bounds.size / 2 + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        }
        else {
            tilePlacementScale = Vector3.Scale((prevTile.GetComponent<Renderer>().bounds.size - (Vector3.one * 2)) + (Vector3.one * startingTile.GetComponent<BoxCollider>().size.z / 2), currentTileDirection);
        }

        currentTileLocation += tilePlacementScale;

        //es generen un numero random de tiles rectes 
        int currentPathLength = Random.Range(minStraightTiles, maxStraightTiles);
        for (int i = 0; i < currentPathLength; i++) {
            SpawnTile(startingTile.GetComponent<Tile>(), (i == 0) ? false : true);
        }

        //seguent turn tile
        SpawnTile(SelectRandoGameObject(turnTiles).GetComponent<Tile>(), false);

    }

    //funcio per spawnejar un obstacle, funciona similar a la detile
    private void SpawnObstacle() {
        if (Random.value > obstacleProbability) return; //provabilitat de spawnejar un obstacle

        GameObject obstaclePrefab = SelectRandoGameObject(obstacles);
        Quaternion newObjectRotation = obstaclePrefab.gameObject.transform.rotation * Quaternion.LookRotation(currentTileDirection, Vector3.up);
        GameObject obstacle = Instantiate(obstaclePrefab, currentTileLocation, newObjectRotation);

        currentObstacles.Add(obstacle);
    }

    private GameObject SelectRandoGameObject(List<GameObject> list){
        if (list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
}