using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimulationManager : MonoBehaviour
{
    public UnityEvent simulationReadyEvent = new UnityEvent();
    public UnityEvent simulationBusyEvent = new UnityEvent();
    public UnityEvent<string> simulationErrorEvent = new UnityEvent<string>();

}
