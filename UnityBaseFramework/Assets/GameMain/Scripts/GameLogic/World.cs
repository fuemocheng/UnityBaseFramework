using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGame
{
    public class World
    {
        public int Tick { get; private set; }

        //private List<BaseSystem> _systems = new List<BaseSystem>();

        private bool _hasStart = false;

        public void Start()
        {
            Tick = 0;
        }

        public void OnGameCreate()
        {
            //初始化Service, 初始化系统

        }

        public void StartSimulate()
        {
            if(_hasStart)
            {
                return;
            }

            _hasStart = true;


        }
        

        /// <summary>
        /// 帧步进。
        /// </summary>
        public void Step()
        {
            //更新系统



            Tick++;
        }


    }
}
