using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;
using System.Linq;
public class EditorDeMapas : MonoBehaviour
{
    public Terrain aux;
    public float scale = 1.0F;
    public float escala = 10F;
    public float reducir = 2f;
    public float arboles = 10F;
    public float control = 2f;
    private float[,] altura;

    private void Awake()
    {
        aux = GetComponent<Terrain>();
        altura = new float[aux.terrainData.heightmapWidth, aux.terrainData.heightmapHeight];


    }

    void Start()
    {

        Elevar(aux, escala);
        Pintar(aux);
        GenerarArboles(aux);

    }



    void Update()
    {


    }



    public void Elevar(Terrain terrain, float escala)
    {


        for (int i = 0; i < terrain.terrainData.heightmapWidth; i++)
        {
            for (int j = 0; j < terrain.terrainData.heightmapHeight; j++)
            {
                altura[i, j] = Mathf.PerlinNoise(((float)i / (float)terrain.terrainData.heightmapWidth) * escala, ((float)j / (float)terrain.terrainData.heightmapHeight) * escala) / reducir;
            }
        }

        terrain.terrainData.SetHeights(0, 0, altura);

    }



    public void Pintar(Terrain terrain)
    {

        float[,,] MapaTexturas = new float[aux.terrainData.alphamapWidth, aux.terrainData.alphamapHeight, aux.terrainData.alphamapLayers];
        for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
            {

                float y_01 = (float)y / (float)terrain.terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrain.terrainData.alphamapWidth;


                float height = terrain.terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrain.terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrain.terrainData.heightmapWidth));
                Vector3 normal = terrain.terrainData.GetInterpolatedNormal(y_01, x_01);
                float gradiente = terrain.terrainData.GetSteepness(y_01, x_01);
                float[] textures = new float[terrain.terrainData.alphamapLayers];
                textures[0] = Mathf.Clamp01((terrain.terrainData.heightmapHeight));//low
                textures[1] = 1.0f - Mathf.Clamp01(gradiente / (terrain.terrainData.heightmapHeight * gradiente) - height);//flat
                textures[2] = (gradiente * Mathf.Clamp01(normal.z));//high
                float z = textures.Sum();


                for (int i = 0; i < terrain.terrainData.alphamapLayers; i++)
                {


                    textures[i] /= z;


                    MapaTexturas[x, y, i] = textures[i];
                }
            }
        }


        terrain.terrainData.SetAlphamaps(0, 0, MapaTexturas);




    }
    private void GenerarArboles(Terrain terrain)
    {
      
        terrain.terrainData.RefreshPrototypes();
        for (int i = 0; i < terrain.terrainData.alphamapHeight; i++)
        {
            for (int j = 0; j < terrain.terrainData.alphamapWidth; j++)
            {

                float y = (float)i / (float)terrain.terrainData.alphamapHeight;
                float x = (float)j / (float)terrain.terrainData.alphamapWidth;

                float inclinación = terrain.terrainData.GetSteepness(x, y);
                float angulo = inclinación / 90.0f;
                if (angulo < 0.5f)
                {
                    float height = terrain.terrainData.GetHeight(Mathf.RoundToInt(y * terrain.terrainData.heightmapHeight), Mathf.RoundToInt(x * terrain.terrainData.heightmapWidth));
                    float gradiente = terrain.terrainData.GetSteepness(y, x);
                    Vector3 normal = terrain.terrainData.GetInterpolatedNormal(y, x);
                    float ruido = (height * Mathf.Clamp01(normal.z) - gradiente);//high
                    float altura = terrain.terrainData.GetHeight(i, j);
                    if (ruido < 0.0f && altura < 1)
                    {
                        
                        TreeInstance arbol = new TreeInstance();
                        arbol.position = new Vector3(x, altura, y);
                        arbol.prototypeIndex = 0;
                        arbol.widthScale = 1;
                        arbol.heightScale = 1;
                        arbol.color = Color.white;
                        arbol.lightmapColor = Color.white;
                       terrain.AddTreeInstance(arbol);
                    }
                }
            }
        }
    }
}

