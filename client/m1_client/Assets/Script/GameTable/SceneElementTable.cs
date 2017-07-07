using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table
{
    public class SceneElementTable
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ArtResource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Stop { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Destroy1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Destroy2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Invisible { get; set; }
    }

    public class SceneElementTableConfig
    {
        public string GetJsonName()
        {
            return "SceneElementTable";
        }
    }
}
