using System.Collections.Generic;
using System.Threading.Tasks;
using BL.Servers.CoC.Logic.Structure;

namespace BL.Servers.CoC.Logic.Manager
{
    internal class WorkerManager
    {
        internal List<GameObject> GameObjectReferences;

        internal int WorkerCount;

        public WorkerManager()
        {
            this.GameObjectReferences = new List<GameObject>();
            this.WorkerCount = 0;
        }

        public static int GetFinishTaskOfOneWorkerCost() => 0;

        public static void RemoveGameObjectReferences(GameObject go)
        {
        }

        public void AllocateWorker(GameObject go)
        {
            if (this.GameObjectReferences.IndexOf(go) == -1)
            {
                this.GameObjectReferences.Add(go);
            }
        }

        public void DeallocateWorker(GameObject go)
        {
            if (this.GameObjectReferences.IndexOf(go) != -1)
            {
                this.GameObjectReferences.Remove(go);
            }
        }

        public void DecreaseWorkerCount() => this.WorkerCount--;

        public void FinishTaskOfOneWorker()
        {
            GameObject go = GetShortestTaskGO;
            if (go != null)
            {
                if (go.ClassId == 3)
                {
                }
                else
                {
                    var b = (ConstructionItem)go;
                    if (b.IsConstructing)
                        b.SpeedUpConstruction();
                    else
                    {
                        var hero = b.GetHeroBaseComponent();
                        hero?.SpeedUpUpgrade();
                    }
                }
            }
        }

        public int GetFreeWorkers() => this.WorkerCount - this.GameObjectReferences.Count;

        public GameObject GetShortestTaskGO
        {
            get
            {
                GameObject shortestTaskGO = null;
                int shortestGOTime = 0;
                int currentGOTime;

                Parallel.ForEach((this.GameObjectReferences), go =>
                {
                    currentGOTime = -1;
                    if (go.ClassId == 3)
                    {
                    }
                    else
                    {
                        var c = (ConstructionItem)go;
                        if (c.IsConstructing)
                        {
                            currentGOTime = c.GetRemainingConstructionTime();
                        }
                        else
                        {
                            var hero = c.GetHeroBaseComponent();
                            if (hero != null)
                            {
                                if (hero.IsUpgrading())
                                {
                                    currentGOTime = hero.GetRemainingUpgradeSeconds();
                                }
                            }
                        }
                    }
                    if (shortestTaskGO == null)
                    {
                        if (currentGOTime > -1)
                        {
                            shortestTaskGO = go;
                            shortestGOTime = currentGOTime;
                        }
                    }
                    else if (currentGOTime > -1)
                    {
                        if (currentGOTime < shortestGOTime)
                        {
                            shortestGOTime = currentGOTime;
                            shortestTaskGO = go;
                        }
                    }
                });
                return shortestTaskGO;
            }
        }

        public void IncreaseWorkerCount() => this.WorkerCount++;
    }
}
