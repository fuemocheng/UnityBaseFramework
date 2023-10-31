using System.Collections.Generic;
using UnityBaseFramework.Runtime;

namespace XGame
{
    public class MapComponent : BaseFrameworkComponent
    {
        private List<MapGrid> m_MapGrids = new List<MapGrid>();

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

        public void ClearMapGrid()
        {
            m_MapGrids.Clear();
        }

        public void AddMapGrid(MapGrid mapGrid)
        {
            m_MapGrids.Add(mapGrid);
        }

        public void AddAllMapColliderProxy()
        {
            foreach (var mapGrid in m_MapGrids)
            {
                mapGrid.AddColliderProxy();
            }
        }

        public void RemoveAddMapColliderProxy()
        {
            foreach (var mapGrid in m_MapGrids)
            {
                mapGrid.RemoveColliderProxy();
            }
        }
    }
}
