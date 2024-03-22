using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.PackageManager;
using UnityEngine;



public class TransformController : MonoBehaviour,IController
{   
     
    [System.Serializable]
    public struct Transsocket{
    
        public GameObject obj;
        public List<Vec3ParaSet> Vec3ParaList;

    }
    [System.Serializable]
    public struct Trigger{
        public string ID ;
        public List<Transsocket> SocketList;
        
    }
    
    [SerializeField]
    protected List<Trigger> TriggerList;
    protected Dictionary<string,bool> FadeFlags = new();
    quaternion defaultRotation;
    // Start is called before the first frame update
    void OnEnable()
    {   
        for (int i = 0; i < TriggerList.Count; i++) {
            if (TriggerList[i].ID == "") {
                Trigger trig = TriggerList[i];
                trig.ID = "OnOff";
                TriggerList[i] = trig;
            }
        }
        defaultRotation = this.transform.rotation;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
    // set on
    public void SetTrigger(string name,float percent){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        
        
        // set float 
        foreach(Transsocket socket in trigger.SocketList){
            // set all float value
            foreach(Vec3ParaSet Vec3set in socket.Vec3ParaList){
                switch(Vec3set.mode){
                    case(TrasnsMode.POSITION):
                        socket.obj.transform.localPosition = Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent);
                        break;
                    

                    case(TrasnsMode.ROTATION):
                        socket.obj.transform.localRotation = Quaternion.Euler(Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent));
                        //renew default
                        defaultRotation = Quaternion.Euler(Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent));
                        break;
                    case(TrasnsMode.SCALE):

                        socket.obj.transform.localScale = Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent);
                        break;
                    case(TrasnsMode.VELOCITY):
                        try{
                            socket.obj.GetComponent<Rigidbody>().velocity = Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent);
                        }
                        catch(System.Exception e){
                            Debug.LogError(e);
                        }
                        break;
                    case(TrasnsMode.ROTATEVELOCITY):
                     try{
                            socket.obj.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent);
                            socket.obj.transform.rotation = defaultRotation;
                        }
                        catch(System.Exception e){
                            Debug.LogError(e);
                        }
                        break;
                }
            }
           
        }
    }

    public void SetTriggerFade(string name ,float percent, float duration){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        
         foreach(Transsocket socket in trigger.SocketList){
            // set all float value
            foreach(Vec3ParaSet Vec3set in socket.Vec3ParaList){
                Vector3 targetVec = Vector3.Lerp(Vec3set.vec01,Vec3set.vec02,clamped_percent);
                StartCoroutine(SetVec3Fade(socket.obj,Vec3set.mode,targetVec ,duration));
            }
           
        }

        }

    public IEnumerator SetVec3Fade(GameObject obj,TrasnsMode mode, Vector3 target, float duration){

        
        string key = "Vec3" + obj.name + mode;
        // if first time, register
        if(!FadeFlags.ContainsKey(key)){
                FadeFlags.Add(key,false);
        }
        // if is fading
        if(FadeFlags[key]){
                yield break;
        }
        // fade processing 
        FadeFlags[key]= true;
        //start fade
        float timer = 0.0f;
        switch(mode){
            case(TrasnsMode.POSITION):
                Vector3 initPost = obj.transform.localPosition;   
                while(timer <= duration){
                    obj.transform.localPosition = Vector3.Lerp(initPost,target,timer/duration);
                    timer+=Time.deltaTime;
                    yield return null;
                }
                obj.transform.localPosition = target ;
                FadeFlags[key]= false;
                yield return null;

            break;
                    
            case(TrasnsMode.ROTATION):
                quaternion initRot = obj.transform.localRotation;   
                while(timer <= duration){
                    obj.transform.localRotation = Quaternion.Lerp(initRot,Quaternion.Euler(target),timer/duration) ;
                    timer+=Time.deltaTime;
                    yield return null;
                }
                obj.transform.localRotation = Quaternion.Euler(target);
                //renew default
                defaultRotation = Quaternion.Euler(target);
                FadeFlags[key]= false;
                yield return null;        
            break;
            case(TrasnsMode.SCALE):
                Vector3 initScale = obj.transform.localScale;   
                while(timer <= duration){
                    obj.transform.localScale = Vector3.Lerp(initScale,target,timer/duration);
                    timer+=Time.deltaTime;
                    yield return null;
                }
                obj.transform.localScale = target ;
                FadeFlags[key]= false;
                yield return null;
                       
            break;
            case(TrasnsMode.VELOCITY):
                if(obj.GetComponent<Rigidbody>()!=null){
                    Vector3 initVelocity = obj.GetComponent<Rigidbody>().velocity;   
                    while(timer <= duration){
                        obj.GetComponent<Rigidbody>().velocity = Vector3.Lerp(initVelocity,target,timer/duration);
                        timer+=Time.deltaTime;
                        yield return null;
                    }
                    obj.GetComponent<Rigidbody>().velocity = target ;
                    FadeFlags[key]= false;
                    yield return null;
                }
                else{
                    Debug.LogError(obj.name+" do not have rigidbody!");
                }
                
            break;
            case(TrasnsMode.ROTATEVELOCITY):
                if(obj.GetComponent<Rigidbody>()!=null){
                    Vector3 initVelocity = obj.GetComponent<Rigidbody>().angularVelocity;   
                    while(timer <= duration){
                        obj.GetComponent<Rigidbody>().angularVelocity = Vector3.Lerp(initVelocity,target,timer/duration);
                        timer+=Time.deltaTime;
                        yield return null;
                    }
                    obj.GetComponent<Rigidbody>().angularVelocity = target ;
                    // fade tp begining 
                    quaternion currentRot = obj.transform.rotation;
                    timer = 0;
                    if(obj.GetComponent<Rigidbody>().angularVelocity == new Vector3(0,0,0)){
                        while(timer <= duration/4){
                        obj.transform.rotation = Quaternion.Lerp(currentRot,defaultRotation,timer/(duration/2));
                        timer+=Time.deltaTime;
                        yield return null;
                    }
                    obj.transform.rotation = defaultRotation;
                    }
                    FadeFlags[key]= false;
                    yield return null;
                }
                else{
                    Debug.LogError(obj.name+" do not have rigidbody!");
                }
            break;
        }
        
    }

}
