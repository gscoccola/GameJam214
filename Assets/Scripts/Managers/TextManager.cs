using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : Singleton<TextManager>
{
	[SerializeField] private bool _debug;
    [HideInInspector] public bool isInCutscene;
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
	}
	List<CutsceneFrame> scenes;
	[SerializeField]
	TextAsset textfile;

	private Dictionary<string, int> sceneAliases;
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
	
	string[] portraitNamesArr = {"mc","an1","an2","hu1","hu2","hu3"};
	Dictionary<string,int> portraitNames;

	[SerializeField]
	GameObject cutsceneUI;



    void Start()
    {
		portraitNames = new();
		for(int i = 0; i < portraitNamesArr.Length; i++) {
			portraitNames.Add(portraitNamesArr[i],i);
		}
		string input = textfile.text;
		scenes = new();
		sceneAliases = new();
		string[] lines = input.Split("\r\n");
		int state = 0;
		
		string name = "", text = "", setFlag = "", sceneAlias = "";
		List<string> actors = new();
		foreach(string s in lines) {
			if (_debug) {
				Debug.Log("State: " + state);
				Debug.Log("Line: " + s);
			}
			if(String.IsNullOrWhiteSpace(s))
			{
				state = 0;
				//dump into new cutsceneframe
				CutsceneFrame newScene = new();
				newScene.text = text;
				newScene.name = name;
				newScene.id = scenes.Count;
				foreach(string a in actors) {
					newScene.actors.Add(a);
				}
				newScene.setFlag = setFlag;
				newScene.sceneAlias = sceneAlias;
				sceneAliases.Add(sceneAlias, scenes.Count);
				scenes.Add(newScene);
                if (_debug) Debug.Log("Add scene at position " + (scenes.Count - 1) + " with alias " + sceneAlias);
				name = "";
				text = "";
				setFlag = "";
				sceneAlias = "";
                actors = new();
                continue;
			}
			else if(s[0] == '#') {
				//add a sceneAlias
				sceneAlias = s.Substring(1);
				
			}
			else if(s[0] == '@') {
				//add an actor
				actors.Add(s.Substring(1));
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
		//RenderScene(0);
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
	
	void SetFlag(string f) {
		if (flags == null) {
			flags = new();
		}
		flags[f] = true;
	}
	
	void RenderScene(string sceneAlias) {
		if(sceneAliases.ContainsKey(sceneAlias)) {
			RenderScene(sceneAliases[sceneAlias]);
		}
		else {
			return;
		}
	}

	private int currentScene = 0;

    public void NextScene()
	{
		currentScene++;
		RenderScene(currentScene);
	}
	
	void EndScene()
	{
        cutsceneUI.SetActive(false);
        isInCutscene = false;
    }
	
	public void RenderScene(int id) {
        if (_debug) Debug.Log("Attempt to enter scene " + id);
		if(id == -1) {
			return;
		}
		cutsceneUI.SetActive(true);
        isInCutscene = true;
        CutsceneFrame cur = scenes[id];
		//set the name
		if(cur.name == "fin") {
			//close the textbox
			EndScene();

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
			int exactid = -1;
			if(Int32.TryParse(goingto, out exactid)){
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
			leftPortraitSlot.SetActive(false);
            rightPortraitSlot.SetActive(false);
        }
		else if(cur.actors.Count == 1) {
            //make right invisible, set left
            leftPortraitSlot.SetActive(true);
            rightPortraitSlot.SetActive(false);
			leftPortraitSlot.GetComponent<Image>().sprite = portraits[portraitNames[cur.actors[0]]].GetComponent<SpriteRenderer>().sprite;
        }
		else if(cur.actors.Count == 2) {
            //make both visible and set both
            leftPortraitSlot.SetActive(true);
            rightPortraitSlot.SetActive(true);
            leftPortraitSlot.GetComponent<Image>().sprite = portraits[portraitNames[cur.actors[0]]].GetComponent<SpriteRenderer>().sprite;
            rightPortraitSlot.GetComponent<Image>().sprite = portraits[portraitNames[cur.actors[1]]].GetComponent<SpriteRenderer>().sprite;
        }
		
		//set the flag
		SetFlag(cur.setFlag);
	}

}