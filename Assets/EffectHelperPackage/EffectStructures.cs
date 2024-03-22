using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


  // interfaces
  public interface IController {
    public void SetTrigger(string name,float percent);
    public void SetTriggerFade(string name ,float percent, float duration);

  }

  [Serializable]
  public class ControllerWrapper {
      public MonoBehaviour component;

      public IController GetController() {
          return component as IController;
      }
  }

  // structs
  [System.Serializable]
 
    public struct FloatParaSet{
       
        public string para;
         [Tooltip("default sleep")]
        public float val1;
         [Tooltip("default wake")]
        public float val2;
    }

     [System.Serializable]
    public struct ColorParaSet{
        public string para;
        [ColorUsage(true, true)] 
        public Color color1;
        [ColorUsage(true, true)] 
        public Color color2;
    }

     [System.Serializable]
     public struct BoolParaSet{
        public string para;
        [Range(0, 1)] 
        public float threshold;
     }
    [System.Serializable]
    public struct Vec3ParaSet{
      public TrasnsMode mode;
      [Tooltip("default sleep")]
      public Vector3 vec01;
       [Tooltip("default wake")]
      public Vector3 vec02;
    }
     [System.Serializable]
    public struct LightColorParaSet{
  
        [ColorUsage(true, true)] 
        public Color color1;
        [ColorUsage(true, true)] 
        public Color color2;
    }
    [System.Serializable]
    public struct LightFloatParaSet{
        public LightPara para;
       
          [Tooltip("default min")]
        public float val1;
         [Tooltip("default max")]
        public float val2;
    }

    [System.Serializable]
     public struct LightBoolParaSet{
        public bool need;
        [Range(0, 1)] 
        public float threshold;
     }

    [System.Serializable]
    public struct ControlEvent{
     
      public string ID ;
      public bool is_testing ;
      
      [Range(0, 1)] 
      public float testbar ;
    }

     public struct MatInstSet{
        public string Obj_name;
        public Material Mat;
      
     }

    // broadCastr & Reciever

    [System.Serializable]
    public struct SingleAction{
      public string action_name;
      public List<string> trigger_names;
      public int ID;
      [Range(0, 1)] 
      public float target;
    }
    [System.Serializable]
    public struct FadeAction{
      public string action_name;
      public float duration;
      public List<string> trigger_names;
      public int ID;
      [Range(0, 1)] 
      public float target;
    }
    
    public struct BroadcastCall{
      public string action_name;
      public int callIndex;
    }


     public enum LightPara{
        INTENSITY,
        RANGE,


     }

     public enum TrasnsMode{
      POSITION,
      ROTATION,
      SCALE,
      VELOCITY,
      ROTATEVELOCITY,

     }
    