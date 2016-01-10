﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;

public class Connection
{
    public int value1;
    public int value2;
}

public class CustomFlatRideLoader : MonoBehaviour
{

    private List<BuildableObject> _sceneryObjects = new List<BuildableObject>();

    public string Path;
    public List<Connection> Connections = new List<Connection> { };
    public string Identifier;
    public FlatRide FlatRideComponent;


    public GameObject LoadAsset(string PrefabName)
    {
        try
        {

            GameObject hider = new GameObject();

            char dsc = System.IO.Path.DirectorySeparatorChar;

            using (WWW www = new WWW("file://" + Path + dsc + "assetbundle" + dsc + "flatrides"))
            {
                if (www.error != null)
                    throw new Exception("Loading had an error:" + www.error);

                AssetBundle bundle = www.assetBundle;


                try
                {
                    GameObject asset = Instantiate(bundle.LoadAsset(PrefabName)) as GameObject;
                    bundle.Unload(false);

                    hider.SetActive(false);
                    return asset;

                }

                catch (Exception e)
                {
                    Debug.Log(e);

                    LogException(e);
                    bundle.Unload(false);
                    hider.SetActive(false);
                    return null;
                }



            }


        }
        catch (Exception e)
        {
            LogException(e);
            return null;
        }

    }
    public void SetWaypoints(GameObject asset, List<int> connections)
    {

        Waypoints points = asset.GetComponent<Waypoints>();
        foreach (Transform T in asset.transform.FindChild("WayPoints").transform)
        {
            Debug.Log("Waypoint name : " + T.name);
            Waypoint p = new Waypoint();

            if (T.name.StartsWith("Outer"))
            {
                p.isOuter = true;
            }
            else if (T.name.StartsWith("Point"))
            {
                p.isOuter = false;
            }
            else
            { return; }

            p.localPosition = T.localPosition;
            points.waypoints.Add(p);
            int currentIndex = points.waypoints.IndexOf(p);
            for (int i = 0; i < connections.Count; i += 2) // Loop with for.
            {
                if (connections[i] == currentIndex)
                {
                    points.waypoints[currentIndex].connectedTo.Add(connections[i + 1]);
                }
            }
            for (int i = 1; i < connections.Count; i += 2) // Loop with for.
            {
                if (connections[i] == currentIndex)
                {
                    points.waypoints[currentIndex].connectedTo.Add(connections[i - 1]);
                }
            }
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = p.getWorldPosition(asset.transform);
            sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
    public void BasicFlatRideSettings(FlatRide FlatRideScript, string DisplayName, float price, float excitement, float intensity, float nausea, int x, int Z)
    {
        _sceneryObjects.Add(FlatRideScript);
        AssetManager.Instance.registerObject(FlatRideScript);
        FlatRideScript.fenceGO = AssetManager.Instance.rideFenceGO;
        FlatRideScript.entranceGO = AssetManager.Instance.rideEntranceGO;
        FlatRideScript.exitGO = AssetManager.Instance.rideExitGO;
        FlatRideScript.entranceExitBuilderGO = AssetManager.Instance.flatRideEntranceExitBuilderGO;
        FlatRideScript.price = price;
        FlatRideScript.excitementRating = excitement;
        FlatRideScript.intensityRating = intensity;
        FlatRideScript.nauseaRating = nausea;
        FlatRideScript.setDisplayName(DisplayName);
        FlatRideScript.xSize = x;
        FlatRideScript.zSize = Z;

    }
    public void AddRestraints(GameObject asset, Vector3 closeAngle)
    {
        RestraintRotationController controller = asset.AddComponent<RestraintRotationController>();
        controller.closedAngles = closeAngle;
    }
    public void AddColors(GameObject asset, Color[] C)
    {
        CustomColors cc = asset.AddComponent<CustomColors>();
        cc.setColors(C);
       
        foreach (Material material in AssetManager.Instance.objectMaterials)
        {
            if (material.name == "CustomColorsSpecular")
            {
                asset.GetComponentInChildren<Renderer>().sharedMaterial = material;

                // Go through all child objects and recolor		
                Renderer[] renderCollection;
                renderCollection = asset.GetComponentsInChildren<Renderer>();

                foreach (Renderer render in renderCollection)
                {
                    render.sharedMaterial = material;
                }

                break;
            }
        }
    }

    private void LogException(Exception e)
    {
        StreamWriter sw = File.AppendText(Path + @"/mod.log");

        sw.WriteLine(e);

        sw.Flush();

        sw.Close();
    }

    public void UnloadScenery()
    {
        foreach (BuildableObject deco in _sceneryObjects)
        {
            AssetManager.Instance.unregisterObject(deco);
            DestroyImmediate(deco.gameObject);
        }
    }
    public Color ConvertColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}

