﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerAcademy : Academy
{
    public override void InitializeAcademy()
    {
        Monitor.verticalOffset = 1f;
        Physics.defaultSolverIterations = 12;
        Physics.defaultSolverVelocityIterations = 12;
        // Physics.gravity *= 1.5f;
    }

    public override void AcademyReset()
    {


    }

    public override void AcademyStep()
    {


    }
}