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
    public class AgentController : Agent
    {
        System.Random rnd = new System.Random();

        public MoveController mvC;
        public MoveController mvCEnemy;
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

        public float previousDistance = float.MaxValue;
        public float distanceToTarget = float.MaxValue;

        public Vector3 relativePosition;
        const float rayDistance = 10f;
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };

        string[] detectableObjects = new string[] { "Enemy", "Wall", "Fence", "Player", "Obstacle" };
        float fencesStartPositionY;
        float obstaclesStartPositionX;
        float obstaclesStartPositionY;

        public Vector3 startPostion;
        public Quaternion startRotation;
        public Vector3 targetPosition;
               
        public WarriorAcademy academy;
        
        public float previousAgentHp;
        public float previousEnemyHp;
        public float localY;
        public float lookAtRotation;

        public string mode;
        private const string walkMode = "WalkMode";
        private const string fightMode = "FightMode";
        private const string jumpMode = "JumpMode";
        private const string walkAndJumpModeJumper = "WalkAndJumpModeJumper";
        private const string walkAndJumpModeWalker = "WalkAndJumpModeWalker";

        void Start()
        {
            mvC = GetComponent<MoveController>();         
            rayPer = GetComponent<RayPerception>();
            rBody = GetComponent<Rigidbody>();
            academy = FindObjectOfType<WarriorAcademy>();

            startPostion = this.transform.position;
            startRotation = this.transform.rotation;
            targetPosition = Target.position;

            mvC.moveSpeed = 7.0f;

            PrepareScene();
        }

        public override void AgentReset()
        {
            int which = rnd.Next(0, 4);
            mvC.transform.position = new Vector3(startPostion.x, startPostion.y, startPostion.z);
            mvC.transform.rotation = startRotation;
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

            lookAtRotation = Vector3.Angle(mvC.transform.forward, relativePosition);

            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, -0.6f, 0));
            AddVectorObs(relativePosition.x / float.MaxValue);
            AddVectorObs(relativePosition.z / float.MaxValue);
            AddVectorObs(distanceToTarget / float.MaxValue);
            AddVectorObs(previousDistance / float.MaxValue);
            AddVectorObs(mvC.transform.position.x / float.MaxValue);
            AddVectorObs(mvC.transform.position.z / float.MaxValue);
            AddVectorObs(mvC.transform.position.y / float.MaxValue);
            if (mode != fightMode)
                AddVectorObs(mvC.transform.localRotation.y);
            else
                AddVectorObs(lookAtRotation / 180);
            AddVectorObs(Target.position.x / float.MaxValue);
            AddVectorObs(Target.position.z / float.MaxValue);
            AddVectorObs(mvC.stats.currentStamina / 100);

            if(mode == fightMode)
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

                if (mvCEnemy.state.lightAttack)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvCEnemy.state.heavyAttack)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvCEnemy.state.isImpact)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvCEnemy.state.isShieldImpact)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);


                if (mvC.state.isAttacking)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvC.state.lightAttack)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvC.state.heavyAttack)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvC.state.isImpact)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);

                if (mvC.state.isShieldImpact)
                    AddVectorObs(1);
                else
                    AddVectorObs(0);
            }
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            AgentActionInDiffrentScenes(vectorAction);
            localY = this.mvC.transform.localRotation.y;
            
            AddReward(-0.05f);
            if (mode != fightMode)
            {
                if (Math.Round(previousDistance, 1) > Math.Round(distanceToTarget, 1))
                {
                    AddReward(0.1f);
                    if (this.mvC.transform.localRotation.y > -0.5f && this.mvC.transform.localRotation.y < -0.5f)
                        AddReward(0.1f);
                }

                if (Math.Round(previousDistance, 1) < Math.Round(distanceToTarget, 1))
                {
                    AddReward(-0.1f);
                    if (this.mvC.transform.localRotation.y < -0.5f || this.mvC.transform.localRotation.y > 0.5f)
                        AddReward(-0.1f);
                }


                if (this.mvC.transform.position.z < startPostion.z - 2.5)
                {
                    AddReward(-1.0f);
                    Done();
                }
            }
            else
            {
                if (Math.Round(previousDistance, 1) > Math.Round(distanceToTarget, 1))
                {
                    AddReward(0.1f);
                    if (lookAtRotation < 90)
                        AddReward(0.1f);
                }
            }
            previousDistance = distanceToTarget;

            mvC.CharacterMovement(vectorAction);
            mvC.SetAnimatorValues(vectorAction);
        }

        public void PrepareScene()
        {
            if (mode == jumpMode)
                fencesStartPositionY = fences[0].transform.position.y;

            if (mode == walkMode)
            {
                obstaclesStartPositionX = obstacles[0].transform.position.x;
                obstaclesStartPositionY = obstacles[0].transform.position.y - 5;
            }

            if (mode == walkAndJumpModeWalker)
            {
                obstaclesStartPositionX = obstacles[0].transform.position.x;
                obstaclesStartPositionY = obstacles[0].transform.position.y - 5;
                fencesStartPositionY = fences[0].transform.position.y;
            }

            if (mode == fightMode)
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
            if (mode == walkMode || mode == walkAndJumpModeWalker)
            {
                foreach (var x in obstacles)
                {
                    int pom = rnd.Next(-3, 3);
                    x.transform.position = new Vector3(obstaclesStartPositionX + pom, obstaclesStartPositionY, x.transform.position.z);
                }
                if (academy.resetParameters["walls"] == 0)
                {
                    obstacles[3].transform.position = new Vector3(obstacles[3].transform.position.x, obstaclesStartPositionY + 5, obstacles[3].transform.position.z);
                }
                else if (academy.resetParameters["walls"] == 1)
                {
                    obstacles[3].transform.position = new Vector3(obstacles[3].transform.position.x, obstaclesStartPositionY + 5, obstacles[3].transform.position.z);
                    obstacles[2].transform.position = new Vector3(obstacles[2].transform.position.x, obstaclesStartPositionY + 5, obstacles[2].transform.position.z);
                }
                else if (academy.resetParameters["walls"] == 2)
                {
                    obstacles[3].transform.position = new Vector3(obstacles[3].transform.position.x, obstaclesStartPositionY + 5, obstacles[3].transform.position.z);
                    obstacles[2].transform.position = new Vector3(obstacles[2].transform.position.x, obstaclesStartPositionY + 5, obstacles[2].transform.position.z);
                    obstacles[1].transform.position = new Vector3(obstacles[1].transform.position.x, obstaclesStartPositionY + 5, obstacles[1].transform.position.z);
                }
                else if (academy.resetParameters["walls"] == 3)
                {
                    obstacles[3].transform.position = new Vector3(obstacles[3].transform.position.x, obstaclesStartPositionY + 5, obstacles[3].transform.position.z);
                    obstacles[2].transform.position = new Vector3(obstacles[2].transform.position.x, obstaclesStartPositionY + 5, obstacles[2].transform.position.z);
                    obstacles[1].transform.position = new Vector3(obstacles[1].transform.position.x, obstaclesStartPositionY + 5, obstacles[1].transform.position.z);
                    obstacles[0].transform.position = new Vector3(obstacles[0].transform.position.x, obstaclesStartPositionY + 5, obstacles[0].transform.position.z);
                }
            }


            if (mode == walkAndJumpModeWalker || mode == jumpMode)
            {
                fences[0].transform.localScale = new Vector3(
                    fences[0].transform.localScale.x,
                    academy.resetParameters["high_fence"],
                    fences[0].transform.localScale.z);
            }

            if(mode != fightMode)
            {
                Target.position = new Vector3(targetPosition.x + rnd.Next(-4, 0), 0.5f, targetPosition.z + rnd.Next(-4, 0));
            }
            if(mode == fightMode)
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
            if (mode == jumpMode)
            {
                if (this.transform.position.z > fences[0].transform.position.z + 4)
                {
                    AddReward(1.0f);
                    Done();
                }
            }

            if (mode == walkMode)
            {
                if (this.transform.position.z > obstacles[3].transform.position.z + 2)
                {
                    AddReward(1.0f);
                    Done();
                }
            }

            if (mode == walkAndJumpModeWalker)
            {
                if (this.transform.position.z > obstacles[3].transform.position.z + 2)
                {
                    AddReward(1.0f);
                    mode = walkAndJumpModeJumper;
                    GiveBrain(jumpBrain);
                }
            }

            if (mode == walkAndJumpModeJumper)
            {
                if (this.transform.position.z > fences[0].transform.position.z + 4)
                {
                    AddReward(1.0f);
                    GiveBrain(walkBrain);
                    mode = walkAndJumpModeWalker;
                    Done();
                }
            }

            if (mode == fightMode)
            {
                if (vectorAction[4] > 0)
                    afc.Block();

                if (vectorAction[3] > 0.5)
                {
                    afc.LightAttack();
                }
                else if (vectorAction[3] < -0.5)
                    afc.HeavyAttack();

                if (previousAgentHp > mvC.stats.currentHp)
                {
                    AddReward(-0.01f * (previousAgentHp - mvC.stats.currentHp));
                    previousAgentHp = mvC.stats.currentHp;
                }
                if (previousEnemyHp > tsc.currentHp)
                {
                    AddReward(0.02f * (previousEnemyHp - tsc.currentHp));
                    previousEnemyHp = mvCEnemy.stats.currentHp;
                }

                academy.resetAgents();
                Target.position = mvCEnemy.transform.position;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {            
            if (hit.gameObject.tag == "Wall")
            {
                AddReward(-0.5f);               
            }

            if (hit.gameObject.tag == "Obstacle")
            {
                AddReward(-0.5f);
            }
        }

    }
}
