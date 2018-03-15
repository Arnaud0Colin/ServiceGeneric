using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceGeneric
{


    /// <summary>
    /// Create a singleton
    /// </summary>
    /// <example>  </example>/// 
    public class Singleton<T> where T : new()
    {
        /// <summary>
        /// Instance
        /// </summary>
        /// <example>  Truc.Instance </example>/// 
        public readonly static T Instance = new T();
    }

    /// <summary>
    /// Create a singleton With Lock
    /// </summary>
    /// <example>  </example>/// 
    public sealed class SingletonLock<T> where T : new()
    {
       private static /*volatile*/ T instance;
       private static object syncRoot = new Object();

       /// <summary>
       /// Instance
       /// </summary>
       /// <example>  Truc.Instance </example>/// 
       public static T Instance
       {
          get 
          {
             if (instance == null) 
             {
                lock (syncRoot) 
                {
                   if (instance == null) 
                      instance = new T();
                }
             }

             return instance;
          }
       }
    }

}
