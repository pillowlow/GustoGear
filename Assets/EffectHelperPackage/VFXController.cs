using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;



public class VFXController : MonoBehaviour,IController
{   

    
    [System.Serializable]
    public struct VFXsocket{
    
        public VisualEffect VFX;
        public List<FloatParaSet> flaotParaList;
        public List<ColorParaSet> colorParaList;
        public List<BoolParaSet> booParalList;

    }
    [System.Serializable]
    public struct Trigger{
        public string ID ;
        public List<VFXsocket> SocketList;
        
    }
    
    [SerializeField]
    protected List<Trigger> TriggerList;
    protected Dictionary<string,bool> FadeFlags = new();
    // Start is called before the first frame update

    // setting paras to control? 
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
    public void SetTrigger(string name,float percent){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        
        // set float 
        foreach(VFXsocket socket in trigger.SocketList){
            // set all float value
            foreach(FloatParaSet Fset in socket.flaotParaList){
                socket.VFX.SetFloat(Fset.para,Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent));
            }
            // set all color value
            foreach(ColorParaSet Cset in socket.colorParaList){
                socket.VFX.SetVector4(Cset.para,Color.Lerp(Cset.color1,Cset.color2,clamped_percent));
            }
            //set all bool values
            foreach(BoolParaSet Bset in socket.booParalList){
                if(clamped_percent>Bset.threshold){
                    socket.VFX.SetBool(Bset.para,true);
                }
                else{
                    socket.VFX.SetBool(Bset.para,false);
                }
                    
            }
        }
    }

    public void SetTriggerFade(string name ,float percent, float duration){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        foreach(VFXsocket socket in trigger.SocketList){
            // set all float value
            foreach(FloatParaSet Fset in socket.flaotParaList){
                
                StartCoroutine(SetFloatFade(socket.VFX,Fset.para,Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent),duration));
            }
            // set all color value
            foreach(ColorParaSet Cset in socket.colorParaList){
                StartCoroutine(SetColorFade(socket.VFX,Cset.para,Color.Lerp(Cset.color1,Cset.color2,clamped_percent),duration));
            }
            //set all bool values
            foreach(BoolParaSet Bset in socket.booParalList){
                StartCoroutine(SetBoolFade(socket.VFX,Bset.para,percent>=Bset.threshold,Bset.threshold,duration));
            }
        }

        }

    public IEnumerator SetFloatFade(VisualEffect VFX, string name, float target, float duration){

        if(VFX!=null){
            string key = "VFXFloat" + VFX.gameObject.name + name;
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
            float initvalue = VFX.GetFloat(name);
            while(timer <= duration){
                VFX.SetFloat(name,Mathf.Lerp(initvalue,target,timer/duration));
                timer+=Time.deltaTime;
                yield return null;
            }
            VFX.SetFloat(name, target);
            FadeFlags[key]= false;
            yield return null;
        }
        else{
            Debug.Log("cant find VFX with "+ name + " para");
        }
    }
    public IEnumerator SetColorFade(VisualEffect VFX, string name, Color target, float duration){
        
        if(VFX!=null){
                string key = "VFXColor  " + VFX.gameObject.name + "  "+ name;
                //Debug.Log("Start Vfx color fade: " + key);

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
                Color initcolor = VFX.GetVector4(name);
                while (timer < duration)
                {
                    Color currentColor = Color.Lerp(initcolor, target, timer / duration);
                    VFX.SetVector4(name, HdrColor2Vector4(currentColor));
                    timer += Time.deltaTime;
                    yield return null;
                }
                // Ensure the final color is set to 'endColor'
                VFX.SetVector4(name, target);

                FadeFlags[key] = false;
                //Debug.Log("End vfx color fade: " + key);
                }
        else{
            Debug.Log("cant find VFX with "+ name + " para");
        }
        
        
    }
    public IEnumerator SetBoolFade(VisualEffect VFX, string name, bool target,float threshold, float duration){
        if(VFX!=null){
                string key = "VFXBool  " + VFX.gameObject.name + "  "+ name;
                //Debug.Log("Start Vfx bool fade: " + key);

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
                        VFX.SetBool(name,target);
                    }
                    yield return null;
                }
                // Ensure the final color is set to 'endColor'
                VFX.SetBool(name, target);

                FadeFlags[key] = false;
                //Debug.Log("End vfx bool fade: " + key);
                }
        else{
            Debug.Log("cant find VFX with "+ name + " para");
        }
    }

    protected Vector4 HdrColor2Vector4(Color hdrColor)
    {
        Vector4 colorVector = new Vector4(hdrColor.r, hdrColor.g, hdrColor.b, hdrColor.a);
        return colorVector;
    }

    

}
