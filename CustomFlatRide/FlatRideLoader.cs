using System.Collections.Generic;
using UnityEngine;
public class FlatRidesLoader : CustomFlatRideLoader
{

    public void LoadKMG()
    {
        GameObject asset = base.LoadAsset("KMGExperience");
        List<Transform> Arms = new List<Transform>();
        List<Transform> ArmRotators = new List<Transform>();
        List<Transform> Main = new List<Transform>();
        List<Transform> RotateAxis = new List<Transform>();
        Utility.recursiveFindTransformsStartingWith("Arm_", asset.transform, Arms);
        Utility.recursiveFindTransformsStartingWith("ArmRota", asset.transform, ArmRotators);
        Utility.recursiveFindTransformsStartingWith("MainRotator", asset.transform, Main);
        Utility.recursiveFindTransformsStartingWith("RotateAxis", asset.transform, RotateAxis);
        KMGExperience KMGE = asset.AddComponent<KMGExperience>();
        KMGE.armRaiseAxis = Arms;
        KMGE.armSpinAxis = ArmRotators;
        KMGE.armAxis = RotateAxis;
        KMGE.mainAxis = Main[0];
        base.AddColors(asset, new Color[2]{ base.ConvertColor(171, 18, 16), Color.yellow });
        base.AddRestraints(asset, new Vector3(0, 0, 120));
        base.BasicFlatRideSettings(KMGE,"KMG Expirence", 800, .7f, .4f, .7f, 6, 6);
        List<int> connections = new List<int>()
        {
            0,22,
            22,1,
            1,21,
            21,2,
            20,2,
            20,3,
            3,19,
            19,4,
            4,18,
            18,5,
            5,0,
            20,7,
            20,14,
            18,8,
            18,17,
            22,11,
            22,12,
            4,16,
            16,17,
            5,9,
            9,8,
            0,11,
            11,10,
            1,13,
            13,12,
            2,15,
            15,14,
            18,20,
            18,22,
            22,20

        };
        base.SetWaypoints(asset, connections);
    }
    public void LoadFlatRides()
    {
        LoadKMG();
    }

}


