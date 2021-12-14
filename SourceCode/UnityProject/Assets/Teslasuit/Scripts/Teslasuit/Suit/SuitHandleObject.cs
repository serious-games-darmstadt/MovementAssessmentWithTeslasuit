using UnityEngine;
using System;
using System.Threading;

namespace TeslasuitAPI
{

    /**@addtogroup UnityComponents */
    /*@{*/

    /// <summary>
    /// HandleObject
    /// </summary>
    public class SuitHandleObject : MonoBehaviour
    {
        /// <summary>
        /// SuitIndex of suit which is waited for
        /// </summary>
        public SuitIndex SuitIndex
        {
            get { return suitIndex; }
            set
            {
                bool changed = this.suitIndex != value;
                if(changed)
                {
                    if(IsAvailable)
                        OnSuitDisconnected();

                    this.suitIndex = value;
                    InitHandle(this.suitIndex);
                }
                
            }
        }
        [SerializeField]
        private SuitIndex suitIndex = SuitIndex.None;


        /// <summary>
        /// event of suit connection
        /// </summary>
        private event Action<SuitHandleObject> becameAvailable = delegate { };
        public event Action<SuitHandleObject> BecameAvailable
        {
            add
            {
                if (IsAvailable)
                    value.Invoke(this);
                becameAvailable += value;
            }
            remove
            {
                becameAvailable -= value;
            }
        }

        /// <summary>
        /// event when suit disconnects
        /// </summary>
        public event Action<SuitHandleObject> BecameUnavailable = delegate { };

        /// <summary>
        /// suithandle of this suit
        /// </summary>
        public ISuitHandle Handle { get; private set; }

        /// <summary>
        /// check if suit is connected
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return Handle != null && Handle.State == HandleState.Connected;
            }
        }

        /// <summary>
        /// ISuit object
        /// </summary>
        public ISuit Suit
        {
            get
            {
                if (IsAvailable)
                    return Handle.Suit;
                else return null;
            }
        }

        protected void Start()
        {
            //Teslasuit.Load();
            InitHandle(suitIndex);
        }

        void InitHandle(SuitIndex index)
        {
            Handle = Teslasuit.DeviceManager.GetHandle(index);

            Handle.Connected += OnSuitConnected;
            Handle.Disconnected += OnSuitDisconnected;

            if(Handle.State == HandleState.Connected)
            {
                OnSuitConnected();
            }
        }

        private void OnSuitConnected()
        {
            MainThreadDispatcher.Execute(HandleUpdated, Handle);
        }


        private void HandleUpdated(object handle)
        {
            HandleUpdated((ISuitHandle)handle);
        }

        private void HandleUpdated(ISuitHandle handle)
        {
            if (Handle != null && Handle.State == HandleState.Connected)
            {
                becameAvailable(this);
            }
        }

        private void OnSuitDisconnected()
        {
            MainThreadDispatcher.Execute(() => BecameUnavailable(this));
            //Handle = null;
        }

        private void OnDestroy()
        {
            Destroy();
        }

        private void Destroy()
        {
            
            if (Teslasuit.Loaded())
            {
                //Handle.Connected -= OnSuitConnected;
                //Handle.Disconnected -= OnSuitDisconnected;
            }
                
            //Handle = null;
        }

        //TODO is it enough?
        //TODO enable
        //public void ChangeSuitIndex(SuitIndex newIndex)
        //{
        //    if(Handle.State == HandleState.Connected)
        //    {
        //        Teslasuit.DeviceManager.OnDeviceDisconnected -= OnSuitDisconnected;
        //    }
        //    else
        //    {
        //        Teslasuit.DeviceManager.OnDeviceDisconnected -= OnSuitConnected;
        //    }
        //    suitIndex = newIndex;
        //    InitHandle();
        //}
    }
    /*@}*/
}