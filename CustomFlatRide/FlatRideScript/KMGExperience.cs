using System.Collections.Generic;
using UnityEngine;

public class KMGExperience : FlatRide
{
    private new enum State
    {
        STARTING,
        RUNNING,
        LOWERING,
        STOPPING
    }

    public Transform mainAxis;

    public List<Transform> armRaiseAxis;

    public List<Transform> armSpinAxis;

    public List<Transform> armAxis;
    public int spins = 3;

    public FabricEvent raiseArmSound;

    public FabricEvent lowerArmSound;

    private Quaternion armTargetRotation = Quaternion.Euler(30f, 0f, 0f);

    private int accelerationSpeed = 22;

    private float maxSpeed = 60f;

    private float armSpinAxisMaxSpeed = 120f;

    [Serialized]
    private KMGExperience.State currentState;

    [Serialized]
    private Rotator mainRotator = new Rotator();

    [Serialized]
    private Rotator armSpinRotator = new Rotator();

    [Serialized]
    private RotateBetween armRaise = new RotateBetween();

    public override void Start()
    {
        base.Start();
        this.armRaise.Initialize(this.armRaiseAxis[0], Quaternion.identity, this.armTargetRotation, 4f);
        this.mainRotator.Initialize(this.mainAxis, (float)this.accelerationSpeed, this.maxSpeed);
        this.armSpinRotator.Initialize(this.armSpinAxis[0], (float)this.accelerationSpeed, this.armSpinAxisMaxSpeed);
        this.armSpinRotator.setDirection(-1);
    }

    public override void onStartRide()
    {
        base.onStartRide();
        this.currentState = KMGExperience.State.STARTING;
        this.mainRotator.start();
    }

    public override void tick(StationController stationController)
    {
        if (stationController != null && stationController.getState() != StationController.State.ACTIVE)
        {
            return;
        }
        this.armRaise.tick(Time.deltaTime);
        this.mainRotator.tick(Time.deltaTime);
        this.armSpinRotator.tick(Time.deltaTime);
        if (this.currentState != KMGExperience.State.LOWERING && this.currentState != KMGExperience.State.STOPPING)
        {
            if (this.mainRotator.getCurrentSpeed() > this.mainRotator.getMaxSpeed() * 0.8f && this.armRaise.startFromTo())
            {
                Fabric.EventManager.Instance.PostEvent(this.raiseArmSound.name, base.gameObject);
            }
        }
        else if (this.armSpinRotator.getCurrentSpeed() < this.armSpinRotator.getMaxSpeed() * 0.2f && this.armRaise.startToFrom())
        {
            Fabric.EventManager.Instance.PostEvent(this.lowerArmSound.name, base.gameObject);
        }
        if (this.currentState == KMGExperience.State.STARTING)
        {
            if (this.mainRotator.reachedFullSpeed())
            {
                this.armSpinRotator.start();
                this.currentState = KMGExperience.State.RUNNING;
                base.triggerRunloopSound();
            }
        }
        else if (this.currentState == KMGExperience.State.RUNNING)
        {
            if (this.armSpinRotator.getCompletedRotationsCount() >= this.spins)
            {
                this.armSpinRotator.stop();
                this.currentState = KMGExperience.State.LOWERING;
            }
        }
        else if (this.currentState == KMGExperience.State.LOWERING && this.armSpinRotator.isStopped())
        {
            this.currentState = KMGExperience.State.STOPPING;
            this.mainRotator.stop();
            base.triggerDecelerateSound();
        }
        foreach(Transform T in armRaiseAxis)
        {
            T.localRotation = armRaiseAxis[0].localRotation;
        }
        foreach(Transform T in armSpinAxis)
        {
            T.localRotation = armSpinAxis[0].localRotation;
        }
        foreach (Transform T in armAxis)
        {
            T.localRotation = armSpinAxis[0].localRotation;
        }
    }

    public override bool shouldLetGuestsOut()
    {
        return this.currentState == KMGExperience.State.STOPPING && this.mainRotator.isStopped();
    }
}
