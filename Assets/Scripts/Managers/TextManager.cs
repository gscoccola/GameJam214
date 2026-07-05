using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
	class CutsceneFrame {
		public int id;
		public string sceneAlias;
		public string name;
		public string text;
		public string setFlag;
		public List<string> actors;
		public CutsceneFrame() {
			actors = new();
			id = -1;
			sceneAlias = "";
			name = "";
			text = "";
			setFlag = "";
		}
	List<CutsceneFrame> scenes;
	string input = """
	#firstscene
	@mc
	Main Character
	I'm the main character and I'm gonna do main character things
	sawfirstcutscene
	
	Main Character
	By not specifying an actor on this scene it should still be the mc, but if you were to jump straight to this frame, it would be blank.
	
	#when a scene has a name text of 'condition' it will evaluate the conditional and immediately proceed (flag ifset else)
	condition
	sawfirstcutscene endoffirstscene secondscene
	
	#endoffirstscene
	fin
	Reaching a scene with name text of 'fin' will end the scene and return to gameplay.
	
	#secondscene
	Main Character
	This would mean I somehow entered on frame 2
	
	"""

	private Dictionary<string, int> sceneAliases;
	private Dictionary<
	[SerializeField]
	GameObject nameText;
	[SerializeField]
	GameObject textText;
	[SerializeField]
	GameObject leftPortraitSlot;
	[SerializeField]
	GameObject rightPortraitSlot;
	
	[SerializeField]
	List<GameObject> portraits;
	//put them in order of androids first, then humans
	
	const string[] portraitNamesArr = {"mc","an1","an2","an3","hu1","hu2","hu3"};
	Dictionary<string,int> portraitNames;
	


    // Start is called before the first frame update
    void Start()
    {
		portraitNames = new();
		for(int i = 0; i < portraitNames.Length; i++) {
			portraitNames.Add(portraitNamesArr,i);
		}
		
		scenes = new();
		sceneAliases = new();
		string[] lines = input.Split("\n\r");
		int state = 0;
		
		string name = "", text = "", setFlag = "", sceneAlias = "";
		List<string> actors = new();
		foreach(string s in lines) {
			if(s == "")
			{
				state = 0;
				//dump into new cutsceneframe
				CutsceneFrame newScene = new();
				newScene.text = text;
				newScene.name = name;
				newScene.id = scenes.Count();
				foreach(string a in actors) {
					newScene.actors.Append(a);
				}
				newScene.setFlag = setFlag;
				newScene.sceneAlias = sceneAlias;
				sceneAliases.Add(sceneAlias, scenes.Count());
				scenes.Append(newScene);
				continue;
			}
			else if(s.CharAt(0) == '#') {
				//add a sceneAlias
				sceneAlias = s.Substring(1);
				
			}
			else if(s.CharAt(0) == '@') {
				//add an actor
				actors.Append(s.Substring(1));
				//if actors == {"none"}, clear it when rendering
			}	
			else {
				switch(state) {
					case 0:
						//set name
						name = s;
						state++;
						break;
					case 1:
						//set text
						text = s;
						state++;
						break;
					case 2:
						//set flag
						setFlag = s;
						break;
				}
			}
			
		}
    }
	
	
	Dictionary<string,bool> flags = null;
	bool GetFlag(string f) {
		if(flags == null)
			return false;
		if(flags.ContainsKey(f)) {
			return flags[f];
		}
		return false;
	}
	
	bool SetFlag(string f) {
		if (flags == null) {
			flags = new();
		}
		flags[f] = true;
	}
	
	void RenderScene(string sceneAlias) {
		if(sceneAliases.ContainsKey(sceneAlias) {
			RenderScene(sceneAliases[sceneAlias]);
		}
		else {
			return;
		}
	}
	
	
	
	
	void RenderScene(int id) {
		if(id == -1) {
			return;
		}
		CutsceneFrame cur = scenes[id];
		//set the name
		if(cur.name == "fin") {
			//close the textbox
			return;
		}
		else if(cur.name == "conditional") {
			string[] parameters = cur.text.Split(' ');
			string cond = parameters[0];
			string ifso = parameters[1];
			string ifnot = parameters[2];
			string goingto = ifnot;
			if(GetFlag(cond)) {
				goingto = ifso;
			}
			//evaluate conditional and then RenderScene the correct choice
			int exactid;
			if(Int32.TryParse(goingto,exactid)){
				RenderScene(exactid);
			}
			else {
				RenderScene(goingto);
			}
			return;
		}
		nameText.GetComponent<TextMeshProUGUI>().text = cur.name;
		
		//set the text
		textText.GetComponent<TextMeshProUGUI>().text = cur.text;
		
		//display the actors
		if(cur.actors.Contains("none")) {
			//make them invisible
		}
		else if(cur.actors.Count == 1) {
			//make right invisible, set left
		}
		else if(cur.actors.Count == 2) {
			//make both visible and set both
		}
		
		//set the flag
		SetFlag(cur.setFlag);
	}
	
	
	

    // Update is called once per frame
    void Update()
    {

    }
}