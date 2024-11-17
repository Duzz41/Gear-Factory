using UnityEngine;
using UnityEngine.UI;

public class RotateWheel : MonoBehaviour
{
    public RectTransform wheel;
    public Button[] buttons;
    private int rotationStep = 5;

    private int currentRotationIndex = 0;

    void Start()
    {
        AlignWheelToButton(0);
        UpdateButtonStates(); 
    }

    public void RotateClockwise()
    {
        if (currentRotationIndex > 0)
        {
            currentRotationIndex--;
            RotateWheelAndButtons(rotationStep);
        }
    }

    public void RotateCounterClockwise()
    {
        if (currentRotationIndex < buttons.Length - 1)
        {
            currentRotationIndex++;
            RotateWheelAndButtons(-rotationStep);
        }
    }

    private void RotateWheelAndButtons(int angle)
    {
        wheel.Rotate(0, 0, angle);
        UpdateButtonStates();
    }

    private void AlignWheelToButton(int buttonIndex)
    {
        int targetRotation = -rotationStep * buttonIndex;
        wheel.localRotation = Quaternion.Euler(0, 0, targetRotation);
        currentRotationIndex = buttonIndex;
    }

    private void UpdateButtonStates()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == currentRotationIndex)
            {
                buttons[i].interactable = true;
                HighlightButton(buttons[i]);
            }
            else
            {
                buttons[i].interactable = false;
                ResetButtonAppearance(buttons[i]);
            }
        }
    }

    private void HighlightButton(Button button)
    {
        button.GetComponent<Image>().color = Color.yellow;
    }

    private void ResetButtonAppearance(Button button)
    {
        button.GetComponent<Image>().color = Color.white;
    }
}
