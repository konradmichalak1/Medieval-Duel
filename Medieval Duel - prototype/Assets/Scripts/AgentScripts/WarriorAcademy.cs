using Assets.Scripts;
using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAcademy : Academy {
    public List<AgentController> agents;

    public void resetAgents()
    {
        agents.ToArray();
        if(!agents[0].mvC.state.isAlive)
        {
            agents[0].AddReward(-1.0f);
            agents[0].Done();
            agents[1].AddReward(1.0f);
            agents[1].Done();
        }
        else if (!agents[1].mvC.state.isAlive)
        {
            agents[0].AddReward(1.0f);
            agents[0].Done();
            agents[1].AddReward(-1.0f);
            agents[1].Done();
        }
    }
}
