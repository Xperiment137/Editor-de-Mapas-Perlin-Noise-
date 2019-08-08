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
    public Texture textura;
    public Texture textura1;
    private Texture[] texturas;
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

    //https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/

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
                textures[1] = 1.0f - Mathf.Clamp01(gradiente / (terrain.terrainData.heightmapHeight *gradiente)-height);//flat
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
}
