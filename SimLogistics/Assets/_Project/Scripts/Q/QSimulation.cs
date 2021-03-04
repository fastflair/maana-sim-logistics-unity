using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

public static class QSimulationFragment
{
    public const string data = @"
        fragment simulationData on Simulation {
          id
          name
          agentEndpoint
          steps
          balance
          income
          expenses
        }";
}

public class QSimulation
{
    public string id;
    public string name;
    public string agentEndpoint;
    public int steps;
    public float balance;
    public float income;
    public float expenses;
}
