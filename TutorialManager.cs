using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
    public GameObject tutorialMsg;
    public GameObject[] tutorialMessages;
    int currentIndex = -1;

    void Start() { ShowNextMessage(); }

    public void ShowNextMessage() {
        currentIndex++;
        if (currentIndex < tutorialMessages.Length) {
            if (currentIndex > 0) {
                tutorialMessages[currentIndex - 1].SetActive(false);
            }

            tutorialMessages[currentIndex].SetActive(true);
        } else {
            currentIndex--;
            CloseTutorial();
        }
    }
    
    public void CloseTutorial() {
        tutorialMessages[currentIndex].SetActive(false);
    }
}