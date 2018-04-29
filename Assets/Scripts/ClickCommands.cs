/* Created by Sharon Shen 2018
 * Define how every keywords do when they are selected*/

using System;
using System.Collections.Generic;
using UnityEngine;

public class ClickCommands : MonoBehaviour
{
    List<Color> colors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow, Color.cyan };
    private void Start()
    {
        GameObject.Find("normalscale").GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
    }
    // Called by GazeGestureManager when the user performs a Select gesture
    void OnSelect()
    {
        // Sharon Shen 03 2018
        // click on save button, note that mesh could be saved if only the mesh is running, 
        //i.e., no room file is displayed
        if (this.gameObject.name == "Cube_save" && GameObject.Find("file_show") == null)
        {
            Debug.Log("cube state: cube to clicked");
            this.gameObject.name = "Cube_save_clicked";
            //this.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
        }
        // click on room file to display room model 
        else if (this.gameObject.name == "file" )//&& GameObject.Find("file_show")==null)
        {
            Debug.Log("cube state: click on file: ");
            this.gameObject.name = "file_clicked";
            int number_end = this.transform.GetChild(0).GetComponent<TextMesh>().text.IndexOf(")");
            int color_count = Convert.ToInt32(this.transform.GetChild(0).GetComponent<TextMesh>().text.Substring(1, number_end-1));
            this.gameObject.GetComponent<Renderer>().material.color = colors[(color_count-1)%5];

        }
        // click on displaying room file to turn it off
        else if (this.gameObject.name == "file_show")
        {
            Debug.Log("cube state: release file");
            this.gameObject.name = "file_hide";
            this.gameObject.GetComponent<Renderer>().material.color = new Color(0.5f, 0.0f, 0.5f, 0.0f);

        }
        // click on "mini" button to show miniature of the model
        else if (this.gameObject.name == "miniature")
        {
            this.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
            GameObject.Find("normalscale").GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);
        }
        // click on "normal" button to show normal size model
        else if (this.gameObject.name == "normalscale")
        {
            this.gameObject.GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f, 0.0f);
            GameObject.Find("miniature").GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f, 0.0f);
        }
        // click on menu bar to stop menu tagging along, i.e., let menu stay at where it was
        else if (this.gameObject.name == "menu")
        {
            this.gameObject.name = "menu_clicked";
        }
    }
}