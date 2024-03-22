using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightController : MonoBehaviour,IController
{   
    [System.Serializable]
    public struct Lightsocket{
        
        public Light light;
        public List<LightFloatParaSet> flaotParaList;
        public List<LightColorParaSet> colorParaList;
        public LightBoolParaSet needBool;

    }
    [System.Serializable]
    public struct Trigger{
        public string ID ;
        public List<Lightsocket> SocketList;
        
    }
    
    [SerializeField]
    protected List<Trigger> TriggerList;

    protected Dictionary<string,bool>FadeFlags = new();
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

    }       

    // Update is called once per frame
    void Update()
    {
        
    }

    
    // set on
    public void SetTrigger(string name ,float percent){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        
        // set float 
        foreach(Lightsocket socket in trigger.SocketList){
            // set all float value
            foreach(LightFloatParaSet Fset in socket.flaotParaList){
                switch(Fset.para){
                    case( LightPara.INTENSITY):
                        socket.light.intensity = Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent);break;
                    case( LightPara.RANGE):
                        socket.light.range = Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent);break;
                }
                
            }
            // set all color value
            foreach(LightColorParaSet Cset in socket.colorParaList){
                socket.light.color = Color.Lerp(Cset.color1,Cset.color2,clamped_percent);
            }
            //set all bool values
            if(socket.needBool.need){
                if(percent>=socket.needBool.threshold){
                    socket.light.enabled = true;
                }
                else{
                    socket.light.enabled = false;
                }
            }
        }
    }

    public void SetTriggerFade(string name ,float percent, float duration){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        foreach(Lightsocket socket in trigger.SocketList){
            // set all float value
            foreach(LightFloatParaSet Fset in socket.flaotParaList){
                StartCoroutine(SetFloatFade(socket.light,Fset.para,Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent),duration));
            }
            // set all color value
            foreach(LightColorParaSet Cset in socket.colorParaList){
                StartCoroutine(SetColorFade(socket.light,Color.Lerp(Cset.color1,Cset.color2,clamped_percent),duration));
            }
            // set bool
            
            if(socket.needBool.need){
                bool target = percent >= socket.needBool.threshold;
                StartCoroutine(SetBoolFade(socket.light,target,socket.needBool.threshold,duration));
            }
           
        }

        }

    public IEnumerator SetFloatFade(Light light, LightPara para, float target, float duration){

        if(light!=null){
            string key = "LightFloat" + light.gameObject.name + name + para;
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
            float initIntensity = light.intensity;
            float initRange = light.range;
            while(timer <= duration){
                 
                switch(para){
                    case( LightPara.INTENSITY):
                        light.intensity = Mathf.Lerp(initIntensity,target,timer/duration);Debug.Log( "current intensity : " + light.intensity +"" );break;
                        
                    case( LightPara.RANGE):
                        light.range = Mathf.Lerp(initRange,target,timer/duration);break;
                }
                timer += Time.deltaTime;
               yield return null;
            }
            switch(para){
                    case( LightPara.INTENSITY):
                        light.intensity =target;break;
                    case( LightPara.RANGE):
                        light.range = target;break;
            }
            FadeFlags[key]= false;
            
        }
        else{
            Debug.Log("cant find light");
        }
    }
    public IEnumerator SetColorFade(Light light, Color target, float duration){
        
        if(light!=null){
                string key = "LightColor" + light.gameObject.name;
                Debug.Log("Start light color fade: " + key);

                // If first time, register
                if (!FadeFlags.ContainsKey(key))
                {
                    FadeFlags.Add(key, false);
                }
                // If is fading
                if (FadeFlags[key])
                {
                    yield break;
                }
                // Fade processing
                FadeFlags[key] = true;

                // Start fade
                float timer = 0.0f;
                Color initColor= light.color;
                while (timer < duration)
                {
                    Color currentColor = Color.Lerp(initColor, target, timer / duration);
                    light.color = currentColor;
                    timer += Time.deltaTime;
                    yield return null;
                }
                // Ensure the final color is set to 'endColor'
               light.color = target;

                FadeFlags[key] = false;
                Debug.Log("End light color fade: " + key);}
        else{
            Debug.Log("cant find light ");
        }
        
        
    }
    public IEnumerator SetBoolFade(Light light, bool target ,float threshold, float duration){
        if(light!=null){
                string key = "LightBool  " + light.gameObject.name ;
                Debug.Log("Start Light bool fade: " + key);

                // If first time, register
                if (!FadeFlags.ContainsKey(key))
                {
                    FadeFlags.Add(key, false);
                }
                // If is fading
                if (FadeFlags[key])
                {
                    yield break;
                }
                // Fade processing
                FadeFlags[key] = true;

                // Start fade
                float timer = 0.0f;
                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    if(timer >= threshold){
                        light.enabled = target;
                    }
                    yield return null;
                }
                // Ensure the final color is set to 'endColor'
                light.enabled = target;

                FadeFlags[key] = false;
                Debug.Log("End vfx color bool: " + key);}
        else{
            Debug.Log("cant find VFX with "+ name + " para");
        }
    }

}
