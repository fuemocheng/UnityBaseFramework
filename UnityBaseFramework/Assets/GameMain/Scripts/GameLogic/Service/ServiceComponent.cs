using Lockstep.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityBaseFramework.Runtime;
using UnityEngine;

namespace XGame
{
    public class ServiceComponent : BaseFrameworkComponent
    {
        private Dictionary<Type, IService> m_Services = new Dictionary<Type, IService>();

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {

        }

        void Update()
        {

        }

        public IService[] GetAllServices()
        {
            return m_Services.Values.ToArray();
        }

        public void RegisterService(IService service, bool overwriteExisting = true)
        {
            //var interfaceTypes = service.GetType().FindInterfaces(
            //        (type, criteria) =>
            //        type.GetInterfaces().Any(t => t == typeof(IService)), service).ToArray();

            //foreach (var type in interfaceTypes)
            //{
            //    if (!m_Services.ContainsKey(type))
            //    {
            //        m_Services.Add(type, service);
            //    }
            //    else if (overwriteExisting)
            //    {
            //        m_Services[type] = service;
            //    }
            //}

            if (service != null)
            {
                Type type = service.GetType();
                if(!m_Services.ContainsKey(type))
                {
                    m_Services.Add(type, service);
                }
                else if(overwriteExisting)
                {
                    m_Services[type] = service;
                }
            }
        }

        public T GetService<T>() where T : IService
        {
            Type key = typeof(T);

            if (!m_Services.ContainsKey(key))
            {
                return default(T);
            }
            else
            {
                return (T)m_Services[key];
            }
        }
    }
}
