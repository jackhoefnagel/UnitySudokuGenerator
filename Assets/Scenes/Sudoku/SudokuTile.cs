using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SudokuTile : MonoBehaviour {

    public Text valueText;
    public Image buttonImage;

    public void SetValue(int value){
        valueText.text = value.ToString();
    }

    public void ButtonClicked(){
        DisplayValue();
    }
	
    void DisplayValue(){
        Debug.Log(valueText.text);
    }

    public void Highlight(){
        buttonImage.color = Color.red;
    }

    public void Unhighlight()
    {
        buttonImage.color = Color.white;
    }
}
