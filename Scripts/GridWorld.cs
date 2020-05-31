using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GridExtensions;

public class GridWorld : MonoBehaviour
{
    [System.NonSerialized]
    public MeshRenderer[] MeshRenders;
    public Vector2[] Centers { get; }// 変更不可
    public Vector2 CentersMin { get; }
    public Vector2 CentersMax { get; }
    public State[] States;


    public float GridSize { get; } // m単位にする可能性があるためfloatが適切
    public int X_Extend { get; }
    public int Y_Extend { get; }
    public float Diagonal { get; }



    public GridWorld(float gridSize, int xExtend, int yExtend, Vector2 gridOrigin)
    {
        this.GridSize = gridSize;
        this.X_Extend = xExtend;
        this.Y_Extend = yExtend;
        this.Diagonal = gridSize * 1.414f;

        int meshCount = xExtend * yExtend;
        MeshRenders = new MeshRenderer[meshCount];
        Centers = new Vector2[meshCount];

        States = new State[meshCount];

        //make each grids mesh
        int idx = 0;
        for (int i = 0; i < yExtend; i++)
        {
            for (int j = 0; j < xExtend; j++)
            {

                Vector2 center = new Vector2((gridSize * j) + (float)(gridSize / 2.0), (gridSize * i) + (float)(gridSize / 2.0));
                center += gridOrigin;
                Centers[idx] = center;
                MeshRenders[idx] = MakeMesh(center, gridSize, idx.ToString()).GetComponent<MeshRenderer>();
                States[idx] = State.Alive;
                idx++;
            }
        }
        CentersMin = Centers.First();
        CentersMax = Centers.Last();
    }


    /// <summary>
    /// もしその方向が壁なら-1を返す
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public int GetRight(int current)
    {
        if (Centers[current].x == CentersMax.x) return -1; //一番右の列

        return current + 1;
    }

    /// <summary>
    /// もしその方向が壁なら-1を返す
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public int GetLeft(int current)
    {
        if (Centers[current].x == CentersMin.x) return -1; //一番左側

        return current - 1;
    }

    /// <summary>
    /// もしその方向が壁なら-1を返す
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public int GetUp(int current)
    {
        if (Centers[current].y == CentersMax.y) return -1; //一番上

        return current + X_Extend;
    }

    /// <summary>
    /// もしその方向が壁なら-1を返す
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public int GetDown(int current)
    {
        if (Centers[current].y == CentersMin.y) return -1; //一番下

        return current - X_Extend;
    }

    public int GetRightUp(int current)
    {
        if (Centers[current].y == CentersMax.y || Centers[current].x == CentersMax.x)
        {
            return -1; //一番上 or 一番右の列
        }

        return current + X_Extend + 1;
    }

    public int GetLeftUp(int current)
    {
        if (Centers[current].y == CentersMax.y || Centers[current].x == CentersMin.x)
        {
            return -1; //一番上 or 一番右の列
        }

        return current + X_Extend - 1;
    }

    public int GetRightDown(int current)
    {
        if (Centers[current].y == CentersMin.y || Centers[current].x == CentersMax.x)
        {
            return -1; //一番sita or 一番右の列
        }

        return current - X_Extend + 1;
    }

    public int GetLeftDown(int current)
    {
        if (Centers[current].y == CentersMin.y || Centers[current].x == CentersMin.x)
        {
            return -1; //一番sita or 一番hidariの列
        }

        return current - X_Extend - 1;
    }

    private GameObject MakeMesh(Vector2 center, float gridSize, string name = "")
    {
        GameObject meshObj = new GameObject("Mesh" + name);
        var mesh = new Mesh();

        mesh.vertices = new Vector3[] {
            new Vector3( center.x - (float)(gridSize /2.0), center.y - (float)(gridSize /2.0), 0),
            new Vector3(center.x - (float)(gridSize / 2.0), center.y + (float)(gridSize / 2.0), 0),
            new Vector3( center.x + (float)(gridSize /2.0), center.y - (float)(gridSize /2.0), 0),
            new Vector3( center.x + (float)(gridSize /2.0), center.y + (float)(gridSize /2.0), 0),
        };

        mesh.triangles = new int[] {
        0, 1, 2,
        1, 3, 2,
         };

        var meshRender = meshObj.AddComponent<MeshRenderer>();
        var meshFilter = meshObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        meshRender.material.color = Color.clear;
        meshRender.material.shader = Shader.Find("UI/Default");

        return meshObj;
    }

    /// <summary>
    /// Set the camera so that it is in the center of grid.
    /// 
    /// </summary>
    public void SetCameraOnCenter(Camera camera,float Z_Offset)
    {
       // Camera camera = Camera.main;
        Vector2 center = (CentersMax - CentersMin) / 2;
        float harfGrid = GridSize / 2.0f;
        camera.gameObject.transform.position = new Vector3(center.x + harfGrid, center.y + harfGrid, Z_Offset);
    }

    public void RefreshDisplay()
    {
        for (int i = 0; i < States.Length; i++)
        {
            if (States[i] == State.Dead)
            {
                MeshRenders[i].material.color = Color.white;
            }
            else
            {
                MeshRenders[i].material.color = Color.black;
            }
        }
    }

    /// <summary>
    ///  Statues をリセットして表示を更新する
    /// </summary>
    public void Reset()
    {
        for (int i = 0; i < States.Length; i++)
        {
            States[i] = State.Alive;
        }
        RefreshDisplay();
    }
}
