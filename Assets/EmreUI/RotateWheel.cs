using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.InputSystem;

public class RotateWheel : MonoBehaviour
{
    public RectTransform wheel;
    public Button[] buttons;
    private int rotationStep = 45;
    [SerializeField] MainMenuUI mainMenuUI;
    public UnityEvent<Button> OnButtonSelected; // Seçili butonu dışarıya bildiren UnityEvent

    [SerializeField] private int currentRotationIndex = 0;
    private bool isRotating = false;

    void Start()
    {
        AlignWheelToButton(0);
        UpdateButtonStates();
    }

    public void RotateClockwise()
    {
        if (!isRotating)
        {
            currentRotationIndex = (currentRotationIndex - 1 + buttons.Length) % buttons.Length;
            RotateWheelAndButtons(-rotationStep);
        }
    }

    public void RotateCounterClockwise()
    {
        if (!isRotating)
        {
            currentRotationIndex = (currentRotationIndex + 1) % buttons.Length;
            RotateWheelAndButtons(rotationStep);
        }
    }

    private void RotateWheelAndButtons(int angle)
    {
        if (isRotating == false)
        {
            isRotating = true;
            Vector3 wheelRotate = wheel.localRotation.eulerAngles;
            wheel.DORotate(wheelRotate + new Vector3(0, 0, angle), 1.25f).OnComplete(() => { isRotating = false; });
            UpdateButtonStates();
        }
    }

    public void AlignWheelToButton(int buttonIndex)
    {
        int targetRotation = -rotationStep * buttonIndex;
        wheel.localRotation = Quaternion.Euler(0, 0, targetRotation);
    }

    public void UpdateButtonStates()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == currentRotationIndex)
            {
                buttons[i].interactable = true;
                HighlightButton(buttons[i]);
                Debug.Log(buttons[i].name);
                // UnityEvent ile seçili butonu bildir

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
        //OnButtonSelected.Invoke(button);
        button.Select();
        //  button.GetComponent<Image>().color = Color.yellow;
    }

    private void ResetButtonAppearance(Button button)
    {
        button.StopAllCoroutines();
        // button.GetComponent<Image>().color = Color.white;
    }
    #region Inputs
    public void Rotate(InputAction.CallbackContext context)
    {
        if (mainMenuUI.isMenu == false)
        {
            if (context.performed)
            {
                if (context.ReadValue<Vector2>().y > 0)
                {
                    RotateClockwise();
                }
                else if (context.ReadValue<Vector2>().y < 0)
                {
                    RotateCounterClockwise();
                }
            }
        }

    }
    #endregion
}
