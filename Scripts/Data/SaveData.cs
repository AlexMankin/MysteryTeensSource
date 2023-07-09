using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct SaveData {

    //000. System / Metadata
    //System / Metadata
    public char[] catName;
    public string CatName {
        get { return new string(catName); }
        set { catName = new char[value.Length];

            for (int i = 0; i < value.Length; ++i)
                catName[i] = value[i];
        }
    }

    public float
        Sys_Playtime,
        Sys_PlayerPositionX,
        Sys_PlayerPositionY;

    public bool
        Sys_LoadingSaveData,
        Sys_CurrentlyInGame,
        Sys_DisableMeow;

    public bool
        completed1cLB,
        completed2aBR,
        completed2bGD,
        completed3aKT,
        completed3bGB,
        completed3cGL,
        completed3dBA;

    public int[]
        NametagIncrement;

    public AllPersons
        CurrentPlayer;

    public int
        previousSceneIndex,
        currentSceneIndex;

    public DateTime
        Sys_DateOfSave;

    public List<ActionVariablePackage> VariablePackages;
    public List<PersonIndex> PersonNameIndices;

    //Mystery Teens - List of inspected art pieces
    public List<ArtPiece> ArtInspectedList;

    //METHODS

    public ActionVariablePackage GetPackage(Type scriptType) {
        if (scriptType == null) {
            Debug.LogWarning("Reading package for null script type.");
            return new ActionVariablePackage();
        }

        foreach (ActionVariablePackage P in VariablePackages) {
            if (P.ScriptType != null ? P.ScriptType.Equals(scriptType) : false) {
                return P;
            }
        }

        ActionVariablePackage newPackage = new ActionVariablePackage();
        newPackage.ScriptType = scriptType;
        newPackage.Flags = new bool[ActionVariablePackage.NUM_FLAGS];
        VariablePackages.Add(newPackage);
        return newPackage;
    }

    public void UpdatePackage(Type scriptType, ActionVariablePackage newPackage) {
        int index = VariablePackages.IndexOf(newPackage);
        if (index >= 0) {
            VariablePackages[index] = newPackage;
        }
        else {
            VariablePackages.Add(newPackage);
        }
    }

    public void setPersonNameIndex(AllPersons P, int index) {
        PersonIndex pi = new PersonIndex();
        pi.person = P;
        pi.index = index;

        int indexInList = -1;
        for (int i = 0; i < PersonNameIndices.Count && indexInList < 0; ++i) {
            if (PersonNameIndices[i].person == P) {
                indexInList = i;
            }
        }

        if (indexInList < 0) {
            indexInList = PersonNameIndices.Count;
            PersonNameIndices.Add(pi);
        }

        PersonNameIndices[indexInList] = pi;
    }

    public int getPersonNameIndex(AllPersons P) {
        for (int i = 0; i < PersonNameIndices.Count; ++i) {
            if (PersonNameIndices[i].person == P)
                return PersonNameIndices[i].index;
        }

        setPersonNameIndex(P, 0);
        return 0;
    }

    //Mystery Teens
    public bool registerArtInspected(ArtPiece A) {
        if (!ArtInspectedList.Contains(A))
            ArtInspectedList.Add(A);

        return ArtComplete;
    }

    public bool ArtComplete => ArtInspectedList?.Count >= Enum.GetNames(typeof(ArtPiece)).Length;
}

[System.Serializable]
public struct ActionSelectData {
    public SceneData SceneID;
    public TxtMultiChoice OptionID;
}

[System.Serializable]
public struct ActionVariablePackage {
    public const int NUM_FLAGS = 5;
    public Type ScriptType;
    public int Phase;
    public bool[] Flags;

    public override bool Equals(object other) {
        if (other == null)
            return false;

        ActionVariablePackage otherAsPackage = (ActionVariablePackage)other;
        return other is ActionVariablePackage && otherAsPackage.ScriptType == ScriptType;
    }
}

[System.Serializable]
public struct PersonIndex {
    public AllPersons person;
    public int index;

    public override bool Equals(object other) {
        if (other == null)
            return false;

        if (!(other is PersonIndex))
            return false;

        PersonIndex otherPersonIndex = (PersonIndex)other;
        return person == otherPersonIndex.person;
    }

}
