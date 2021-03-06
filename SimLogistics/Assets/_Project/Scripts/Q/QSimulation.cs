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
    [UsedImplicitly] public string id;
    [UsedImplicitly] public string name;
    [UsedImplicitly] public string agentEndpoint;
    [UsedImplicitly] public int steps;
    [UsedImplicitly] public float balance;
    [UsedImplicitly] public float income;
    [UsedImplicitly] public float expenses;

    public override string ToString()
    {
        return @$"{{
            id: ""{id}""
            name: ""{name}""
            agentEndpoint: ""{agentEndpoint}""
            steps: {steps}
            balance: {balance}
            income: {income}
            expenses: {expenses}
        }}";
    }
}
