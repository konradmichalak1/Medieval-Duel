using Assets.Scripts.EnemyScripts;
using MLAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts
{
    class AgentController : Agent
    {
        System.Random rnd = new System.Random();

        MoveController mvC;
        MoveController mvCEnemy;
        RayPerception rayPer;
        Rigidbody rBody;
        public Transform Target;

        AgentFightingController afc;
        AgentFightingController tfc;

        StatsController tsc;
        States enemyStates;

        public List<GameObject> obstacles = new List<GameObject>();
        public List<GameObject> fences = new List<GameObject>();

        public Brain jumpBrain;
        public Brain walkBrain;

        private float previousDistance = float.MaxValue;
        private float distanceToTarget = float.MaxValue;

        public Vector3 relativePosition;
        const float rayDistance = 10f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };

        string[] detectableObjects = new string[] { "Enemy", "Wall", "Fence", "Player" };
        float fencesStartPositionY;
        float obstaclesStartPositionX;
        float obstaclesStartPositionY;

        public Vector3 startPostion;
        public Vector3 targetPosition;
               
        public WarriorAcademy academy;
        public int changeFlag = -1;
        public float previousAgentHp;
        public float previousEnemyHp;
        

        void Start()
        {
            mvC = GetComponent<MoveController>();         
            rayPer = GetComponent<RayPerception>();
            rBody = GetComponent<Rigidbody>();
            academy = FindObjectOfType<WarriorAcademy>();

            startPostion = this.transform.position;
            targetPosition = Target.position;

            mvC.moveSpeed = 7.0f;

            PrepareScene();
        }

        public override void AgentReset()
        {
            int which = rnd.Next(0, 4);
            mvC.transform.position = new Vector3(startPostion.x, startPostion.y, startPostion.z - 2);
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            mvC.stats.currentStamina = 100;
            previousDistance = float.MaxValue;

            AgentResetInDiffrentScenes();
        }

        public override void CollectObservations()
        {
            distanceToTarget = Vector3.Distance(this.transform.position,
                                              Target.position);
            relativePosition = Target.position - this.transform.position;

            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, -0.6f, -0.6f));
            AddVectorObs(relativePosition.x / float.MaxValue);
            AddVectorObs(relativePosition.z / float.MaxValue);
            AddVectorObs(distanceToTarget / float.MaxValue);
            AddVectorObs(previousDistance / float.MaxValue);
            AddVectorObs(mvC.transform.position.x / float.MaxValue);
            AddVectorObs(mvC.transform.position.z / float.MaxValue);
            AddVectorObs(mvC.transform.position.y / float.MaxValue);
            AddVectorObs(Target.position.x / float.MaxValue);
            AddVectorObs(Target.position.z / float.MaxValue);
            AddVectorObs(mvC.stats.currentStamina / 100);

            if(changeFlag == 5)
            {
                AddVectorObs(mvC.stats.currentHp / 100);
                AddVectorObs(mvCEnemy.stats.currentHp / 100);
                if (mvCEnemy.state.isAttacking)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvCEnemy.state.isBlocking)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);
            }
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            AgentActionInDiffrentScenes(vectorAction);

            int i = this.GetStepCount();
            if (i % 60 == 0)
                AddReward(-0.05f);

            if (Math.Round(previousDistance, 1) > Math.Round(distanceToTarget, 1))
            {
                AddReward(0.05f);
            }

            previousDistance = distanceToTarget;
            if (changeFlag != 1)
                mvC.CharacterMovement(vectorAction);
            else
                mvC.CharacterMovementWithoutLefRight(vectorAction);
            mvC.SetAnimatorValues(vectorAction);
        }

        public void PrepareScene()
        {
            if (changeFlag == 0)
                fencesStartPositionY = fences[0].transform.position.y;

            if (changeFlag == 1)
            {
                obstaclesStartPositionX = obstacles[0].transform.position.x;
                obstaclesStartPositionY = obstacles[0].transform.position.y;
            }

            if (changeFlag == 2)
            {
                obstaclesStartPositionX = obstacles[0].transform.position.x;
                obstaclesStartPositionY = obstacles[0].transform.position.y;
                fencesStartPositionY = fences[0].transform.position.y;
            }

            if (changeFlag == 5)
            {
                afc = GetComponent<AgentFightingController>();
                tfc = Target.GetComponent<AgentFightingController>();
                tsc = Target.GetComponent<StatsController>();
                mvCEnemy = Target.GetComponent<MoveController>();
                enemyStates = Target.GetComponent<States>();
            }
        }

        public void AgentResetInDiffrentScenes()
        {
            if (changeFlag == 1)
            {
                foreach (var x in obstacles)
                {
                    int pom = rnd.Next(-4, 4);
                    x.transform.position = new Vector3(obstaclesStartPositionX + pom, obstaclesStartPositionY, x.transform.position.z);
                }
            }

            if (changeFlag == 2 || changeFlag == 0)
            {
                fences[0].transform.localScale = new Vector3(
                    fences[0].transform.localScale.x,
                    academy.resetParameters["high_fence"],
                    fences[0].transform.localScale.z);
            }

            if(changeFlag != 5)
            {
                Target.position = new Vector3(targetPosition.x + rnd.Next(-4, 0), 0.5f, targetPosition.z + rnd.Next(-4, 0));
            }
            if(changeFlag == 5)
            {
                previousAgentHp = 100;
                previousEnemyHp = 100;
                mvC.stats.currentHp = 100;
                mvCEnemy.stats.currentHp = 100;
                mvC.state.isAlive = true;
                mvCEnemy.state.isAlive = true;
            }
        }

        public void AgentActionInDiffrentScenes(float[] vectorAction)
        {
            if (changeFlag == 0)
            {
                if (this.transform.position.z > fences[0].transform.position.z + 4)
                {
                    AddReward(1.0f);
                    Done();
                }
            }

            if (changeFlag == 1)
            {
                if (this.transform.position.z > obstacles[3].transform.position.z + 2)
                {
                    AddReward(1.0f);
                    Done();
                }
            }


            if (changeFlag == 2)
            {
                if (this.transform.position.z > obstacles[3].transform.position.z + 2)
                {
                    AddReward(1.0f);
                    changeFlag = 3;
                    GiveBrain(jumpBrain);
                }
            }

            if (changeFlag == 3)
            {
                if (this.transform.position.z > fences[0].transform.position.z + 4)
                {
                    AddReward(1.0f);
                    GiveBrain(walkBrain);
                    changeFlag = 2;
                    Done();
                }
            }

            if (changeFlag == 4)
            {
                if (this.transform.position.z > fences[0].transform.position.z + 4)
                {
                    AddReward(1.0f);
                    changeFlag = -1;
                    GiveBrain(walkBrain);
                    Done();
                }
            }

            if (changeFlag == 5)
            {
                if (vectorAction[2] > 0)
                    afc.LightAttack();
                else
                    afc.HeavyAttack();

                if (vectorAction[3] > 0)
                    afc.Block();

                if (previousAgentHp > mvC.stats.currentHp)
                {
                    AddReward(-0.1f);
                    previousAgentHp = mvC.stats.currentHp;
                }
                if (previousEnemyHp > tsc.currentHp)
                {
                    AddReward(0.1f);
                    previousEnemyHp = mvCEnemy.stats.currentHp;
                }

                if(!mvC.state.isAlive)
                {
                    AddReward(-1.0f);
                    Done();
                }

                if (!mvCEnemy.state.isAlive)
                {
                    AddReward(1.0f);
                    Done();
                }
                Target.position = mvCEnemy.transform.position;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {            
            if (hit.gameObject.tag == "Wall")
            {
                AddReward(-0.5f);               
            }
        }

    }
}
