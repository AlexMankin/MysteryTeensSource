using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAdvanceIcon : SummonableActor {

    //VARIABLES

    //CONSTANTS

    //EVENTS

    //METHODS

    //PROPERTIES
    protected override bool ShouldBeVisible => MessageBox.ReadyToAdvance;
}
