﻿using System;
using UnityEngine;
namespace Snaker.Game.Entity.Factory
{
    //不继承Mono
    public abstract class EntityObject:IRecyclableObject
	{
        //----------------------------------------------------------------------
        private bool m_isReleased = false;
        public bool IsReleased { get { return m_isReleased; } }


		internal void InstanceInFactory()
		{
			m_isReleased = false;
		}

        //----------------------------------------------------------------------
        internal void ReleaseInFactory()
		{
			if (!m_isReleased)
			{
				Release();
				m_isReleased = true;
			}
		}

		protected abstract void Release();


		//----------------------------------------------------------------------
        public virtual Vector3 Position()
        {
            return Vector3.zero;
        }


		//----------------------------------------------------------------------

        public string GetRecycleType()
        {
            //存在反射，最好缓存
            return this.GetType().FullName;
        }

        public void Dispose()
        {
            //由系统的GC机制来处理
            //Do nothing!
        }
        //----------------------------------------------------------------------
    }
}
