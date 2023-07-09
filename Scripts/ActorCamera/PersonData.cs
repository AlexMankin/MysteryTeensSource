using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPerson", menuName = "Mystery Teens/ActorData")]
public class PersonData : ScriptableObject {

    //VARIABLES
    public PersonAnimationPackage animations;
    public Color colorDark, colorMid, colorLight;
    public TextData nameData;
	public AllPersons personName;
    
	public AnimData[] portraits;
   


    //CONSTANTS

    //EVENTS

    //METHODS
    public AnimData GetPortrait(int index) {
        if (index < 0 || index >= portraits.Length)
            return portraits.Length > 0 ? portraits[0] : null;

        return portraits[index];
    }

    public string Nametag => nameData?.GetText(sv.Data.getPersonNameIndex(personName));

}

[System.Serializable]
public struct BattleAnimations {
    public AnimData hurt, ko, active;
    public Sprite turnOrderTab, throwdownImage;
    public AnimData eyePortrait;
}