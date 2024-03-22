using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MatController : MonoBehaviour,IController
{   
    
    
    [System.Serializable]
    public struct Matsocket{
    
        public Material Mat;
        public List<FloatParaSet> flaotParaList;
        public List<ColorParaSet> colorParaList;
        public List<BoolParaSet> booParalList;

    }
    [System.Serializable]
    public struct Trigger{
        public string ID;
        public List<Matsocket> SocketList;
        
    }
    
    [SerializeField]
    protected List<Trigger> TriggerList;

    protected Dictionary<string,bool>FadeFlags = new();
    // get instances
    protected List<GameObject> ObjTree =new();
 
    protected List<MatInstSet> MatInsSetList = new();
    // Start is called before the first frame update
    void Start()
    {
        // init 
        TraAddObjTree(this.gameObject);
        //Debug.Log("Objtree:" + ObjTree.Count);
        // cloning materials
        FillMatInstance();
        //Debug.Log("InstMat:" + MatInsSetList.Count);

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
        foreach(Matsocket socket in trigger.SocketList){

            List<MatInstSet> MatSets = MatInsSetList.FindAll(MatSets =>MatSets.Mat.name.StartsWith(socket.Mat.name));
            // set all float value
            foreach(MatInstSet Set in  MatSets){
                foreach(FloatParaSet Fset in socket.flaotParaList){
                    Set.Mat.SetFloat(Fset.para,Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent));
                }
                // set all color value
                foreach(ColorParaSet Cset in socket.colorParaList){
                    Set.Mat.SetColor(Cset.para,Color.Lerp(Cset.color1,Cset.color2,clamped_percent));
                }
                //set all bool values
                foreach(BoolParaSet Bset in socket.booParalList){
                    if(clamped_percent>Bset.threshold){
                        Set.Mat.SetFloat(Bset.para,1);
                    }
                    else{
                        Set.Mat.SetFloat(Bset.para,0);
                    }
                }
            }
            
        }
    }

    public void SetTriggerFade(string name ,float percent, float duration){
        Trigger trigger = TriggerList.Find(trigger => trigger.ID == name);
        float clamped_percent = Mathf.Clamp(percent,0,1);
        foreach(Matsocket socket in trigger.SocketList){

            List<MatInstSet> MatSets = MatInsSetList.FindAll(MatSets =>MatSets.Mat.name.StartsWith(socket.Mat.name));
            foreach(MatInstSet Set in  MatSets){
                // set all float value
                foreach(FloatParaSet Fset in socket.flaotParaList){
                    
                    StartCoroutine(SetFloatFade(Set,Fset.para,Mathf.Lerp(Fset.val1,Fset.val2,clamped_percent),duration));
                }
                // set all color value
                foreach(ColorParaSet Cset in socket.colorParaList){
                    StartCoroutine(SetColorFade(Set,Cset.para,Color.Lerp(Cset.color1,Cset.color2,clamped_percent),duration));
                }
                //set all bool values
                foreach(BoolParaSet Bset in socket.booParalList){
                    StartCoroutine(SetBoolFade(Set,Bset.para,percent>=Bset.threshold,Bset.threshold,duration));
                }
            }
            
        }

    }

    public IEnumerator SetFloatFade(MatInstSet Set, string name, float target, float duration){

        if(Set.Mat!=null){
            string key = "MatFloat: Mat:" + Set.Mat.name +"Obj:  " +Set.Obj_name +"Para name:  " +name;
            //Debug.Log("Start Mat flaot fade: " + key);
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
            float initValue = Set.Mat.GetFloat(name);
            float timer = 0.0f;
            while(timer <= duration){
                Set.Mat.SetFloat(name,Mathf.Lerp(initValue,target,timer/duration));
                timer+=Time.deltaTime;
                yield return null;
            }
            Set.Mat.SetFloat(name, target);
            FadeFlags[key]= false;
             //Debug.Log("End Mat float fade: " + key);
            yield return null;
        }
        else{
            Debug.Log("cant find Mat with "+ name + " para");
        }
    }
    public IEnumerator SetColorFade(MatInstSet Set, string name, Color target, float duration){
        
        if(Set.Mat!=null){
                string key = "MatColor: Mat:" + Set.Mat.name +"Obj:  " +Set.Obj_name +"Para name:  " +name;
                //Debug.Log("Start Mat color fade: " + key);

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
                Color initColor = Set.Mat.GetColor(name);
                while (timer < duration)
                {
                    Color currentColor = Color.Lerp(initColor, target, timer / duration);
                    Set.Mat.SetColor(name,currentColor);
                    timer += Time.deltaTime;
                    yield return null;
                }
                // Ensure the final color is set to 'endColor'
                Set.Mat.SetColor(name, target);

                FadeFlags[key] = false;
                //Debug.Log("End Mat color fade: " + key);
                }
        else{
            Debug.Log("cant find Mat with "+ name + " para");
        }
        
        
    }
    // should be problem of key
    public IEnumerator SetBoolFade(MatInstSet Set, string name, bool target,float threshold, float duration){
        if(Set.Mat!=null){
                string key = "MatBool: Mat:" + Set.Mat.name +"Obj:  " +Set.Obj_name +"Para name:  " +name;
                Debug.Log("Start Mat bool fade: " + key);

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
                        Set.Mat.SetFloat(name,target?1.0f:0.0f);
                    }
                    yield return null;
                }
                // Ensure the final color is set to 'endColor'
                Set.Mat.SetFloat(name,target?1.0f:0.0f);

                FadeFlags[key] = false;
                Debug.Log("End Mat Fade bool: " + key);}
        else{
            Debug.Log("cant find Mat with "+ name + " para");
        }
    }
    void TraAddObjTree(GameObject Obj){
        if(!ObjTree.Contains(Obj)){
            ObjTree.Add(Obj);
        }
        foreach(Transform child in Obj.transform){
            TraAddObjTree(child.gameObject);
        }
    }
    
    void FillMatInstance(){
        foreach(GameObject obj in ObjTree){
            if (obj.GetComponent<VisualEffect>() != null) {
            continue;
            }
            Renderer renderer = obj.GetComponent<Renderer>();
            if(renderer!=null){
                // applying clone 
                Material[] InstMaterials = renderer.materials;

                for (int i = 0; i < InstMaterials.Length; i++)
                {   
                    // originMat is before change, which is the instone
                    Material originalMat = InstMaterials[i];
                    foreach(Trigger trigger in TriggerList){
                        foreach(Matsocket socket in trigger.SocketList){
                            Material mat = socket.Mat;
                            if (originalMat.name.StartsWith(mat.name)){
                                // Clone the original material and perform other operations
                                Material InsMat = new Material(originalMat);
                                InstMaterials[i] = InsMat;
                                MatInstSet Set ;
                                Set.Mat= InsMat;
                                Set.Obj_name = obj.name;
                                MatInsSetList.Add(Set);

                                // Optional: Log the matching material name
                                //Debug.Log("Matching material name: " + mat.name);
                                break; // Exit the loop once a match is found
                            }
                        }
                    }
                }

                renderer.materials = InstMaterials;
            }
            
        }
    }
}
