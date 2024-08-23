

using SGF;
using UnityEngine;

namespace Snaker.Game.Entity.Factory
{


    //==========================================================

    public static class ViewFactory
    {
		public static bool EnableLog = false;
        private const string LOG_TAG = "ViewFactory";
        private static bool m_isInit = false;
        private static Transform m_viewRoot;
        private static Recycler m_recycler;

        /// <summary>
        /// 工厂所实例化的对象列表，player由多个实体节点组成，每个实体节点绑定一个view
        /// </summary>
        private static DictionaryEx<EntityObject, ViewObject> m_mapObject;


        public static void Init(Transform viewRoot)
        {
            if (m_isInit)
            {
                return;
            }
            m_isInit = true;

            m_viewRoot = viewRoot;

            m_mapObject = new DictionaryEx<EntityObject, ViewObject>();
            m_recycler = new Recycler();

        }

        /// <summary>
        /// 释放工厂所创建的所有对象，包括空闲的对象
        /// </summary>
        public static void Release()
        {
            m_isInit = false;

            foreach(var pair in m_mapObject)
            {
                pair.Value.ReleaseInFactory();
                pair.Value.Dispose();
            }
            m_mapObject.Clear();

            m_recycler.Release();

            m_viewRoot = null;
        }


		public static void CreateView(string resPath, string resDefaultPath, EntityObject entity, Transform parent = null)
        {
            ViewObject obj = null;
            string recycleType = resPath;
			bool useRecycler = true;
            //从缓冲池中取，根据资源路径
            obj = m_recycler.Pop(recycleType) as ViewObject;
			if (obj == null) 
			{
                //没找到，从资源路径实例化，并且返回ViewObject
				useRecycler = false;
				obj = InstanceViewFromPrefab (recycleType, resDefaultPath);
			}

            if (obj != null)
            {
				if (!obj.gameObject.activeSelf) 
				{
					obj.gameObject.SetActive (true);
				}

                if (parent != null)
                {
                    obj.transform.SetParent(parent, false);
                }
                else
                {
                    obj.transform.SetParent(m_viewRoot, false);
                }

                obj.CreateInFactory(entity,recycleType);

				if (EnableLog && Debuger.EnableLog) 
				{
					Debuger.Log (LOG_TAG, "CreateView() {0}:{1} -> {2}:{3}, UseRecycler:{4}", 
						entity.GetType ().Name, entity.GetHashCode (),
						obj.GetRecycleType (), obj.GetInstanceID (),
						useRecycler);
				}
				
				if (m_mapObject.ContainsKey (entity)) 
				{
					Debuger.LogError(LOG_TAG,"CreateView() 不应该存在重复的映射！");
				}
				m_mapObject [entity] = obj;
                //m_mapObject.Add(entity, obj);
            }
        }


        public static void ReleaseView(EntityObject entity)
        {
            if (entity != null)
            {

                ViewObject obj = m_mapObject[entity];
                if (obj != null)
                {
					if (EnableLog && Debuger.EnableLog) 
					{
						Debuger.Log (LOG_TAG, "ReleaseView() {0}:{1} -> {2}:{3}", 
							entity.GetType ().Name, entity.GetHashCode (),
							obj.GetRecycleType (), obj.GetInstanceID ());
					}

                    m_mapObject.Remove(entity);
                    obj.ReleaseInFactory();
					obj.gameObject.SetActive (false);

                    //将对象加入对象池，放入的是Mono，但是根据Mono中资源名决定这是哪个type。因为同个FoodView脚本可能挂载到多个不同GameObject上
                    m_recycler.Push(obj);



                }
            }
        }



        private static ViewObject InstanceViewFromPrefab(string prefabName, string defaultPrefabName)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabName);
            if(prefab == null)
            {
                prefab = Resources.Load<GameObject>(defaultPrefabName);
            }
            GameObject go = GameObject.Instantiate(prefab);
            ViewObject instance = go.GetComponent<ViewObject>();

            if (instance == null)
            {
                Debuger.LogError(LOG_TAG, "InstanceViewFromPrefab() prefab = " + prefabName);
            }

            return instance;
        }






    }
}
