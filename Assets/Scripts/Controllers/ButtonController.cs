using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public ButtonTypes buttonType;

    public void ButtonClicked()
    {
        switch (buttonType)
        {
            case ButtonTypes.UndoButton:
                EventManager.Undo();
                break;
            case ButtonTypes.SolutionButton:
                EventManager.SolutionButtonClicked();
                break;
            case ButtonTypes.Reset:
                EventManager.Reset();
                break;
            
        }
    }

    public void LevelButtonClicked(int index)
    {
        EventManager.NextLevelButton(index);
    }
}
