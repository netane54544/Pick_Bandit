using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pick_Bandit
{
    internal class Agent
    {
        public const double DEFUALT_ALPHA = 0.1;
        private int[] enemysLevels;
        private double alpha;
        private double[] expected;
        private int[] N;
        private static Random rnd;
        public readonly int myLevel;
        private uint time = 0;
        public bool isLCB = true;

        public Agent(int agentLevel, int[] enemysLevels, double alpha, bool isLCB = true) 
        {
            if (alpha <= 0 || alpha >= 1)
                throw new Exception("The exploration rate must be between 0 and 1");

            if(agentLevel <= 0)
                throw new Exception("The agent level must be greater than 0");

            if(enemysLevels == null)
                throw new Exception("The enemy's level array must be initialized");

            this.myLevel = agentLevel;
            this.alpha = alpha;
            this.isLCB = isLCB;
            this.expected = new double[3];
            this.N = [0, 0, 0];
            rnd = MainWindow.rnd;

            this.enemysLevels = new int[enemysLevels.Length];
            Array.Copy(enemysLevels, this.enemysLevels, enemysLevels.Length);
        }

        public void UpdateEnemysLevels(int[] enemysLevels) => Array.Copy(enemysLevels, this.enemysLevels, enemysLevels.Length);

        public void ChangeExplorationRate(double alpha)
        {
            if (alpha <= 0 || alpha >= 1)
                throw new Exception("The exploration rate must be between 0 and 1");

            this.alpha = alpha;
        }

        public int[] ActionCopy()
        {
            int[] copyArr = new int[N.Length];
            Array.Copy(N, copyArr, N.Length);
            return copyArr;
        } 

        private double[] Rewards()
        {
            double[] rewards = new double[3];

            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i] = Utils.Sigmoid(enemysLevels[i] - myLevel);
            }

            return rewards;
        }

        private int LCB()
        {
            int minIndex = 0;
            double min = double.MaxValue;
            

            for (int i = 0; i < expected.Length; i++)
            {
                double variance = alpha * Math.Sqrt(Math.Log(time) / N[i]);

                if (expected[i] - variance < min)
                {
                    minIndex = i;
                    min = expected[i] - variance;
                }
            }

            return minIndex;
        }

        private int EGreedy()
        {
            int minIndex = 0;
            double min = double.MaxValue;


            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] < min)
                {
                    minIndex = i;
                    min = expected[i];
                }
            }

            return minIndex;
        }

        public int MakeMove()
        {
            time++;
            int action;

            if (isLCB)
            {
                action = LCB();
            }
            else
            {
                if (rnd.NextDouble() < 1 - alpha)
                    action = EGreedy();
                else
                    action = Utils.RandomExclude(EGreedy(), rnd);
            }

            N[action]++;

            double[] rewards = Rewards();
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i] = expected[i] + alpha * (rewards[i] - expected[i]);
            }

            return action;
        }

    }
}
